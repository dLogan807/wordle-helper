using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WordleHelper.Commands;

public class RelayCommand(Action<object> executeMethod, Predicate<object> canExecuteMethod)
    : ICommand
{
    public event EventHandler? CanExecuteChanged;

    private Action<object> ExecuteAction { get; set; } = executeMethod;
    private Predicate<object> CanExecutePredicate { get; set; } = canExecuteMethod;

    //Will this command continue invoking?
    public bool CanExecute(object? parameter)
    {
        return CanExecutePredicate(parameter);
    }

    public void Execute(object? parameter)
    {
        ExecuteAction(parameter);
    }
}
