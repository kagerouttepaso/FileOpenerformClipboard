//system
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

//my Utils
using Commons.Extensions;

namespace FileOpenerformClipboard
{
    class Program
	{
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            //クリップボードから取得
            var clipboard_object = Clipboard.GetDataObject();

            //文字列の取得、出来なかったら終了
            var clipboardString = "";
            if (clipboard_object.GetDataPresent(DataFormats.Text))
            {
                clipboardString = (String)clipboard_object.GetData(DataFormats.Text);
            }
            else
                return;

            var clipboardStrings = clipboardString
                //改行コード変更後
                .Replace("\r", "")
                .Split('\n')
                //各行頭の空白文字とおせっかいな行末の\を削除
                .Select(x => Regex.Replace(x.Trim(), @"^(.*?)[\\]?$", "$1"))
                //空行を削除
                .Where(x => x != "")
                .ToList();

            var tryPathList = new List<string>();
            Action<int, string> serch = (c, s) => { };
            serch = (count, name) =>
            {
                if (clipboardStrings.Count() <= count) { return; }
                else if (clipboardStrings.Count() - 1 == count)
                {
                    if (name != "")
                        tryPathList.Add(string.Format("{0}\\{1}", name, clipboardStrings[count]));
                    tryPathList.Add(string.Format("{0}{1}", name, clipboardStrings[count]));
                }
                else
                {
                    if (name != "")
                        serch(count + 1, string.Format("{0}\\{1}", name, clipboardStrings[count]));
                    serch(count + 1, string.Format("{0}{1}", name, clipboardStrings[count]));
                    tryPathList.Add(string.Format("{0}\\{1}", name, clipboardStrings[count]));
                    tryPathList.Add(string.Format("{0}{1}", name, clipboardStrings[count]));
                }
            };
            (clipboardStrings.Count).Takes(i =>
            {
                serch(i, "");
            });


            //作った文字列がフォルダかファイルのパスならば、ファイルオープン
            var filename = tryPathList
                .Where(x => !string.IsNullOrWhiteSpace(x) && x != "\\" && !Regex.IsMatch(x, @"^[\\][\\][\\]"))
                .Where(uri => (Directory.Exists(uri) || File.Exists(uri))).ToList();
            if (filename.Count() > 0)
            {
                System.Diagnostics.Process.Start(filename.OrderByDescending(x => x.Length).First());
            }

            //おまけ 選択された文字列の中にURLがあったら開いてみる
            if (clipboardString.Replace("\r", "").Split('\n').Any(x => Regex.IsMatch(x, @"^[\s]*(http|https|ftp)://.*")))
            {
                var url = clipboardString.Split('\n')
                    .First(x => Regex.IsMatch(x, @"^[\s]*(http|https|ftp)://.*"));
                System.Diagnostics.Process.Start(url);
            }

        }
	}
}