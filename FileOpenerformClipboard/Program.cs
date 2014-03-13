using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;

using System.Threading;

using System.Text.RegularExpressions;
using System.IO;

namespace FileOpenerformClipboard
{
	class Program
	{
		[STAThreadAttribute]
		static void Main(string[] args) {
			//クリップボードから取得
			var clipboard_object = Clipboard.GetDataObject();
			//文字列の取得、出来なかったら終了
			var str = "";
			if(clipboard_object.GetDataPresent(DataFormats.Text)) {
				str = (String)clipboard_object.GetData(DataFormats.Text);
			} else
				return;

            var lines = str
                //改行コード変更後
                .Replace("\r", "")
                .Split('\n')
                //各行頭の空白文字とおせっかいな行末の\を削除
                .Select(x => Regex.Replace(x.Trim(), @"^(.*?)[\\]?$", "$1"))
                .ToList();
            Action<int,string> serch = (c,s) => { };
            var list = new List<string>();
            serch = (count, name) =>
            {
                if (lines.Count() <= count) { return; }
                else if (lines.Count() - 1 == count)
                {
                    list.Add(string.Format("{0}\\{1}", name, lines[count]));
                    list.Add(string.Format("{0}{1}", name, lines[count]));
                }
                else
                {
                    if (count != 0) serch(count + 1, string.Format("{0}\\{1}", name, lines[count]));
                    serch(count + 1, string.Format("{0}{1}", name, lines[count]));
                    list.Add(string.Format("{0}\\{1}", name, lines[count]));
                    list.Add(string.Format("{0}{1}", name, lines[count]));

                }
            };
            serch(0, "");


			//作った文字列がフォルダかファイルのパスならば、ファイルオープン
            if (list.Any(uri => (Directory.Exists(uri) || File.Exists(uri))))
            {
                var filename = list.First(uri => (Directory.Exists(uri) || File.Exists(uri)));
                System.Diagnostics.Process.Start(filename);
            }

			//おまけ 選択された文字列の中にURLがあったら開いてみる
			if(str.Replace("\r", "").Split('\n').Any(x => Regex.IsMatch(x, @"^[\s]*(http|https|ftp)://.*"))) {
				var url = str.Split('\n')
					.First(x => Regex.IsMatch(x, @"^[\s]*(http|https|ftp)://.*"));
				System.Diagnostics.Process.Start(url);
			}

		}
	}
}