
namespace Bwl.Imaging.Skia
{
    public class StructureMatrixRGB : StructureMatrix<RGB>
    {

        public StructureMatrixRGB(int width, int height) : base(width, height)
        {
        }

        public StructureMatrixRGB(RGB[] matrix, int width, int height) : base(matrix, width, height)
        {
        }

        public StructureMatrixRGB(RGBMatrix rgbmatrix) : base(rgbmatrix.Width, rgbmatrix.Height)
        {
            for (int i = 0, loopTo = _matrix.Length - 1; i <= loopTo; i++)
                _matrix[i] = new RGB(rgbmatrix.Red[i], rgbmatrix.Green[i], rgbmatrix.Blue[i]);
        }
    }
}