using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCrew.Model;
using TheCrew.Player.AI;
using TheCrew.Wpf.Factories;

namespace TheCrew.Wpf;
internal class Services : IDisposable, IServiceProvider
{
   readonly ServiceProvider _sp;

   public Services()
   {
      var sc = new ServiceCollection();

      // Models
      sc.AddSingleton<GameModel>();

      sc.AddSingleton<ICardImageSelector, CardImageSelector>();

      sc.AddTransient<MainViewModel>();

      // Factories
      sc.AddSingleton<PlayerViewModelFactory>();
      sc.AddSingleton<CardViewModelFactory>();

      _sp = sc.BuildServiceProvider();
   }

   public void Dispose()
   {
      _sp.Dispose();
   }

   public object? GetService(Type serviceType)
   {
      return _sp.GetService(serviceType);
   }
}
