using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DrPerfmon.Model
{
    public class ActionCommand : ICommand
    {
        private Action _action;
        private bool _isExecutable;

        public bool IsExecutable
        {
            get { return _isExecutable; }
            set
            {
                _isExecutable = value;
                if (CanExecuteChanged == null)
                {
                    return;
                }
                CanExecuteChanged(this, new EventArgs());

            }
        }

        public ActionCommand(Action action)
        {
            _action = action;
        }

        /// <summary>
        /// Предикат показывает можно ли запускать команды при заданном аргументе.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return IsExecutable;
        }

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Что должна выполнять команда
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _action();
        }
    }
}
