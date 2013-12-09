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

			//選択された文字列を\をつけて結合。
			var url = String.Join("\\",
				//改行コード変更後
						str.Replace("\r", "")
				//各行頭の空白文字とおせっかいな行末の\を削除
						.Split('\n')
						.Select(s => Regex.Replace(s.Trim(), @"^(.*?)[\\]?$", "$1"))
						.ToArray()
					);
			//作った文字列がフォルダかファイルのパスならば、ファイルオープン
			if(Directory.Exists(url) || File.Exists(url)) {
#if DEBUG

				Console.WriteLine( str );
				Console.ReadKey();
#else
				System.Diagnostics.Process.Start(url);
#endif
			}


			//おまけ 選択された文字列の中にURLがあったら開いてみる
			if(str.Replace("\r", "").Split('\n').Any(x => Regex.IsMatch(x, @"^[\s]*(http|https|ftp)://.*"))) {
				url = str.Split('\n')
					.First(x => Regex.IsMatch(x, @"^[\s]*(http|https|ftp)://.*"));
				System.Diagnostics.Process.Start(url);
			}

		}
	}
}