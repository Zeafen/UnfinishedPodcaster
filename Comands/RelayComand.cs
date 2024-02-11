using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VlogManager_Client.Comands
{
    public class RelayComand : ModifyCommandBase
    {
        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _execute;

        public RelayComand() { }
        public RelayComand( Action<object> execute, Func<object, bool> canExecute)
        {
            _canExecute = canExecute?? throw new ArgumentNullException(nameof(_canExecute));
            _execute = execute;
        }

        public RelayComand(Action<object> execute) : this(execute, null) { }

        public override bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);

        public override void Execute(object? parameter) => _execute(parameter);
    }
}
