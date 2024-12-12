using System.Windows.Media.Imaging;

namespace ImageProcess.ViewModel;

public class ShowImagesWindowViewModel
{
    public BitmapImage? OriginalImage { get; }
    public BitmapImage? ProcessedImage { get; }
    public string Title { get; }

    public ShowImagesWindowViewModel(string originalPath, string processedPath, string processType)
    {
        Title = $"图像对比 - {processType}";

        try
        {
            OriginalImage = new BitmapImage(new Uri(originalPath));
            ProcessedImage = new BitmapImage(new Uri(processedPath));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载图片出错: {ex.Message}");
        }
    }
}