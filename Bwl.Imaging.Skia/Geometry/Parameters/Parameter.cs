namespace Bwl.Imaging;

public class Parameter
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public bool Visible { get; set; } = true;
    public string Caption { get; set; } = string.Empty;
    public string AdditionalSettings { get; set; } = string.Empty;

    public Parameter()
    {
    }

    public Parameter(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public Parameter(string key, string value, bool visible)
    {
        Key = key;
        Value = value;
        Visible = visible;
    }

    public Parameter(string key, string value, string unit, bool visible, string caption, string additionalSettings)
    {
        Key = key;
        Value = value;
        Unit = unit;
        Visible = visible;
        Caption = caption;
        AdditionalSettings = additionalSettings;
    }
}