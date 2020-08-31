//system

//my Utils

namespace FileOpenerformClipboard.Helper
{
    public static class Constants
    {
        /// <summary>
        /// Windowsのファイルパスで許されないもの
        /// </summary>
        public static readonly char[] InvalidCharsWindowsPath = new[]
        {
            '/',
            ':',
            '*',
            '?',
            '"',
            '<',
            '>',
            '|'
        };

        public static readonly char[] ValidCharsUrlPath = new[]
        {
            '?'
        };
    }
}