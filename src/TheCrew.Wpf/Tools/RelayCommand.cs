
using System;
using System.Linq;
using System.Windows.Input;

namespace TheCrew.Wpf.Tools;

internal class RelayCommand<TParameter> : ICommand
{
   private Action<TParameter> _execute;
   private Predicate<TParameter> _predicate;

   public RelayCommand(Action<TParameter> execute) : this(_ => true, execute)
   {
   }

   public RelayCommand(Predicate<TParameter> predicate, Action<TParameter> execute)
   {
      _predicate = predicate;
      _execute = execute;
   }

   public event EventHandler? CanExecuteChanged;

   public bool CanExecute(object? parameter)
   {
      return parameter is TParameter p && _predicate.Invoke(p);
   }

   public void Execute(object? parameter)
   {
      if (parameter is TParameter p)
      {
         _execute.Invoke(p);
      }
   }

   public void Revalidate()
   {
      App.Current.Dispatcher.Invoke(() =>
      {
         CanExecuteChanged?.Invoke(this, EventArgs.Empty);
      });
   }
}

internal class RelayCommand : ICommand
{
   private Action _execute;
   private Func<bool> _predicate;

   public RelayCommand(Action execute) : this(() => true, execute)
   {
   }

   public RelayCommand(Func<bool> predicate, Action execute)
   {
      _predicate = predicate;
      _execute = execute;
   }

   public event EventHandler? CanExecuteChanged;

   public bool CanExecute(object? parameter)
   {
      return _predicate.Invoke();
   }

   public void Execute(object? parameter)
   {
      _execute.Invoke();
   }
}