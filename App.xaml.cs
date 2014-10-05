namespace Alienlab.DMT
{
  using System.Linq;
  using System.Windows;
  using System.Windows.Input;

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
          var mainWindow = new AttachWindow();
          mainWindow.ShowDialog();
          return;
        case DatabaseAction.Detach:
          var detachDatabaseWindow = new DetachWindow();
          detachDatabaseWindow.ShowDialog();
          return;
        default:
          mainWindow = new AttachWindow();
          mainWindow.Show();
          mainWindow.Close();
          return;
      }
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
          // TODO: Replace this with native Windows select dialolg with two options
          var result = MessageBox.Show("Would you like to attach the database? Select No to detach.", "Database Management Tool", MessageBoxButton.YesNoCancel);
          return result == MessageBoxResult.Yes ? DatabaseAction.Attach : result == MessageBoxResult.No ? DatabaseAction.Detach : DatabaseAction.Exit;
      }
    }
  }

  public enum DatabaseAction
  {
    Attach,
    Detach,
    Exit
  }
}
