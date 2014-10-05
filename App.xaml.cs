namespace Alienlab.DMT
{
  using System.Linq;
  using System.Windows;
  using System.Windows.Media;
  using Alienlab.DMT.Attach;
  using Alienlab.DMT.Common;
  using Alienlab.DMT.Detach;

  public enum DatabaseAction
  {
    Attach,
    Detach,
    Exit
  }

  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App
  {
    /// <summary>
    /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      var args = e.Args;
      var mode = ParseMode(args.FirstOrDefault());
      switch (mode)
      {
        case DatabaseAction.Attach:
          var attachWindow = new AttachWindow();
          attachWindow.ShowDialog();
          return;
        case DatabaseAction.Detach:
          if (!args.Skip(1).Any())
          {
            MessageBox.Show("The database file path is not specified.", "Database Management Tool", MessageBoxButton.OK, MessageBoxImage.Error);
            CloseApplication();
            return;
          }

          var detachDatabaseWindow = new DetachWindow();
          detachDatabaseWindow.ShowDialog();
          return;
        default:
          CloseApplication();
          return;
      }
    }

    private static void CloseApplication()
    {
      var window = new Window
      {
        WindowStyle = WindowStyle.None,
        Background = Brushes.Transparent,
        Left = 9999
      };

      window.Show();
      window.Close();
    }

    private DatabaseAction ParseMode(string text)
    {
      switch (text)
      {
        case "-attach":
          return DatabaseAction.Attach;
        case "-detach":
          return DatabaseAction.Detach;
        default:
          Helper.HandleError("The application expects two command-line parameters:\n1. Action (-attach or -detach)\n2. Database file path (.mdf)");
          return DatabaseAction.Exit;
      }
    }
  }
}
