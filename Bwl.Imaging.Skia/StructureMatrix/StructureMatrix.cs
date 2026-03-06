namespace Bwl.Imaging.Skia;

public class StructureMatrix<T> where T : struct, IRGBConvertable
{
    protected T[] _matrix;
    protected int _width;
    protected int _height;

    public StructureMatrix(int width, int height)
    {
        if (width < 1)
            throw new ArgumentException("width must be >0");
        if (height < 1)
            throw new ArgumentException("height must be >0");
        _width = width;
        _height = height;
        var matrix = new T[(width * height)];
        _matrix = matrix;
    }

    public StructureMatrix(T[] matrix, int width, int height)
    {
        if (width < 1)
            throw new ArgumentException("width must be >0");
        if (height < 1)
            throw new ArgumentException("height must be >0");
        if (height * width != matrix.Length)
            throw new ArgumentException("matrix length <> width*height");
        _width = width;
        _height = height;
        _matrix = matrix;
    }

    public T this[int x, int y]
    {
        get
        {
            return _matrix[x + y * Width];
        }
        set
        {
            _matrix[x + y * Width] = value;
        }
    }

    public T[] Matrix
    {
        get
        {
            return _matrix;
        }
    }

    public int FitX(int x)
    {
        if (x < 0)
            x = 0;
        if (x >= Width)
            x = Width - 1;
        return x;
    }

    public int FitY(int y)
    {
        if (y < 0)
            y = 0;
        if (y >= Height)
            y = Height - 1;
        return y;
    }

    public int HalfWidth()
    {
        return Width / 2;
    }

    public int HalfHeight()
    {
        return Height / 2;
    }

    public int Width
    {
        get
        {
            return _width;
        }
    }

    public int Height
    {
        get
        {
            return _height;
        }
    }

    public T[] CloneMatrix(T[] matrix)
    {
        var arr = new T[matrix.Length];
        Array.Copy(matrix, arr, matrix.Length);
        return arr;
    }

    public T[] ResizeMatrixHalf(T[] matrix)
    {
        throw new NotImplementedException();
        var result = new T[(_width / 2 * (_height / 2))];
        for (int y = 0, loopTo = _height / 2 - 1; y <= loopTo; y++)
        {
            int lineOffset1 = y * 2 * _width;
            int lineOffset2 = (y * 2 + 1) * _width;
            int resOffset = y * (_width / 2);
            for (int x = 0, loopTo1 = _width / 2 - 1; x <= loopTo1; x++)
            {
                // Dim point As Integer = 0
                // point += matrix(x * 2 + lineOffset1)
                // Point += matrix(x * 2 + 1 + lineOffset1)
                // Point += matrix(x * 2 + lineOffset2)
                // Point += matrix(x * 2 + 1 + lineOffset2)
                // Point \= 4
                // result(x + resOffset) = point
            }
        }
        return result;
    }

    public T[] ResizeMatrixTwo(T[] matrix)
    {
        throw new NotImplementedException();
        var result = new T[(_width * 2 * _height * 2)];
        for (int y = 0, loopTo = Height - 1; y <= loopTo; y++)
        {
            int lineOffset1 = y * 2 * _width * 2;
            int lineOffset2 = (y * 2 + 1) * _width * 2;
            int offset = y * _width;
            for (int x = 0, loopTo1 = _width - 1; x <= loopTo1; x++)
            {
                // result(x * 2 + lineOffset1) = elem
                // result(x * 2 + 1 + lineOffset1) = elem
                // result(x * 2 + lineOffset2) = elem
                // result(x * 2 + 1 + lineOffset2) = elem
                var elem = matrix[x + offset];
            }
        }
        return result;
    }

    public RGBMatrix ToRGBMatrix()
    {
        var rgbm = new RGBMatrix(Width, Height);
        for (int i = 0, loopTo = _matrix.Length - 1; i <= loopTo; i++)
        {
            var pixel = _matrix[i].ToRGB();
            rgbm.Red[i] = pixel.R;
            rgbm.Green[i] = pixel.G;
            rgbm.Blue[i] = pixel.B;
        }
        return rgbm;
    }

    public virtual StructureMatrix<T> Clone()
    {
        return new StructureMatrix<T>(CloneMatrix(_matrix), Width, Height);
    }

    public virtual StructureMatrix<T> ResizeTwo()
    {
        throw new NotImplementedException();
        // Dim list As New List(Of Integer())
        // For Each mtr In _matrices
        // list.Add(ResizeMatrixTwo(mtr))
        // Next
        // Return New CommonMatrix(list, Width * 2, Height * 2)
    }

    public virtual StructureMatrix<T> ResizeHalf()
    {
        throw new NotImplementedException();
        // Dim list As New List(Of Integer())
        // For Each mtr In _matrices
        // list.Add(ResizeMatrixHalf(mtr))
        // Next
        // Return New CommonMatrix(list, Width \ 2, Height \ 2)
    }
}