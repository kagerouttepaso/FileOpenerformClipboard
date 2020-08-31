//system
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

//my Utils

namespace FileOpenerformClipboard.Helper
{
    public static class Extentions
    {
        /// <summary>
        /// URLかどうかチェック
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static bool IsUrl(this string _this)
            => IsUrlRegex.IsMatch(_this);

        private static Regex IsUrlRegex { get; } = new Regex(@"\As?https?://[-_.!~*'()a-zA-Z0-9;/?:@&=+$,%#]+\z", RegexOptions.Compiled);

        public static bool IsFilePath(this string _this) => IsFilePathRegexs.Any(r => r.IsMatch(_this));

        private static Regex[] IsFilePathRegexs { get; } = new Regex[]
        {
            new Regex(@"^[a-z]:", RegexOptions.Compiled|RegexOptions.IgnoreCase), // ドライブレター
            new Regex(@"^\\\\",RegexOptions.Compiled), // ネットワークフォルダ
        };

        /// <summary>
        /// ファイルパスを再帰的に構築
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static IEnumerable<string> FilePathBuilder(this IEnumerable<string> strings)
        {
            var nowString = strings.FirstOrDefault();
            if (nowString == null) yield break;

            foreach (var filePath in strings.JointTailStrings(JoinSeparators))
            {
                yield return filePath;
            }
        }

        /// <summary>
        /// ファイルパス構築時につなぎに使うセパレータ
        /// </summary>
        public static IList<string> JoinSeparators { get; } = new string[] { "", "\\", " " };

        /// <summary>
        /// 後ろの文字列を再帰的に繋げる
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static IEnumerable<string> JointTailStrings(this IEnumerable<string> strings, IEnumerable<string> separators)
        {
            var nowLineString = strings.FirstOrDefault();
            if (nowLineString == null) yield break;

            yield return nowLineString;
            foreach (var tailString in strings.Skip(1).JointTailStrings(separators))
            {
                foreach (var separator in separators)
                {
                    yield return string.Format("{0}{1}{2}",
                        nowLineString,
                        separator,
                        tailString);
                }
            }
        }
    }
}