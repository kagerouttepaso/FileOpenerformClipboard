using System;
using System.Windows.Input;

namespace FileOpenerFromClipboardGui
{
    /// <summary>
    /// 適当コマンド
    /// </summary>
    public class GenericCommand : ICommand
    {
        private Func<bool> canExecute;
        private Action execute;

        public event EventHandler CanExecuteChanged;

        public GenericCommand(Func<bool> canExecute, Action execute)
        {
            this.canExecute = canExecute;
            this.execute = execute;
        }

        public bool CanExecute(object parameter) => canExecute();

        public void Execute(object parameter) => execute();
    }
}