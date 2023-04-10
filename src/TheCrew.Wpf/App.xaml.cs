using System.Threading;
using System.Windows;
using TheCrew.Wpf.Tools;

namespace TheCrew.Wpf;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
   CancellationTokenSource _cts;
   public App()
   {
      _cts = new CancellationTokenSource();
      Startup += App_Startup;
      base.Exit += App_Exit;
   }

   private void App_Exit(object sender, ExitEventArgs e)
   {
      _cts.Cancel();
   }

   private void App_Startup(object sender, StartupEventArgs e)
   {
      MainWindow = new MainWindow()
      {
         DataContext = new MainViewModel(
            new GameInitiator().CreateNewGame(1),
            new CardImageSelector(),
            _cts.Token)
      };
      MainWindow.Show();
   }
}
