﻿using FileOpenerformClipboard.Helper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileOpenerformClipboard.Search
{
    public class Searcher
    {
        /// <summary>
        /// Windowsのファイルパスで許されないもの
        /// </summary>
        private static readonly IReadOnlyList<char> s_invalidCharsWindowsPath = new List<char>
                {
                    '\\',
                    '/',
                    ':',
                    '*',
                    // '?', URLで使われているので許す
                    '"',
                    '<',
                    '>',
                    '|'
                };

        /// <summary>
        /// くっ付ける行数
        /// 5行より多く分割する人はめったにいないはず
        /// </summary>
        private static readonly int s_concatLineSize = 8;

        /// <summary>
        /// クリップボードから受け取った文字列
        /// </summary>
        private string ClipboardString
        {
            get => _clipboardString;
            set
            {
                if (_clipboardString == value) return;
                _clipboardString = value;
                _canditatesSize = null;
            }
        }

        private string _clipboardString = default(string);

        /// <summary>
        /// いい感じに整形した行コレクション
        /// </summary>
        private IEnumerable<string> StringElements
        {
            get
            {
                var icPre = s_invalidCharsWindowsPath
                    .Where(x => x != '\\') // ネットワーク上のファイルは\\から始まるので除外
                    .ToArray();
                var icPost = s_invalidCharsWindowsPath
                    .ToArray();

                // 行両端の無効文字を削除するメソッド
                // 単純に空白だけでなく、下記のURLのような
                // 両側にファイルパスに使えない記号で修飾された文字も削除する
                // __<__URL__>
                string trimMethod(string str)
                {
                    var t = str.Trim().TrimStart(icPre).TrimEnd(icPost);
                    // 除去する文字がなくなるまで再帰する
                    return t.Length == str.Length ?
                        t :
                        trimMethod(t);
                }

                return ClipboardString
                //改行コード変更後
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Split('\n')
                //各行の装飾を削除
                .Select(x => trimMethod(x))
                //空行を削除
                .Where(x => x != string.Empty);
            }
        }

        /// <summary>
        /// 検索用の行コレクションのコレクション
        /// </summary>
        private IEnumerable<IEnumerable<string>> SearchStringList
        {
            get
            {
                var indexs = StringElements
                    .Select((row, index) => new { index, row })
                    .Where(x => x.row.IsUrl() || x.row.IsFilePath()) // 少なくとも連結する行先頭はフォーマットが正しいはず
                    .Select(x => x.index)
                    .ToArray();

                // フォーマットの正しい行から10行分ずつ文字列を取り出す
                return indexs.Select(i => StringElements.Skip(i).Take(s_concatLineSize));
            }
        }

        /// <summary>
        /// 候補数
        /// </summary>
        public long CandidatesSize
        {
            get
            {
                if (!_canditatesSize.HasValue)
                {
                    _canditatesSize = SearchStringList
                        .SelectMany(x => x.FilePathBuilder())
                        .LongCount();
                }
                return _canditatesSize.Value;
            }
        }

        private long? _canditatesSize = null;

        /// <summary>
        /// 進捗
        /// </summary>
        public double Progress { get => nowCount == 0 ? 0.0 : (double)nowCount / CandidatesSize; }

        /// <summary>
        /// 検索済みの候補数
        /// </summary>
        private volatile int nowCount;

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
        /// <returns>最も文字数の多い有効なパス</returns>
        public Task<string> GetValidPathAsync()
        {
            nowCount = 0;
            return Task.Run(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                var query = SearchStringList
                   .SelectMany(x => x.FilePathBuilder());
                var result = query
                   // 並列検索
                   .AsParallel()
                   .WithDegreeOfParallelism(64)
                   // 最も長いパスを返す
                   .OrderByDescending(x => x.Count())
                   // 有効なファイルパスまたはURLなものを検索
                   .FirstOrDefault(x =>
                   {
                       Interlocked.Increment(ref nowCount);
                       if (Directory.Exists(x) || File.Exists(x))
                       {
                           return true;
                       }
                       else if (x.IsUrl())
                       {
                           return true;
                       }
                       else
                       {
                           return false;
                       }
                   });
                return result;
            });
        }
    }
}