using System;
using System.Collections.Generic;
using System.Linq;

namespace Bwl.Imaging
{
    public class CommonFloatMatrix
    {
        protected List<double[]> _matrices;
        protected int _width;
        protected int _height;

        public CommonFloatMatrix(int channels, int width, int height)
        {
            if (channels < 1)
                throw new ArgumentException("channels must be >0");
            if (channels > 1024)
                throw new ArgumentException("channels must be <=1024 due to memory overflow protection");
            if (width < 1)
                throw new ArgumentException("width must be >0");
            if (height < 1)
                throw new ArgumentException("height must be >0");
            _width = width;
            _height = height;
            _matrices = new List<double[]>();
            for (int i = 1, loopTo = channels; i <= loopTo; i++)
            {
                var channel = new double[(width * height)];
                _matrices.Add(channel);
            }
        }

        public CommonFloatMatrix(IEnumerable<double[]> matrices, int width, int height)
        {
            if (matrices is null)
                throw new ArgumentException("matrices must not be null");
            if (matrices.Count() == 0)
                throw new ArgumentException("matrices must contain at least one matrix");
            _width = width;
            _height = height;
            _height = matrices.ElementAtOrDefault(0).Length / width;
            _matrices = new List<double[]>();

            foreach (var mtr in matrices)
            {
                if (mtr.Length != width * height)
                    throw new Exception("all matrices must have width*height elements");
                _matrices.Add(mtr);
            }
        }

        public CommonFloatMatrix(IEnumerable<int[]> matrices, int width, int height)
        {
            if (matrices is null)
                throw new ArgumentException("matrices must not be null");
            if (matrices.Count() == 0)
                throw new ArgumentException("matrices must contain at least one matrix");
            _width = width;
            _height = height;
            _matrices = new List<double[]>();

            foreach (var mtr in matrices)
            {
                if (mtr.Length != width * height)
                    throw new Exception("all matrices must have width*height elements");
                var channel = new double[mtr.Length];

                for (int i = 0, loopTo = mtr.Length - 1; i <= loopTo; i++)
                {
                    double pixel = mtr[i];
                    channel[i] = pixel;
                }
                _matrices.Add(channel);
            }
        }

        public CommonFloatMatrix(IEnumerable<double[]> matrices, int width, int height, double multiplier)
        {
            if (matrices is null)
                throw new ArgumentException("matrices must not be null");
            if (matrices.Count() == 0)
                throw new ArgumentException("matrices must contain at least one matrix");
            _width = width;
            _height = height;
            _matrices = new List<double[]>();

            foreach (var mtr in matrices)
            {
                if (mtr.Length != width * height)
                    throw new Exception("all matrices must have width*height elements");
                var channel = new double[mtr.Length];

                for (int i = 0, loopTo = mtr.Length - 1; i <= loopTo; i++)
                {
                    double pixel = mtr[i] * multiplier;
                    channel[i] = pixel;
                }
                _matrices.Add(channel);
            }
        }

        public CommonFloatMatrix(IEnumerable<int[]> matrices, int width, int height, double multiplier)
        {
            if (matrices is null)
                throw new ArgumentException("matrices must not be null");
            if (matrices.Count() == 0)
                throw new ArgumentException("matrices must contain at least one matrix");
            _width = width;
            _height = height;
            _matrices = new List<double[]>();

            foreach (var mtr in matrices)
            {
                if (mtr.Length != width * height)
                    throw new Exception("all matrices must have width*height elements");
                var channel = new double[mtr.Length];

                for (int i = 0, loopTo = mtr.Length - 1; i <= loopTo; i++)
                {
                    double pixel = mtr[i] * multiplier;
                    channel[i] = pixel;
                }
                _matrices.Add(channel);
            }
        }

        public double GetMatrixPixel(int channel, int x, int y)
        {
            return _matrices[channel][x + y * Width];
        }
        public void SetMatrixPixel(int channel, int x, int y, double value)
        {
            _matrices[channel][x + y * Width] = value;
        }

        public double[] GetMatrix(int channel)
        {
            return _matrices[channel];
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

        public double[] CloneMatrix(double[] matrix)
        {
            var arr = new double[matrix.Length];
            Array.Copy(matrix, arr, matrix.Length);
            return arr;
        }

        public double[] ResizeMatrixHalf(double[] matrix)
        {
            var result = new double[(_width / 2 * (_height / 2))];
            for (int y = 0, loopTo = _height / 2 - 1; y <= loopTo; y++)
            {
                int lineOffset1 = y * 2 * _width;
                int lineOffset2 = (y * 2 + 1) * _width;
                int resOffset = y * (_width / 2);
                for (int x = 0, loopTo1 = _width / 2 - 1; x <= loopTo1; x++)
                {
                    double elem = matrix[x * 2 + lineOffset1];
                    elem += matrix[x * 2 + 1 + lineOffset1];
                    elem += matrix[x * 2 + lineOffset2];
                    elem += matrix[x * 2 + 1 + lineOffset2];
                    result[x + resOffset] = elem / 4d;
                }
            }
            return result;
        }

        public double[] ResizeMatrixTwo(double[] matrix)
        {
            var result = new double[(_width * 2 * _height * 2)];
            for (int y = 0, loopTo = Height - 1; y <= loopTo; y++)
            {
                int lineOffset1 = y * 2 * _width * 2;
                int lineOffset2 = (y * 2 + 1) * _width * 2;
                int offset = y * _width;
                for (int x = 0, loopTo1 = _width - 1; x <= loopTo1; x++)
                {
                    double elem = matrix[x + offset];
                    result[x * 2 + lineOffset1] = elem;
                    result[x * 2 + 1 + lineOffset1] = elem;
                    result[x * 2 + lineOffset2] = elem;
                    result[x * 2 + 1 + lineOffset2] = elem;
                }
            }
            return result;
        }

        public virtual CommonFloatMatrix Clone()
        {
            var list = new List<double[]>();
            foreach (var mtr in _matrices)
                list.Add(CloneMatrix(mtr));
            return new CommonFloatMatrix(list, Width, Height);
        }

        public virtual CommonFloatMatrix ResizeTwo()
        {
            var list = new List<double[]>();
            foreach (var mtr in _matrices)
                list.Add(ResizeMatrixTwo(mtr));
            return new CommonFloatMatrix(list, Width * 2, Height * 2);
        }

        public virtual CommonFloatMatrix ResizeHalf()
        {
            var list = new List<double[]>();
            foreach (var mtr in _matrices)
                list.Add(ResizeMatrixHalf(mtr));
            return new CommonFloatMatrix(list, Width / 2, Height / 2);
        }
    }
}