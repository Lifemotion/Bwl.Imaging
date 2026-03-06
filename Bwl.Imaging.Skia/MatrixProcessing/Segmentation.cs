namespace Bwl.Imaging.Skia;

public class Segmentation
{
    public static GrayMatrix CreateSegmentsMap(GrayMatrix matrix, int threshold = 10)
    {
        threshold = 30;
        var segments = new GrayMatrix(matrix.Width, matrix.Height);
        var segmentsValues = new GrayMatrix(matrix.Width, matrix.Height);
        int segmIndex = 1;
        int segmValue = matrix.Gray[0];

        int height = matrix.Height;
        int width = matrix.Width;
        double k = 0.8d;
        for (int y = 0, loopTo = matrix.Height - 1; y <= loopTo; y++)
        {
            int rowstart = y * matrix.Width;
            for (int x = 0, loopTo1 = matrix.Width - 1; x <= loopTo1; x++)
            {
                int pix = matrix.Gray[x + rowstart];
                if (Math.Abs(pix - segmValue) > threshold)
                {
                    segmIndex += 1;
                    segmValue = pix;
                }
                else
                {
                    segmValue = (int)Math.Round(segmValue * k + pix * (1d - k));
                }
                segments.Gray[x + rowstart] = segmIndex;
                segmentsValues.Gray[x + rowstart] = segmValue;
            }
        }

        for (int pass = 1; pass <= 4; pass++)
        {
            if (pass % 2 == 1)
            {
                for (int y = 0, loopTo2 = height - 2; y <= loopTo2; y++)
                {
                    int rowstart = y * width;
                    int nextRowstart = rowstart + width;
                    Filll(segments, segmentsValues, rowstart, nextRowstart, width, threshold);
                }
            }
            else
            {
                for (int y = height - 1; y >= 1; y -= 1)
                {
                    int rowstart = y * width;
                    int nextRowstart = rowstart - width;
                    Filll(segments, segmentsValues, rowstart, nextRowstart, width, threshold);
                }
            }
        }

        // FillSegments(segments.Gray, matrix.Width, matrix.Height)
        // FillSegmentsRev(segments.Gray, matrix.Width, matrix.Height)
        return segments;
    }

    private static void Filll(GrayMatrix segments, GrayMatrix segmentsValues, int rowstart, int nextRowstart, int width, int threshold)
    {
        for (int x = 0, loopTo = width - 1; x <= loopTo; x++)
        {
            int segm = segments.Gray[rowstart + x];
            int segmVal = segmentsValues.Gray[rowstart + x];
            int downSegm = segments.Gray[nextRowstart + x];
            int downSegmVal = segmentsValues.Gray[nextRowstart + x];

            if (Math.Abs(segmVal - downSegmVal) < threshold & downSegm > segm)
            {
                segmVal = (int)Math.Round(segmVal * 0.5d + downSegmVal * 0.5d);
                for (int i = x, loopTo1 = width - 1; i <= loopTo1; i++)
                {
                    if (segments.Gray[nextRowstart + i] != downSegm)
                        break;
                    segments.Gray[nextRowstart + i] = segm;
                    segmentsValues.Gray[nextRowstart + i] = segmVal;
                }
                for (int i = x; i >= 0; i -= 1)
                {
                    if (segments.Gray[nextRowstart + i] != downSegm)
                        break;
                    segments.Gray[nextRowstart + i] = segm;
                    segmentsValues.Gray[nextRowstart + i] = segmVal;
                }
            }
        }
    }

    public static GrayMatrix CreateSegmentsMapWithBinarize(GrayMatrix matrix, int binarizeThreshold = 120, bool invert = false, int passes = 1)
    {
        GrayMatrix segments;
        if (invert)
        {
            segments = FirstSegmentationInvert(matrix, binarizeThreshold);
        }
        else
        {
            segments = FirstSegmentation(matrix, binarizeThreshold);
        }
        for (int p = 1, loopTo = passes; p <= loopTo; p++)
        {
            FillSegments(segments.Gray, matrix.Width, matrix.Height);
            FillSegmentsRev(segments.Gray, matrix.Width, matrix.Height);
        }

        return segments;
    }

    private static GrayMatrix FirstSegmentationInvert(GrayMatrix matrix, int binarizeThreshold)
    {
        var segments = new GrayMatrix(matrix.Width, matrix.Height);
        int segmIndex = 1;
        for (int y = 0, loopTo = matrix.Height - 1; y <= loopTo; y++)
        {
            int rowstart = y * matrix.Width;
            bool last = false;
            for (int x = 0, loopTo1 = matrix.Width - 1; x <= loopTo1; x++)
            {
                int pix = matrix.Gray[x + rowstart];
                if (pix < binarizeThreshold)
                {
                    if (last == false)
                        segmIndex += 1;
                    segments.Gray[x + rowstart] = segmIndex;
                    last = true;
                }
                else
                {
                    last = false;
                }
            }
        }
        return segments;
    }

    private static GrayMatrix FirstSegmentation(GrayMatrix matrix, int binarizeThreshold)
    {
        var segments = new GrayMatrix(matrix.Width, matrix.Height);
        int segmIndex = 1;
        for (int y = 0, loopTo = matrix.Height - 1; y <= loopTo; y++)
        {
            int rowstart = y * matrix.Width;
            bool last = false;
            for (int x = 0, loopTo1 = matrix.Width - 1; x <= loopTo1; x++)
            {
                int pix = matrix.Gray[x + rowstart];
                if (pix > binarizeThreshold)
                {
                    if (last == false)
                        segmIndex += 1;
                    segments.Gray[x + rowstart] = segmIndex;
                    last = true;
                }
                else
                {
                    last = false;
                }
            }
        }
        return segments;
    }

    public static RGBMatrix ColorizeSegments(GrayMatrix segmentsMap)
    {
        var result = new RGBMatrix(segmentsMap.Width, segmentsMap.Height);
        for (int i = 0, loopTo = segmentsMap.Gray.Length - 1; i <= loopTo; i++)
        {
            int pix = segmentsMap.Gray[i];
            if (pix > 0)
            {
                var rnd = new Random(pix);
                result.Red[i] = rnd.Next(0, 255);
                result.Green[i] = rnd.Next(0, 255);
                result.Blue[i] = rnd.Next(0, 255);
            }
        }
        return result;
    }

    public static Segment[] CreateSegmentsList(GrayMatrix segmentsMap)
    {
        var segments = new List<Segment>();
        for (int y = 0, loopTo = segmentsMap.Height - 1; y <= loopTo; y++)
        {
            int rowstart = y * segmentsMap.Width;
            int last = 0;
            for (int x = 0, loopTo1 = segmentsMap.Width - 1; x <= loopTo1; x++)
            {
                int segm = segmentsMap.Gray[x + rowstart];
                if (segm > 0)
                {
                    bool found = false;
                    for (int i = segments.Count - 1; i >= 0; i -= 1)
                    {
                        {
                            var withBlock = segments[i];
                            if (withBlock.ID == segm)
                            {
                                found = true;
                                if (withBlock.Left > x)
                                    withBlock.Left = x;
                                if (withBlock.Top > y)
                                    withBlock.Top = y;
                                if (withBlock.Right < x)
                                    withBlock.Right = x;
                                if (withBlock.Bottom < y)
                                    withBlock.Bottom = y;
                                break;
                            }
                        }
                    }
                    if (!found)
                    {
                        var seg = new Segment();
                        seg.ID = segm;
                        seg.Left = x;
                        seg.Top = y;
                        seg.Width = 1;
                        seg.Height = 1;
                        segments.Add(seg);
                    }
                }
            }
        }
        return segments.ToArray();
    }

    public static Segment[] FilterSegmentsList(IEnumerable<Segment> segments, int minWidth, int minHeight, int maxWidth, int maxHeight)
    {
        var results = new List<Segment>();
        foreach (var segm in segments)
        {
            if (segm.Width > minWidth && segm.Height > minHeight && segm.Width < maxWidth && segm.Height < maxHeight)
            {
                results.Add(segm);
            }
        }
        return results.ToArray();
    }

    public static Segment[] FilterSegmentsList(IEnumerable<Segment> segments, float minWHratio, float maxWHRatio)
    {
        var results = new List<Segment>();
        foreach (var segm in segments)
        {
            if (segm.WHRatio > 0f && segm.WHRatio >= minWHratio && segm.WHRatio <= maxWHRatio)
            {
                results.Add(segm);
            }
        }
        return results.ToArray();
    }

    private static void FillSegments(int[] segments, int width, int height)
    {
        for (int y = 0, loopTo = height - 2; y <= loopTo; y++)
        {
            int rowstart = y * width;
            int nextRowstart = rowstart + width;
            for (int x = 0, loopTo1 = width - 1; x <= loopTo1; x++)
            {
                int segm = segments[rowstart + x];
                if (segm > 0)
                {
                    if (segments[nextRowstart + x] > 0 && segm < segments[nextRowstart + x])
                    {
                        for (int i = x, loopTo2 = width - 1; i <= loopTo2; i++)
                        {
                            if (segments[nextRowstart + i] == 0)
                                break;
                            segments[nextRowstart + i] = segm;
                        }
                        for (int i = x; i >= 0; i -= 1)
                        {
                            if (segments[nextRowstart + i] == 0)
                                break;
                            segments[nextRowstart + i] = segm;
                        }
                    }
                }
            }
        }
    }

    private static void FillSegmentsRev(int[] segments, int width, int height)
    {
        for (int y = height - 1; y >= 1; y -= 1)
        {
            int rowstart = y * width;
            int nextRowstart = rowstart - width;
            for (int x = 0, loopTo = width - 1; x <= loopTo; x++)
            {
                int segm = segments[rowstart + x];
                if (segm > 0)
                {
                    if (segments[nextRowstart + x] > 0 && segm < segments[nextRowstart + x])
                    {
                        for (int i = x, loopTo1 = width - 1; i <= loopTo1; i++)
                        {
                            if (segments[nextRowstart + i] == 0)
                                break;
                            segments[nextRowstart + i] = segm;
                        }
                        for (int i = x; i >= 0; i -= 1)
                        {
                            if (segments[nextRowstart + i] == 0)
                                break;
                            segments[nextRowstart + i] = segm;
                        }
                    }
                }
            }
        }
    }

}