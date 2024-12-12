using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;

namespace ImageProcess.ViewModel;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly string _outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");
    private bool _isProcessing;

    [ObservableProperty]
    private ObservableCollection<ImageFile> imageFiles = new();

    [ObservableProperty]
    private ImageFile? selectedFile;

    [ObservableProperty]
    private ObservableCollection<string> processTypes;

    [ObservableProperty]
    private string? selectedProcessType;

    public MainWindowViewModel()
    {
        // 初始化处理类型列表
        ProcessTypes = new ObservableCollection<string>
        {
            "灰度",
            "放大至200%",
            "缩小至50%",
            "顺时针旋转90°",
            "逆时针旋转90°",
            "边缘检测",
            "二值化",
            "模糊"
        };
        
        // 设置默认选中第一项
        SelectedProcessType = ProcessTypes.FirstOrDefault();
        
        // 确保输出目录存在
        if (!Directory.Exists(_outputDirectory))
        {
            Directory.CreateDirectory(_outputDirectory);
        }
    }

    public bool IsProcessing
    {
        get => _isProcessing;
        set
        {
            SetProperty(ref _isProcessing, value);
            StartProcessCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand(CanExecute = nameof(CanStartProcess))]
    private async Task StartProcess()
    {
        if (SelectedProcessType != "灰度") return;
        
        IsProcessing = true;
        try
        {
            foreach (var file in ImageFiles)
            {
                // 生成输出文件路径
                string outputPath = Path.Combine(_outputDirectory, 
                    $"{Path.GetFileNameWithoutExtension(file.FilePath)}_gray{Path.GetExtension(file.FilePath)}");

                // 处理图像
                bool success = await Task.Run(() => Utility.ImageProcess.ToGrayScale(file.FilePath, outputPath));
                
                // 更新状态
                file.Status = success ? "[已处理]" : "[处理失败]";
            }
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private bool CanStartProcess()
    {
        return !IsProcessing && ImageFiles.Any() && SelectedProcessType != null;
    }

    [RelayCommand]
    private void AddImages()
    {
        var dialog = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "图像文件|*.jpg;*.jpeg;*.png;*.bmp"
        };

        if (dialog.ShowDialog() == true)
        {
            foreach (var file in dialog.FileNames)
            {
                ImageFiles.Add(new ImageFile { FilePath = file, Status = "[待处理]" });
            }
        }
    }
}

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