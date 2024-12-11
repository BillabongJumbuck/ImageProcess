﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;

namespace ImageProcess.ViewModel;

public partial class MainWindowViewModel : ObservableObject
{
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

    [RelayCommand]
    private void RemoveSelected()
    {
        if (SelectedFile != null)
        {
            ImageFiles.Remove(SelectedFile);
        }
    }

    [RelayCommand]
    private void StartProcess()
    {
        // TODO: 实现多线程处理逻辑
    }

    [RelayCommand]
    private void CancelProcess()
    {
        // TODO: 实现取消处理逻辑
    }

    [RelayCommand]
    private void ViewResult()
    {
        // TODO: 实现查看结果逻辑
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