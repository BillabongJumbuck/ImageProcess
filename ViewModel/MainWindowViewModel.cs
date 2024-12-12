using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using ImageProcess.Utility;
using System.Runtime.InteropServices;
using System.Threading;

namespace ImageProcess.ViewModel;

public partial class MainWindowViewModel : ObservableObject
{
    private CancellationTokenSource? _cancellationTokenSource;
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
        Console.WriteLine($"开始处理：{SelectedProcessType}");
        
        IsProcessing = true;
        _cancellationTokenSource = new CancellationTokenSource();
        
        try
        {
            var mainWindowHandle = WindowsMessage.FindWindow(null, "图像处理");
            
            for (int i = 0; i < ImageFiles.Count; i++)
            {
                var file = ImageFiles[i];
                string suffix = SelectedProcessType switch
                {
                    "灰度" => "_gray",
                    "放大至200%" => "_scale200",
                    "缩小至50%" => "_scale50",
                    "顺时针旋转90°" => "_rotate_cw",
                    "逆时针旋转90°" => "_rotate_ccw",
                    "边缘检测" => "_edge",
                    "二值化" => "_binary",
                    "模糊" => "_blur",
                    _ => "_processed"
                };

                string outputPath = Path.Combine(_outputDirectory, 
                    $"{Path.GetFileNameWithoutExtension(file.FilePath)}{suffix}{Path.GetExtension(file.FilePath)}");

                // 发送处理中消息
                WindowsMessage.PostMessage(mainWindowHandle, 
                    WindowsMessage.WM_PROGRESS_UPDATE, 
                    (IntPtr)i,  // 文件索引
                    (IntPtr)0); // 状态：处理中

                bool success = await Task.Run(() =>
                {
                    return SelectedProcessType switch
                    {
                        "灰度" => Utility.ImageProcess.ToGrayScale(file.FilePath, outputPath),
                        "放大至200%" => Utility.ImageProcess.Scale200(file.FilePath, outputPath),
                        "缩小至50%" => Utility.ImageProcess.Scale50(file.FilePath, outputPath),
                        "顺时针旋转90°" => Utility.ImageProcess.RotateClockwise90(file.FilePath, outputPath),
                        "逆时针旋转90°" => Utility.ImageProcess.RotateCounterClockwise90(file.FilePath, outputPath),
                        "边缘检测" => Utility.ImageProcess.EdgeDetection(file.FilePath, outputPath),
                        "二值化" => Utility.ImageProcess.Threshold(file.FilePath, outputPath),
                        "模糊" => Utility.ImageProcess.Blur(file.FilePath, outputPath),
                        _ => false
                    };
                });
                
                // 发送处理完成/失败消息
                WindowsMessage.PostMessage(mainWindowHandle, 
                    WindowsMessage.WM_PROGRESS_UPDATE, 
                    (IntPtr)i,                    // 文件索引
                    (IntPtr)(success ? 1 : 2));   // 状态：1=完成，2=失败
            }
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private bool CanStartProcess()
    {
        var hasFiles = ImageFiles.Any();
        var hasSelectedType = !string.IsNullOrEmpty(SelectedProcessType);
        var notProcessing = !IsProcessing;
        
        Console.WriteLine($"检查处理条件：");
        Console.WriteLine($"- 是否有文件：{hasFiles}");
        Console.WriteLine($"- 是否选择处理类型：{hasSelectedType}（当前选择：{SelectedProcessType ?? "无"}）");
        Console.WriteLine($"- 是否未在处理中：{notProcessing}");
        
        return notProcessing && hasFiles && hasSelectedType;
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
            StartProcessCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand]
    private void RemoveSelected()
    {
        var selectedItems = ImageFiles.Where(x => x == SelectedFile).ToList();
        foreach (var item in selectedItems)
        {
            ImageFiles.Remove(item);
        }
        StartProcessCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void ViewResult()
    {
        if (SelectedFile == null) return;
        
        // 根据处理类型选择对应的文件后缀
        string suffix = SelectedProcessType switch
        {
            "灰度" => "_gray",
            "放大至200%" => "_scale200",
            "缩小至50%" => "_scale50",
            "顺时针旋转90°" => "_rotate_cw",
            "逆时针旋转90°" => "_rotate_ccw",
            "边缘检测" => "_edge",
            "二值化" => "_binary",
            "模糊" => "_blur",
            _ => "_processed"
        };

        string outputPath = Path.Combine(_outputDirectory, 
            $"{Path.GetFileNameWithoutExtension(SelectedFile.FilePath)}{suffix}{Path.GetExtension(SelectedFile.FilePath)}");
            
        if (File.Exists(outputPath))
        {
            // 使用系统默认程序打开图片
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = outputPath,
                UseShellExecute = true
            });
        }
    }

    [RelayCommand]
    private void CancelProcess()
    {
        IsProcessing = false;
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