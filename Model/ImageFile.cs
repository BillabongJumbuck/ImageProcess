using CommunityToolkit.Mvvm.ComponentModel;

namespace ImageProcess.Model;

public class ImageFile : ObservableObject
{
    private string filePath = string.Empty;

    public string FilePath
    {
        get => filePath;
        set => SetProperty(ref filePath, value);
    }

    private string status = "[待处理]";

    public string Status
    {
        get => status;
        set => SetProperty(ref status, value);
    }
}