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
}