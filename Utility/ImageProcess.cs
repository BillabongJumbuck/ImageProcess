using OpenCvSharp;

namespace ImageProcess.Utility;

public static class ImageProcess
{
    /// <summary>
    /// 将图像转换为灰度图
    /// </summary>
    /// <param name="inputPath">输入图像路径</param>
    /// <param name="outputPath">输出图像路径</param>
    /// <returns>处理是否成功</returns>
    public static async Task<bool> ToGrayScale(string inputPath, string outputPath, CancellationToken cancellationToken)
    {
        try
        {
            using var src = Cv2.ImRead(inputPath);
            if (src.Empty()) return false;

            await Task.Delay(1000, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            using var gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            
            cancellationToken.ThrowIfCancellationRequested();
            Cv2.ImWrite(outputPath, gray);
            
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 放大图像至200%
    /// </summary>
    public static async Task<bool> Scale200(string inputPath, string outputPath, CancellationToken cancellationToken)
    {
        try
        {
            using var src = Cv2.ImRead(inputPath);
            if (src.Empty()) return false;

            await Task.Delay(1000, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            using var dst = new Mat();
            Cv2.Resize(src, dst, new Size(), 2.0, 2.0, InterpolationFlags.Linear);
            
            cancellationToken.ThrowIfCancellationRequested();
            Cv2.ImWrite(outputPath, dst);
            
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 缩小图像至50%
    /// </summary>
    public static async Task<bool> Scale50(string inputPath, string outputPath, CancellationToken cancellationToken)
    {
        try
        {
            using var src = Cv2.ImRead(inputPath);
            if (src.Empty()) return false;

            await Task.Delay(1000, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            using var dst = new Mat();
            Cv2.Resize(src, dst, new Size(), 0.5, 0.5, InterpolationFlags.Linear);
            
            cancellationToken.ThrowIfCancellationRequested();
            Cv2.ImWrite(outputPath, dst);
            
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 顺时针旋转90度
    /// </summary>
    public static async Task<bool> RotateClockwise90(string inputPath, string outputPath, CancellationToken cancellationToken)
    {
        try
        {
            using var src = Cv2.ImRead(inputPath);
            if (src.Empty()) return false;

            await Task.Delay(1000, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            using var dst = new Mat();
            Cv2.Rotate(src, dst, RotateFlags.Rotate90Clockwise);
            
            cancellationToken.ThrowIfCancellationRequested();
            Cv2.ImWrite(outputPath, dst);
            
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 逆时针旋转90度
    /// </summary>
    public static async Task<bool> RotateCounterClockwise90(string inputPath, string outputPath, CancellationToken cancellationToken)
    {
        try
        {
            using var src = Cv2.ImRead(inputPath);
            if (src.Empty()) return false;

            await Task.Delay(1000, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            using var dst = new Mat();
            Cv2.Rotate(src, dst, RotateFlags.Rotate90Counterclockwise);
            
            cancellationToken.ThrowIfCancellationRequested();
            Cv2.ImWrite(outputPath, dst);
            
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 边缘检测
    /// </summary>
    public static async Task<bool> EdgeDetection(string inputPath, string outputPath, CancellationToken cancellationToken)
    {
        try
        {
            using var src = Cv2.ImRead(inputPath);
            if (src.Empty()) return false;

            await Task.Delay(1000, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            using var gray = new Mat();
            using var edges = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            
            cancellationToken.ThrowIfCancellationRequested();
            Cv2.Canny(gray, edges, 100, 200);
            
            cancellationToken.ThrowIfCancellationRequested();
            Cv2.ImWrite(outputPath, edges);
            
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 图像二值化
    /// </summary>
    public static async Task<bool> Threshold(string inputPath, string outputPath, CancellationToken cancellationToken)
    {
        try
        {
            using var src = Cv2.ImRead(inputPath);
            if (src.Empty()) return false;

            await Task.Delay(1000, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            using var gray = new Mat();
            using var binary = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            
            cancellationToken.ThrowIfCancellationRequested();
            Cv2.Threshold(gray, binary, 127, 255, ThresholdTypes.Binary);
            
            cancellationToken.ThrowIfCancellationRequested();
            Cv2.ImWrite(outputPath, binary);
            
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 图像模糊
    /// </summary>
    public static async Task<bool> Blur(string inputPath, string outputPath, CancellationToken cancellationToken)
    {
        try
        {
            using var src = Cv2.ImRead(inputPath);
            if (src.Empty()) return false;

            await Task.Delay(1000, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            using var dst = new Mat();
            Cv2.GaussianBlur(src, dst, new Size(65, 65), 0);
            
            cancellationToken.ThrowIfCancellationRequested();
            Cv2.ImWrite(outputPath, dst);
            
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return false;
        }
    }
}