
using System;

namespace TheCrew.Wpf.Tools;

internal class Executor
{
   private Action _execute;
   private Func<bool> _predicate;
   public Executor(Func<bool> predicate, Action execute)
   {
      _predicate = predicate;
      _execute = execute;
   }

   public bool CanExecute()
   {
      return _predicate.Invoke();
   }

   public void Execute()
   {
      _execute.Invoke();
   }
}

internal class Executor<TParameter>
{
   private Action<TParameter> _execute;
   private Predicate<TParameter> _predicate;
   public Executor(Predicate<TParameter> predicate, Action<TParameter> execute)
   {
      _predicate = predicate;
      _execute = execute;
   }

   public bool CanExecute(TParameter parameter)
   {
      return _predicate.Invoke(parameter);
   }

   public void Execute(TParameter parameter)
   {
      _execute.Invoke(parameter);
   }
}
