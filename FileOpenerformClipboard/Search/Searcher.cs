using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FileOpenerformClipboard.Helper;

namespace FileOpenerformClipboard.Search
{
    public class Searcher
    {
        /// <summary>
        /// クリップボードから受け取った文字列
        /// </summary>
        private string ClipboardString { get; set; }

        /// <summary>
        /// 候補リスト
        /// </summary>
        private IEnumerable<string> Candidates
        {
            get
            {
                return ClipboardString
                //改行コード変更後
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Split('\n')
                //各行頭の空白文字とおせっかいな行末の\を削除
                .Select(x => Regex.Replace(x.Trim(), @"^(.*?)[\\]?$", "$1"))
                //空行を削除
                .Where(x => x != "")
                //候補リスト構築
                .FilePathBuilder();
            }
        }

        /// <summary>
        /// 候補数
        /// </summary>
        public int CandidatesSize
        {
            get
            {
                if (_canditatesSize == -1) _canditatesSize = Candidates.Count();
                return _canditatesSize;
            }
        }
        private int _canditatesSize = -1;

        /// <summary>
        /// 進捗
        /// </summary>
        public double Progress { get => nowCount == 0 ? 0.0 : (double)nowCount / CandidatesSize; }

        /// <summary>
        /// 検索済みの候補数
        /// </summary>
        private int nowCount;

        /// <summary>
        /// 実行中フラグ
        /// </summary>
        public bool IsProsess
        {
            get => _isProsess == 1;
            set => Interlocked.Exchange(ref _isProsess, value ? 1 : 0);
        }
        private int _isProsess;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="clipboardString"></param>
        public Searcher(string clipboardString)
        {
            this.ClipboardString = clipboardString;
        }

        /// <summary>
        /// 有効なパス検索
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetValidPathAsync()
        {
            IsProsess = true;
            nowCount = 0;
            return await Task.Factory.StartNew(() =>
             {
                 Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                 var result = Candidates
                    // あたりは1つであることを想定しているため並列検索
                    .AsParallel()
                    // ファイルパスまたはURLなものを検索
                    .FirstOrDefault(x =>
                    {
                        Interlocked.Increment(ref nowCount);
                        return Directory.Exists(x) ||
                        File.Exists(x) ||
                        x.IsUrl();
                    });
                 IsProsess = false;
                 return result;
             });
        }
    }
}