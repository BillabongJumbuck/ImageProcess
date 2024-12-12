using System.Windows;
using ImageProcess.ViewModel;

namespace ImageProcess.View;

public partial class ShowImagesWindow : Window
{
    public ShowImagesWindow(string originalPath, string processedPath, string processType)
    {
        InitializeComponent();
        DataContext = new ShowImagesWindowViewModel(originalPath, processedPath, processType);
    }
}