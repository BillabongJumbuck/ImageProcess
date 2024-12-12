using OpenCvSharp;

namespace ImageProcess.Utility;

public class ImageProcess
{
    /// <summary>
    /// 将图像转换为灰度图
    /// </summary>
    /// <param name="inputPath">输入图像路径</param>
    /// <param name="outputPath">输出图像路径</param>
    /// <returns>处理是否成功</returns>
    public static bool ToGrayScale(string inputPath, string outputPath)
    {
        try
        {
            using var src = Cv2.ImRead(inputPath);
            if (src.Empty())
            {
                return false;
            }

            using var gray = new Mat();
            // 转换为灰度图
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            
            // 保存结果
            Cv2.ImWrite(outputPath, gray);
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 放大图像至200%
    /// </summary>
    public static bool Scale200(string inputPath, string outputPath)
    {
        try
        {
            using var src = Cv2.ImRead(inputPath);
            if (src.Empty()) return false;

            using var dst = new Mat();
            Cv2.Resize(src, dst, new Size(), 2.0, 2.0, InterpolationFlags.Linear);
            Cv2.ImWrite(outputPath, dst);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 缩小图像至50%
    /// </summary>
    public static bool Scale50(string inputPath, string outputPath)
    {
        try
        {
            using var src = Cv2.ImRead(inputPath);
            if (src.Empty()) return false;

            using var dst = new Mat();
            Cv2.Resize(src, dst, new Size(), 0.5, 0.5, InterpolationFlags.Linear);
            Cv2.ImWrite(outputPath, dst);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 顺时针旋转90度
    /// </summary>
    public static bool RotateClockwise90(string inputPath, string outputPath)
    {
        try
        {
            using var src = Cv2.ImRead(inputPath);
            if (src.Empty()) return false;

            using var dst = new Mat();
            Cv2.Rotate(src, dst, RotateFlags.Rotate90Clockwise);
            Cv2.ImWrite(outputPath, dst);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 逆时针旋转90度
    /// </summary>
    public static bool RotateCounterClockwise90(string inputPath, string outputPath)
    {
        try
        {
            using var src = Cv2.ImRead(inputPath);
            if (src.Empty()) return false;

            using var dst = new Mat();
            Cv2.Rotate(src, dst, RotateFlags.Rotate90Counterclockwise);
            Cv2.ImWrite(outputPath, dst);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 边缘检测
    /// </summary>
    public static bool EdgeDetection(string inputPath, string outputPath)
    {
        try
        {
            using var src = Cv2.ImRead(inputPath);
            if (src.Empty()) return false;

            using var gray = new Mat();
            using var edges = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Canny(gray, edges, 100, 200);
            Cv2.ImWrite(outputPath, edges);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 图像二值化
    /// </summary>
    public static bool Threshold(string inputPath, string outputPath)
    {
        try
        {
            using var src = Cv2.ImRead(inputPath);
            if (src.Empty()) return false;

            using var gray = new Mat();
            using var binary = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(gray, binary, 127, 255, ThresholdTypes.Binary);
            Cv2.ImWrite(outputPath, binary);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 图像模糊
    /// </summary>
    public static bool Blur(string inputPath, string outputPath)
    {
        try
        {
            using var src = Cv2.ImRead(inputPath);
            if (src.Empty()) return false;

            using var dst = new Mat();
            Cv2.GaussianBlur(src, dst, new Size(65, 65), 0);
            Cv2.ImWrite(outputPath, dst);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}