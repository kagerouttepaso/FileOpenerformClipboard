﻿using FileOpenerformClipboard.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileOpenerformClipboard.Search
{
    public class Searcher
    {
        private static readonly char[] s_icPre = Constants.InvalidCharsWindowsPath
            .Except(Constants.ValidCharsUrlPath)
            .ToArray();

        private static readonly char[] s_icPost = Constants.InvalidCharsWindowsPath
            .Except(Constants.ValidCharsUrlPath)
            .Append('\\')
            .ToArray();

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

                _stringElements = new Lazy<IEnumerable<string>>(() =>
               {
                   // 行両端の無効文字を削除するメソッド
                   // 単純に空白だけでなく、下記のURLのような
                   // 両側にファイルパスに使えない記号で修飾された文字も削除する
                   // __<__URL__>
                   string trimMethod(string str)
                   {
                       var t = str.Trim().TrimStart(s_icPre).TrimEnd(s_icPost);
                       // 除去する文字がなくなるまで再帰する
                       return t.Length == str.Length ?
                                  t :
                                  trimMethod(t);
                   }

                   return _clipboardString
                       //改行コード変更後
                       .Replace("\r\n", "\n")
                       .Replace("\r", "\n")
                       .Split('\n')
                       //各行の装飾を削除
                       .Select(x => trimMethod(x))
                       //空行を削除
                       .Where(x => !string.IsNullOrWhiteSpace(x))
                       .ToArray();
               });
            }
        }

        private string _clipboardString = default(string);

        /// <summary>
        /// いい感じに整形した行コレクション
        /// </summary>
        private IEnumerable<string> StringElements => _stringElements.Value;

        private Lazy<IEnumerable<string>> _stringElements;

        /// <summary>
        /// 検索用の行コレクションのコレクション
        /// </summary>
        private IEnumerable<IEnumerable<string>> SearchStringList
        {
            get
            {
                var indexs = StringElements
                    .Select((row, index) => (index, row))
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
        public double Progress { get => _nowCount == 0 ? 0.0 : (double)_nowCount / CandidatesSize; }

        /// <summary>
        /// 検索済みの候補数
        /// </summary>
        private volatile int _nowCount;

        /// <summary>
        /// 検索ヒット数
        /// </summary>
        public int Hits => _hits;

        private volatile int _hits;

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
        public async Task<string> GetValidPathAsync()
        {
            _hits = 0;
            _nowCount = 0;
            bool isValid(string filename)
            {
                Interlocked.Increment(ref _nowCount);
                if (Constants.InvalidCharsWindowsPath.All(c => !filename.Contains(c))
                   && (Directory.Exists(filename) || File.Exists(filename)))
                {
                    Interlocked.Increment(ref _hits);
                    return true;
                }
                else if (filename.IsUrl())
                {
                    Interlocked.Increment(ref _hits);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return await Task.Run(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                var query = SearchStringList
                    .SelectMany(x => x.FilePathBuilder());
                var result = query
                    // 並列検索
                    .AsParallel()
                    .WithDegreeOfParallelism(64)
                    // 有効なファイルパスまたはURLなものを検索
                    .Where(x => isValid(x))
                    // 最も長いパスを返す
                    .OrderByDescending(x => x.Count())
                    .FirstOrDefault();
                return result;
            })
            .ConfigureAwait(false);
        }
    }
}