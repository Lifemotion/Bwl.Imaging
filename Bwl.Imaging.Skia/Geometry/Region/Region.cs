using SkiaSharp;

namespace Bwl.Imaging.Skia;


public class Region : Polygon, ICloneable
{

    protected const int _lineWidth = 2;

    protected ParametersDictionary _parameters = new ParametersDictionary();
    protected object _syncRoot = new object();

    public const string ParameterKey = "{RegionParameterKey}";

    public new ParametersDictionary Parameters
    {
        get
        {
            return _parameters;
        }
        set
        {
            _parameters = value;
        }
    }

    public string Caption { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SKColor Color { get; set; } = SKColors.LawnGreen;
    public string ID { get; set; } = string.Empty;
    public bool Visible { get; set; } = true;

    public Region() : base(true, new[] { new SKPoint(), new SKPoint(), new SKPoint(), new SKPoint() }) // Как Tetragon
    {
        ID = Guid.NewGuid().ToString();
    }

    public Region(SKPoint[] points) : base(true, points)
    {
        ID = Guid.NewGuid().ToString();
    }

    public Region(string id) : base(true, new[] { new SKPoint(), new SKPoint(), new SKPoint(), new SKPoint() }) // Как Tetragon
    {
        ID = id;
    }

    public Region(string id, SKPoint[] points) : base(true, points)
    {
        ID = id;
    }

    public virtual DisplayObject[] ToDisplayObjects(bool fullDisplay = true, float textSizeF = 0.013f, string channelIdxKey = "{ChannelIdxKey}")
    {
        lock (_syncRoot)
        {
            // Check-Up
            if (!Visible || !IsNotEmpty())
            {
                return new DisplayObject[] { };
            }
            var displayObjects = new List<DisplayObject>();

            // ChannelIdx / channelIdxDisplayObjectMarker
            int channelIdx;
            if (!Parameters.ContainsKey(channelIdxKey))
            {
                throw new Exception("Not Parameters.ContainsKey(SharedParametersKeys.ChannelIdxKey)");
            }
            else
            {
                channelIdx = Convert.ToInt32(Parameters[channelIdxKey].Value);
            }
            var channelIdxDisplayObjectMarker = new Parameter(channelIdxKey, channelIdx.ToString());

            // Region
            var regionDisplayObj = new DisplayObject(ID, Color, new Polygon(true, Points), true, true) { LineWidth = _lineWidth, Group = ID };
            regionDisplayObj.Parameters.Add(channelIdxDisplayObjectMarker);
            displayObjects.Add(regionDisplayObj);

            // Если выбран режим "показать всё"...
            if (fullDisplay)
            {
                float xF = Points.Max(item => item.X) + textSizeF;
                float yF = Points.Min(item => item.Y) - textSizeF / 1.5f;
                int textRowIdx = 0;
                if (Parameters is not null)
                {
                    string parameterID;
                    if (!string.IsNullOrEmpty(Caption))
                    {
                        // Caption
                        parameterID = ID + "Caption";
                        float minX = Points.Min(item => item.X) - textSizeF / 2.0f;
                        var captionDisplayObject = new DisplayObject(parameterID, Color, new TextObject(minX, yF + (textRowIdx - 3) * textSizeF, Caption, textSizeF)) { Group = ID + "Text", IsVisible = true };
                        captionDisplayObject.Parameters.Add(channelIdxDisplayObjectMarker);
                        displayObjects.Add(captionDisplayObject);
                    }

                    // Перебираем все параметры, формируя пары "читаемое описание ключа" - "значение"
                    Parameter[] parametersArr = Parameters.ToArray();
                    foreach (var parameterKVP in parametersArr)
                    {
                        // Если параметр не начинается с маркера "ключ параметра"
                        if (parameterKVP.Visible && !parameterKVP.Key.StartsWith(ParameterKey) && !parameterKVP.Key.StartsWith(channelIdxKey))
                        {

                            parameterID = ID + parameterKVP.Key;

                            string parameterKeyKVPValue;
                            var parameterKeyKVP = parametersArr.FirstOrDefault(item => (item.Key ?? "") == (ParameterKey + parameterKVP.Key ?? ""));
                            if (parameterKeyKVP is not null && parameterKeyKVP.Visible)
                            {
                                parameterKeyKVPValue = parameterKeyKVP.Value;
                                // "Читаемое" наименование параметра (parameterKeyKVP.Value) / Значение параметра (parameterKVP.Value)
                                var parameterKeyDisplayObject = new DisplayObject(parameterID + ParameterKey, Color, new TextObject(xF, yF + textRowIdx * textSizeF, "< " + parameterKeyKVPValue + " >", textSizeF)) { Group = ID + "Text", IsVisible = true };
                                var parameterValueDisplayObject = new DisplayObject(parameterID, Color, new TextObject(xF, yF + (textRowIdx + 3) * textSizeF, parameterKVP.Value, textSizeF)) { Group = ID + "Text", IsVisible = true };

                                parameterKeyDisplayObject.Parameters.Add(channelIdxDisplayObjectMarker);
                                parameterValueDisplayObject.Parameters.Add(channelIdxDisplayObjectMarker);
                                displayObjects.Add(parameterKeyDisplayObject);
                                displayObjects.Add(parameterValueDisplayObject);
                                textRowIdx += 6;
                            }
                        }
                    }

                    // 'Position
                    // parameterID = ID + "Position"
                    // '"Читаемое" наименование параметра (parameterKeyKVP.Value) / Значение параметра (parameterKVP.Value)
                    // Dim positionKeyDisplayObject = New DisplayObject(parameterID + Region.ParameterKey, Color,
                    // New TextObject(xF, yF + textRowIdx * textSizeF, "< Положение >", textSizeF)) With {.Group = ID + "Text", .IsVisible = True}
                    // Dim positionValueDisplayObject = New DisplayObject(parameterID, Color,
                    // New TextObject(xF, yF + (textRowIdx + 3) * textSizeF, Position, textSizeF)) With {.Group = ID + "Text", .IsVisible = True}
                    // positionKeyDisplayObject.Parameters.Add(channelIdxDisplayObjectMarker)
                    // positionValueDisplayObject.Parameters.Add(channelIdxDisplayObjectMarker)
                    // displayObjects.Add(positionKeyDisplayObject)
                    // displayObjects.Add(positionValueDisplayObject)
                }
            }

            return displayObjects.ToArray();
        }
    }

    public virtual object Clone()
    {
        return CloneWithNewPoints(Points);
    }

    public virtual Region CloneWithNewPoints(SKPoint[] points)
    {
        var obj = new Region(points)
        {
            Caption = Caption,
            Description = Description,
            Color = Color,
            ID = ID,
            Visible = Visible
        };
        foreach (var kvp in Parameters.ToArray())
            obj.Parameters.Add(kvp.Key, kvp.Value, kvp.Visible);
        return obj;
    }

    private string Position()
    {
        float X = Points.Average(item => item.X);
        float Y = Points.Average(item => item.Y);
        return string.Format("X:{0};Y:{1}", X.ToString("F3"), Y.ToString("F3"));
    }

    private bool IsNotEmpty()
    {
        return Points.Min(item => item.X) != Points.Max(item => item.X);
    }
}