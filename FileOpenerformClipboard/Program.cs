using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FileOpenerformClipboard.Helper;

namespace FileOpenerformClipboard
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //クリップボードから取得
            var clipboardObject = Clipboard.GetDataObject();
            

            //文字列の取得、出来なかったら終了
            if (!clipboardObject.GetDataPresent(DataFormats.Text)) return;
            var clipboardString = clipboardObject.GetData(DataFormats.Text) as string;

            var clipboardStrings = clipboardString
                //改行コード変更後
                .Replace("\r", "")
                .Split('\n')
                //各行頭の空白文字とおせっかいな行末の\を削除
                .Select(x => Regex.Replace(x.Trim(), @"^(.*?)[\\]?$", "$1"))
                //空行を削除
                .Where(x => x != "");

            System.Diagnostics.Debug.WriteLine("{0} file paths.",
                clipboardStrings.FilePathBuilder().Count());

            var hitPath = clipboardStrings
                // ファイルパスを再帰的に構築
                .FilePathBuilder()
                // あたりは1つであることを想定しているため並列検索
                .AsParallel()
                // ファイルパスまたはURLなものを検索
                .FirstOrDefault(x => 
                    Directory.Exists(x) ||
                    File.Exists(x) ||
                    x.IsUrl());

            //ヒットしていれば開く
            if (hitPath == null) return;
            System.Diagnostics.Process.Start(hitPath);
        }
    }
}