using SkiaSharp;

namespace Bwl.Imaging.Skia;

/// <summary>
/// Простой детектор движения (изменений в кадре). Для каждого видеоисточника должен быть свой детектор.
/// </summary>
public class MoveDetector
{
    /// <summary>
    /// Порог разности яркости, при котором точка считается изменившейся. Для фильтрации шумов. Можно не менять.
    /// </summary>
    /// <returns></returns>
    public int PointDiffThreshSetting { get; set; } = 5;
    /// <summary>
    /// Порог изменений в кадре, после которых обнаруживается движение. Доля изменившихся точек относительно общего количества пикселей кадра.
    /// </summary>
    /// <returns></returns>
    public double MoveThresholdSetting { get; set; } = 1.0d;
    /// <summary>
    /// В течение какого количества кадров БЕЗ движения после кадра С движением будет выдаваться результат наличия движения.
    /// </summary>
    /// <returns></returns>
    public int AfterMoveSetting { get; set; } = 1;
    public event LoggerEventHandler Logger;

    public delegate void LoggerEventHandler(string @type, string msg);

    private int _afterMoveCounter;

    public bool Process(SKBitmap image)
    {
        var bmp = image.CloneResized(64, 32);
        var mtr = bmp.ToGrayMatrix();
        return Process(mtr);
    }

    private GrayMatrix _Process_lastMatrix = default;

    public bool Process(GrayMatrix matrix)
    {
        lock (this)
        {
            if (_Process_lastMatrix is not null && _Process_lastMatrix.Width == matrix.Width && _Process_lastMatrix.Height == matrix.Height)
            {
                var diff = default(long);
                var cnt = default(int);
                for (int y = 0, loopTo = _Process_lastMatrix.Height - 1; y <= loopTo; y++)
                {
                    for (int x = 0, loopTo1 = _Process_lastMatrix.Width - 1; x <= loopTo1; x++)
                    {
                        int pd = Math.Abs(matrix.GetGrayPixel(x, y) - _Process_lastMatrix.GetGrayPixel(x, y));
                        if (pd > PointDiffThreshSetting)
                            diff += pd;
                        cnt += 1;
                    }
                }
                double val = diff / (double)cnt;
                _Process_lastMatrix = matrix.Clone();
                bool result = val > MoveThresholdSetting;
                if (result == true)
                {
                    _afterMoveCounter = AfterMoveSetting;
                    Logger?.Invoke("DBG", "Move TRUE, Diff: " + val.ToString("0.0") + ", Thresh: " + MoveThresholdSetting.ToString("0.0"));
                }
                else if (_afterMoveCounter > 0)
                {
                    result = true;
                    _afterMoveCounter -= 1;
                    Logger?.Invoke("DBG", "Move TRUE AFTERMOVE (" + _afterMoveCounter.ToString() + "), Diff: " + val.ToString("0.0") + ", Thresh: " + MoveThresholdSetting.ToString("0.0"));
                }
                else
                {
                    Logger?.Invoke("DBG", "Move FALSE, Diff: " + val.ToString("0.0") + ", Thresh: " + MoveThresholdSetting.ToString("0.0"));
                }
                return result;
            }
            else
            {
                Logger?.Invoke("DBG", "No last frame or last frame has different size");
                _Process_lastMatrix = matrix.Clone();
                return false;
            }
        }
    }
}