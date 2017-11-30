using System;
using System.Threading.Tasks;
using System.Windows;

namespace FileOpenerFromClipboardGui
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindowView : Window
    {
        private MainWindowViewModel vm;
        private Task searchTask;

        public MainWindowView()
        {
            vm = new MainWindowViewModel();
            DataContext = vm;
            InitializeComponent();
            // 検索開始
            searchTask = vm.RunAsync();
        }

        /// <summary>
        /// Window表示完了後処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_ContentRenderedAsync(object sender, EventArgs e)
        {
            // 検索完了を待って自動的にWindowクローズ
            await searchTask;
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}