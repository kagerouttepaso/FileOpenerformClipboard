using System;
using System.Diagnostics;
using System.Threading;
using FileOpenerformClipboard.Helper;
using FileOpenerformClipboard.Search;

namespace FileOpenerformClipboardCui
{
    class Program
    {
        static void Main(string[] args)
        {
            var clipboardString = ClipboardHelper.Instance.GetText();
            if (clipboardString == null) return;

            var Searcher = new Searcher(clipboardString);

            Debug.WriteLine("search {0} file paths",Searcher.CandidatesSize);

            var hitPath = Searcher.GetValidPathAsync();
            while (!hitPath.IsCompleted)
            {
                Debug.WriteLine(Searcher.Progress.ToString("##.0%"));
                Thread.Sleep(500);
            }
            
            //ヒットしていれば開く
            if (hitPath.Result == null) return;
            Process.Start(hitPath.Result);
        }
    }
}