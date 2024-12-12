using System.Text;
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
using System.Runtime.InteropServices;

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
            if (msg == WindowsMessage.WM_COPYDATA)
            {
                var cds = (WindowsMessage.COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(WindowsMessage.COPYDATASTRUCT));
                
                int fileIndex = cds.dwData.ToInt32();
                if (int.TryParse(cds.lpData, out int statusValue))
                {
                    var status = (WindowsMessage.ProcessStatus)statusValue;
                    if (fileIndex >= 0 && fileIndex < _viewModel.ImageFiles.Count)
                    {
                        var file = _viewModel.ImageFiles[fileIndex];
                        file.Status = status switch
                        {
                            WindowsMessage.ProcessStatus.Processing => "[处理中]",
                            WindowsMessage.ProcessStatus.Completed => "[处理完毕]",
                            WindowsMessage.ProcessStatus.Failed => "[处理失败]",
                            WindowsMessage.ProcessStatus.Cancelled => "[已取消]",
                            _ => file.Status
                        };
                    }
                }
                handled = true;
            }
            return IntPtr.Zero;
        }
    }
}