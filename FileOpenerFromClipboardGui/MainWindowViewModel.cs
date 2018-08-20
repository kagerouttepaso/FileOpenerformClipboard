using FileOpenerformClipboard.Helper;
using FileOpenerformClipboard.Search;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileOpenerFromClipboardGui
{
    /// <summary>
    /// ViewModel
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// キャンセルコマンド(未実装
        /// </summary>
        public ICommand CancelCommand { get; private set; }

        /// <summary>
        /// 進捗
        /// </summary>
        public double Progress { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            CancelCommand = new GenericCommand(
                canExecute: () => true,
                execute: () => { }
            );
            Progress = 0;
        }

        /// <summary>
        /// 検索開始
        /// </summary>
        /// <returns></returns>
        public async Task RunAsync()
        {
            //文字列読み取り
            var clipboardString = ClipboardHelper.Instance.GetText();
            if(clipboardString == null) return;

            // 検索クラス生成
            var Searcher = new Searcher(clipboardString);

            Debug.WriteLine("search {0} file paths", Searcher.CandidatesSize);

            //検索開始
            var hitPath = Searcher.GetValidPathAsync();

            // 検索中のプログレスバー更新
            await Task.Run(async () =>
            {
                var waitTime = TimeSpan.FromSeconds(1.0 / 60);
                while(!hitPath.IsCompleted)
                {
                    Debug.WriteLine(Searcher.Progress.ToString("##.0%"));
                    Progress = Searcher.Progress * 100;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress)));
                    await Task.Delay(waitTime); // = 60fps
                }
            });

            //ヒットしていれば開く
            var result = await hitPath;
            if(result == null) return;
            Process.Start(result);
        }
    }

    /// <summary>
    /// 適当コマンド
    /// </summary>
    public class GenericCommand : ICommand
    {
        private Func<bool> canExecute;
        private Action execute;

        public event EventHandler CanExecuteChanged;

        public GenericCommand(Func<bool> canExecute, Action execute)
        {
            this.canExecute = canExecute;
            this.execute = execute;
        }

        public bool CanExecute(object parameter) => canExecute();

        public void Execute(object parameter) => execute();
    }
}