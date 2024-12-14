using System.IO;
using OpenCvSharp;

namespace ImageProcess.Utility;

public static class ImageProcess
{
    private static async Task<bool> ProcessImage(string inputPath, string outputPath, 
        CancellationToken cancellationToken, Func<Mat, Mat> processFunc)
    {
        try
        {
            using var src = Cv2.ImRead(inputPath);
            if (src.Empty()) return false;

            await Task.Delay(1000, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            using var dst = processFunc(src);
            
            cancellationToken.ThrowIfCancellationRequested();
            
            // 如果取消了,就不写入文件
            if (!cancellationToken.IsCancellationRequested)
            {
                Cv2.ImWrite(outputPath, dst);
                return true;
            }
            return false;
        }
        catch (OperationCanceledException)
        {
            // 如果文件已经生成,则删除它
            if (File.Exists(outputPath))
            {
                try { File.Delete(outputPath); } catch { }
            }
            throw;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 将图像转换为灰度图
    /// </summary>
    /// <param name="inputPath">输入图像路径</param>
    /// <param name="outputPath">输出图像路径</param>
    /// <returns>处理是否成功</returns>
    public static async Task<bool> ToGrayScale(string inputPath, string outputPath, CancellationToken cancellationToken)
    {
        return await ProcessImage(inputPath, outputPath, cancellationToken, src =>
        {
            var gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            return gray;
        });
    }

    /// <summary>
    /// 放大图像至200%
    /// </summary>
    public static async Task<bool> Scale200(string inputPath, string outputPath, CancellationToken cancellationToken)
    {
        return await ProcessImage(inputPath, outputPath, cancellationToken, src =>
        {
            var dst = new Mat();
            Cv2.Resize(src, dst, new Size(), 1.414, 1.414, InterpolationFlags.Linear);
            return dst;
        });
    }

    /// <summary>
    /// 缩小图像至50%
    /// </summary>
    public static async Task<bool> Scale50(string inputPath, string outputPath, CancellationToken cancellationToken)
    {
        return await ProcessImage(inputPath, outputPath, cancellationToken, src =>
        {
            var dst = new Mat();
            Cv2.Resize(src, dst, new Size(), 0.707, 0.707, InterpolationFlags.Linear);
            return dst;
        });
    }

    /// <summary>
    /// 顺时针旋转90度
    /// </summary>
    public static async Task<bool> RotateClockwise90(string inputPath, string outputPath, CancellationToken cancellationToken)
    {
        return await ProcessImage(inputPath, outputPath, cancellationToken, src =>
        {
            var dst = new Mat();
            Cv2.Rotate(src, dst, RotateFlags.Rotate90Clockwise);
            return dst;
        });
    }

    /// <summary>
    /// 逆时针旋转90度
    /// </summary>
    public static async Task<bool> RotateCounterClockwise90(string inputPath, string outputPath, CancellationToken cancellationToken)
    {
        return await ProcessImage(inputPath, outputPath, cancellationToken, src =>
        {
            var dst = new Mat();
            Cv2.Rotate(src, dst, RotateFlags.Rotate90Counterclockwise);
            return dst;
        });
    }

    /// <summary>
    /// 边缘检测
    /// </summary>
    public static async Task<bool> EdgeDetection(string inputPath, string outputPath, CancellationToken cancellationToken)
    {
        return await ProcessImage(inputPath, outputPath, cancellationToken, src =>
        {
            var gray = new Mat();
            var edges = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Canny(gray, edges, 100, 200);
            gray.Dispose();
            return edges;
        });
    }

    /// <summary>
    /// 图像二值化
    /// </summary>
    public static async Task<bool> Threshold(string inputPath, string outputPath, CancellationToken cancellationToken)
    {
        return await ProcessImage(inputPath, outputPath, cancellationToken, src =>
        {
            var gray = new Mat();
            var binary = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(gray, binary, 127, 255, ThresholdTypes.Binary);
            gray.Dispose();
            return binary;
        });
    }

    /// <summary>
    /// 图像模糊
    /// </summary>
    public static async Task<bool> Blur(string inputPath, string outputPath, CancellationToken cancellationToken)
    {
        return await ProcessImage(inputPath, outputPath, cancellationToken, src =>
        {
            var dst = new Mat();
            Cv2.GaussianBlur(src, dst, new Size(65, 65), 0);
            return dst;
        });
    }
}