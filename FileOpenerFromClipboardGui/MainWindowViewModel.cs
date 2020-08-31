using FileOpenerformClipboard.Helper;
using FileOpenerformClipboard.Search;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FileOpenerFromClipboardGui
{
    /// <summary>
    /// ViewModel
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 進捗
        /// </summary>
        public double Progress
        {
            get => _progress;
            private set
            {
                if (_progress == value) { return; }
                _progress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress)));
            }
        }

        private double _progress;

        public MainWindowViewModel()
        {
            Progress = 0;
        }

        /// <summary>
        /// 検索開始
        /// </summary>
        /// <returns></returns>
        public async Task RunAsync()
        {
            Progress = 0;

            //文字列読み取り
            var clipboardString = ClipboardHelper.Instance.GetText();
            if (clipboardString == null) return;

            // 検索クラス生成
            var Searcher = new Searcher(clipboardString);

            Trace.WriteLine($"search {Searcher.CandidatesSize} file paths");

            //検索開始
            var hitPath = Searcher.GetValidPathAsync();

            // 検索中のプログレスバー更新
            await Task.Run(async () =>
            {
                var waitTime = TimeSpan.FromSeconds(1.0 / 60);
                while (!hitPath.IsCompleted)
                {
                    Trace.WriteLine($"{Searcher.Progress:#0.0%} {Searcher.Hits}");
                    Progress = Searcher.Progress * 100;
                    await Task.Delay(waitTime) // = 60fps
                        .ConfigureAwait(false);
                }
                Progress = Searcher.Progress * 100;
            })
            .ConfigureAwait(false);

            //ヒットしていれば開く
            var result = await hitPath;
            if (result == null) return;
            Trace.WriteLine($"Open {result}");
            Process.Start(result);
        }
    }
}