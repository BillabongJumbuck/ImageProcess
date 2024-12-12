﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImageProcess.ViewModel;
using System.Windows.Interop;
using ImageProcess.Utility;

namespace ImageProcess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;
        private IntPtr _windowHandle;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            // 窗口加载完成后获取窗口句柄
            this.Loaded += (s, e) =>
            {
                _windowHandle = new WindowInteropHelper(this).Handle;
                var source = HwndSource.FromHwnd(_windowHandle);
                source?.AddHook(WndProc);
            };
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WindowsMessage.WM_PROGRESS_UPDATE)
            {
                int fileIndex = wParam.ToInt32();
                int status = lParam.ToInt32();

                if (fileIndex >= 0 && fileIndex < _viewModel.ImageFiles.Count)
                {
                    var file = _viewModel.ImageFiles[fileIndex];
                    file.Status = status switch
                    {
                        0 => "[处理中]",
                        1 => "[处理完毕]",
                        2 => "[处理失败]",
                        3 => "[已取消]",
                        _ => file.Status
                    };
                }
                handled = true;
            }
            return IntPtr.Zero;
        }
    }
}