using System;
using System.Linq;
using System.Windows.Input;

namespace TheCrew.Wpf.Tools;

internal class RelayCommand<TParameter> : Executor<TParameter>, ICommand
{
   public RelayCommand(Action<TParameter> execute)
      : base(_ => true, execute) { }

   public RelayCommand(Predicate<TParameter> predicate, Action<TParameter> execute)
      : base(predicate, execute) { }

   public event EventHandler? CanExecuteChanged;

   public bool CanExecute(object? parameter) => parameter is TParameter p && base.CanExecute(p);

   public void Execute(object? parameter)
   {
      if (parameter is TParameter p)
      {
         base.Execute(p);
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

internal class RelayCommand : Executor, ICommand
{
   public RelayCommand(Action execute) : base(() => true, execute)
   {
   }

   public RelayCommand(Func<bool> predicate, Action execute)
      : base(predicate, execute) { }

   public event EventHandler? CanExecuteChanged;

   bool ICommand.CanExecute(object? parameter) => CanExecute();

   void ICommand.Execute(object? parameter) => Execute();

   public void Revalidate()
   {
      App.Current.Dispatcher.Invoke(() =>
      {
         CanExecuteChanged?.Invoke(this, EventArgs.Empty);
      });
   }
}