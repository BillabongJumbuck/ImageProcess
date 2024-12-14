using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using ImageProcess.Utility;
using ImageProcess.View;

namespace ImageProcess.ViewModel;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly Dictionary<ImageFile, CancellationTokenSource> _cancellationTokenSources = new();
    private readonly string _outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");
    private bool _isProcessing;

    // 添加静态字典来存储处理类型和对应的后缀
    private static readonly Dictionary<string, string> ProcessTypeSuffixes = new()
    {
        ["灰度"] = "_gray",
        ["放大至200%"] = "_scale200",
        ["缩小至50%"] = "_scale50",
        ["顺时针旋转90°"] = "_rotate_cw",
        ["逆时针旋转90°"] = "_rotate_ccw",
        ["边缘检测"] = "_edge",
        ["二值化"] = "_binary",
        ["模糊"] = "_blur"
    };

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
        // 修改初始化处理类型列表
        ProcessTypes = new ObservableCollection<string>(ProcessTypeSuffixes.Keys);
        
        // 设置默认选中第一项
        SelectedProcessType = ProcessTypes.FirstOrDefault();
        
        // 确保输出目录存在
        if (!Directory.Exists(_outputDirectory))
        {
            Directory.CreateDirectory(_outputDirectory);
        }
    }

    private bool IsProcessing
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
        IsProcessing = true;
        var mainWindowHandle = WindowsMessage.FindWindow(null, "图像处理");
        var threads = new List<Thread>();
        
        try 
        {
            foreach (var (file, index) in ImageFiles.Select((f, i) => (f, i)))
            {
                var cts = new CancellationTokenSource();
                _cancellationTokenSources[file] = cts;

                var thread = new Thread(async () =>
                {
                    try
                    {
                        string suffix = ProcessTypeSuffixes.GetValueOrDefault(SelectedProcessType ?? "", "_processed");
                        string outputPath = Path.Combine(_outputDirectory, 
                            $"{Path.GetFileNameWithoutExtension(file.FilePath)}{suffix}{Path.GetExtension(file.FilePath)}");

                        // 发送处理中状态
                        WindowsMessage.SendProgressMessage(mainWindowHandle, index, WindowsMessage.ProcessStatus.Processing);

                        bool success = false;
                        try
                        {
                            success = ProcessTypeSuffixes.GetValueOrDefault(SelectedProcessType ?? "", "_processed") switch
                            {
                                "_gray" => await Utility.ImageProcess.ToGrayScale(file.FilePath, outputPath, cts.Token),
                                "_scale200" => await Utility.ImageProcess.Scale200(file.FilePath, outputPath, cts.Token),
                                "_scale50" => await Utility.ImageProcess.Scale50(file.FilePath, outputPath, cts.Token),
                                "_rotate_cw" => await Utility.ImageProcess.RotateClockwise90(file.FilePath, outputPath, cts.Token),
                                "_rotate_ccw" => await Utility.ImageProcess.RotateCounterClockwise90(file.FilePath, outputPath, cts.Token),
                                "_edge" => await Utility.ImageProcess.EdgeDetection(file.FilePath, outputPath, cts.Token),
                                "_binary" => await Utility.ImageProcess.Threshold(file.FilePath, outputPath, cts.Token),
                                "_blur" => await Utility.ImageProcess.Blur(file.FilePath, outputPath, cts.Token),
                                _ => false
                            };

                            WindowsMessage.SendProgressMessage(mainWindowHandle, index, 
                                success ? WindowsMessage.ProcessStatus.Completed : WindowsMessage.ProcessStatus.Failed);
                        }
                        catch (OperationCanceledException)
                        {
                            WindowsMessage.SendProgressMessage(mainWindowHandle, index, WindowsMessage.ProcessStatus.Cancelled);
                        }
                    }
                    finally
                    {
                        _cancellationTokenSources.Remove(file);
                        cts.Dispose();
                    }
                });

                thread.Start();
                threads.Add(thread);
            }

            // 等待所有线程完成
            await Task.Run(() =>
            {
                foreach (var thread in threads)
                {
                    thread.Join();
                }
            });
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
        
        string suffix = ProcessTypeSuffixes.GetValueOrDefault(SelectedProcessType ?? "", "_processed");

        string outputPath = Path.Combine(_outputDirectory, 
            $"{Path.GetFileNameWithoutExtension(SelectedFile.FilePath)}{suffix}{Path.GetExtension(SelectedFile.FilePath)}");
            
        if (File.Exists(outputPath))
        {
            var window = new ShowImagesWindow(SelectedFile.FilePath, outputPath, SelectedProcessType ?? "未知处理");
            window.Show();
        }
    }

    [RelayCommand]
    private void CancelProcess()
    {
        if (SelectedFile != null && _cancellationTokenSources.TryGetValue(SelectedFile, out var cts))
        {
            cts.Cancel();
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