using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Windows;
using TheCrew.Wpf.Tools;

namespace TheCrew.Wpf;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly CancellationTokenSource _cts;
    private readonly Services _services;

   public App()
   {
      _cts = new CancellationTokenSource();
      _services = new Services();
      
      Startup += App_Startup;
      base.Exit += App_Exit;
   }

   private void App_Exit(object sender, ExitEventArgs e)
   {
      _cts.Cancel();
   }

   private void App_Startup(object sender, StartupEventArgs e)
   {
      MainViewModel viewModel = _services.GetRequiredService<MainViewModel>();
      viewModel.InitNewGame(1);
      viewModel.StartEngine(_cts.Token);

      MainWindow = new MainWindow()
      {
         DataContext = viewModel
      };
      MainWindow.Show();
   }
}
