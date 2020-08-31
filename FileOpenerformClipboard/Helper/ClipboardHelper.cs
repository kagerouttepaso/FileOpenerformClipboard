using System.Threading;
using System.Windows.Forms;

namespace FileOpenerformClipboard.Helper
{
    public class ClipboardHelper
    {
        public string GetText()
        {
            string clipboardString = default(string);
            var t = new Thread(() =>
            {
                //クリップボードから取得
                var clipboardObject = Clipboard.GetDataObject();

                //文字列の取得、出来なかったら終了
                if (!clipboardObject.GetDataPresent(DataFormats.Text)) return;
                clipboardString = clipboardObject.GetData(DataFormats.Text) as string;
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
            return clipboardString;
        }

        public static ClipboardHelper Instance { get; } = new ClipboardHelper();

        private ClipboardHelper()
        { }
    }
}