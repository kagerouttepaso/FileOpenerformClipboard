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
        {
            return Regex.IsMatch(_this, @"^[\s]*(http|https|ftp)://.*");
        }

        /// <summary>
        /// ファイルパスを再帰的に構築
        /// </summary>
        /// <param name="istrings"></param>
        /// <returns></returns>
        public static IEnumerable<string> FilePathBuilder(this IEnumerable<string> istrings)
        {
            var nowString = istrings.FirstOrDefault();
            if (nowString == null) yield break;

            foreach (var filePath in JointTailStrings(istrings, new string[]{ "", "\\" }))
            {
                yield return filePath;
            }
            foreach (var next in FilePathBuilder(istrings.Skip(1)))
            {
                yield return next;
            }
        }

        /// <summary>
        /// 後ろの文字列を再帰的に繋げる
        /// </summary>
        /// <param name="stringList"></param>
        /// <returns></returns>
        public static IEnumerable<string> JointTailStrings(this IEnumerable<string> stringList, IEnumerable<string> separators)
        {
            var nowLineString = stringList.FirstOrDefault();
            if (nowLineString == null) yield break;

            yield return nowLineString;
            foreach (var tailString in JointTailStrings(stringList.Skip(1),separators))
            {
                foreach(var separator in separators)
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