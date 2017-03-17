using UnityEngine;


/// <summary>
/// For binary images only
/// </summary>
public class SimpleEdgeDetector
{
    private static readonly int[,] SobelHorizontalKernel =
    {
        { -1, 0, 1 },
        { -2, 0, 2 },
        { -1, 0, 1 }
    };

    private static readonly int[,] SobelVerticalKernel =
    {
        { -1, -2, -1 },
        {  0,  0,  0 },
        {  1,  2,  1 }
    };

    private static readonly int[,] PrewittHorizontalKernel =
    {
        { -1, 0, 1 },
        { -1, 0, 1 },
        { -1, 0, 1 }
    };

    private static readonly int[,] PrewittVerticalKernel =
    {
        { -1, -1, -1 },
        {  0,  0,  0 },
        {  1,  1,  1 }
    };

    private static readonly int[,] RobertsCrossHorizontalKernel =
    {
        { 1,  0 },
        { 0, -1 }
    };

    private static readonly int[,] RobertsCrossVerticalKernel =
    {
        {  0, 1 },
        { -1, 0 }
    };


    public static float[,] Sobel(int[,] matrix, bool adjustBorder = false)
    {
        return Convolve(SobelHorizontalKernel, SobelVerticalKernel, matrix, adjustBorder);
    }

    public static float[,] Prewitt(int[,] matrix, bool adjustBorder = false)
    {
        return Convolve(PrewittHorizontalKernel, PrewittVerticalKernel, matrix, adjustBorder);
    }

    public static float[,] RobertsCross(int[,] matrix, bool adjustBorder = false)
    {
        return Convolve(RobertsCrossHorizontalKernel, RobertsCrossVerticalKernel, matrix, adjustBorder);
    }

    private static float[,] Convolve(int[,] HorizontalKernel, int[,] VerticalKernel, int[,] matrix, bool adjustBorder)
    {
        int h = matrix.GetLength(0);
        int w = matrix.GetLength(1);
        int kh = HorizontalKernel.GetLength(0);
        int kw = HorizontalKernel.GetLength(1);
        int dkh = kh / 2;
        int dkw = kw / 2;

        float[,] result = new float[matrix.GetLength(0), matrix.GetLength(1)];

        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                int horizontalConvolutionResult = 0;
                int verticalConvolutionResult = 0;

                for (int k = 0; k < kh; k++)
                {
                    for (int m = 0; m < kw; m++)
                    {
                        int verticalOffset = i + k - dkh;
                        int hrizontalOffset = j + m - dkw;
                        int pixel = 0;

                        if (verticalOffset >= 0 && verticalOffset < h && hrizontalOffset >= 0 && hrizontalOffset < w)
                        {
                            pixel = matrix[verticalOffset, hrizontalOffset];
                        }

                        horizontalConvolutionResult += pixel * HorizontalKernel[k, m];
                        verticalConvolutionResult += pixel * VerticalKernel[k, m];
                    }
                }

                if (!adjustBorder)
                {
                    result[i, j] = Mathf.Sqrt(horizontalConvolutionResult * horizontalConvolutionResult +
                        verticalConvolutionResult * verticalConvolutionResult);
                }
                else
                {
                    if (matrix[i, j] > 0)
                    {
                        result[i, j] = Mathf.Clamp01(horizontalConvolutionResult * horizontalConvolutionResult +
                            verticalConvolutionResult * verticalConvolutionResult);
                    }
                }
            }
        }

        return result;
    }
}
