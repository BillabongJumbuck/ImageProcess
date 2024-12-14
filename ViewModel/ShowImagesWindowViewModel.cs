using System.Windows;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ImageProcess.ViewModel;

public class ShowImagesWindowViewModel : ObservableObject
{
    public BitmapImage? OriginalImage { get; }
    public BitmapImage? ProcessedImage { get; }
    public string Title { get; }
    public GridLength LeftColumnWidth { get; }
    public GridLength RightColumnWidth { get; }

    public ShowImagesWindowViewModel(string originalPath, string processedPath, string processType)
    {
        Title = $"图像对比 - {processType}";

        try
        {
            OriginalImage = new BitmapImage(new Uri(originalPath));
            ProcessedImage = new BitmapImage(new Uri(processedPath));

            // 根据处理类型设置列宽
            (LeftColumnWidth, RightColumnWidth) = processType switch
            {
                "放大至200%" => (new GridLength(1, GridUnitType.Star), new GridLength(2, GridUnitType.Star)),
                "缩小至50%" => (new GridLength(2, GridUnitType.Star), new GridLength(1, GridUnitType.Star)),
                _ => (new GridLength(1, GridUnitType.Star), new GridLength(1, GridUnitType.Star))
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载图片出错: {ex.Message}");
        }
    }
}