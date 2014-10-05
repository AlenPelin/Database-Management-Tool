namespace Alienlab.DMT
{
  using System;
  using System.Data.SqlClient;
  using System.Linq;
  using System.Windows;
  using System.Windows.Input;
  using Microsoft.SqlServer.Management.Common;
  using Microsoft.SqlServer.Management.Smo;

  public class DetachDatabaseCommand : ICommand
  {
    /// <summary>
    /// Defines the method to be called when the command is invoked.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
    public void Execute(object parameter)
    {
      var viewModel = (DetachDatabaseViewModel)parameter;
      var connectionString = GetConnectionString(viewModel);
      if (connectionString == null)
      {
        return;
      }

      var physicalPath = viewModel.PhysicalPath;
      if (string.IsNullOrEmpty(physicalPath))
      {
        this.HandleError("The database file path is not specified. ");
        return;
      }

      var server = this.Connect(connectionString);
      if (server == null)
      {
        return;
      }

      var logicalName = this.GetLogicalName(server, physicalPath);
      if (string.IsNullOrEmpty(logicalName))
      {
        return;
      }

      this.DetachDatabase(server, logicalName);
    }

    private string GetLogicalName(Server server, string physicalPath)
    {
      try
      {
        var databases = server.Databases;
        Assert.IsNotNull(databases, "databases");

        foreach (Database database in databases)
        {
          if (database.FileGroups[0].Files[0].FileName.Equals(physicalPath, StringComparison.OrdinalIgnoreCase))
          {
            return database.Name;
          }
        }

        this.HandleError("The database is not attached to the specified SQL server. ");
        return null;
      }
      catch (Exception ex)
      {
        this.HandleError("The error occurred while looking for the database in the specified SQL server. ", ex);
        return null;
      }
    }

    private void DetachDatabase(Server server, string logicalName)
    {
      try
      {
        server.DetachDatabase(logicalName, false);
        Application.Current.MainWindow.Close();
      }
      catch (Exception ex)
      {
        this.HandleError("Cannot detach the database. ", ex);
      }
    }

    private Server Connect(SqlConnectionStringBuilder connectionString)
    {
      var connection = new ServerConnection(connectionString.DataSource, connectionString.UserID, connectionString.Password) { ConnectTimeout = 1 };

      var server = new Server(connection);
      try
      {
        // test connection
        Assert.IsNotNull(server.Databases.Count, string.Empty);
        return server;
      }
      catch (Exception ex)
      {
        this.HandleError("Cannot connect to the server. ", ex);
        return null;
      }
    }

    private void HandleError(string message, Exception ex = null)
    {
      MessageBox.Show(message + this.PrintError(ex), "Database Management Tool", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private static SqlConnectionStringBuilder GetConnectionString(DetachDatabaseViewModel viewModel)
    {
      try
      {
        return new SqlConnectionStringBuilder(viewModel.DataSourceViewModel.Text);
      }
      catch
      {
        MessageBox.Show("The provided connection string does not seem to be valid. ", "Database Management Tool");
        return null;
      }
    }

    private string PrintError(Exception exception)
    {
      return exception == null ? string.Empty : exception.Message + Environment.NewLine + (exception.InnerException != null ? "Inner exception: " + this.PrintError(exception.InnerException) : string.Empty);
    }

    /// <summary>
    /// Defines the method that determines whether the command can execute in its current state.
    /// </summary>
    /// <returns>
    /// true if this command can be executed; otherwise, false.
    /// </returns>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
    public bool CanExecute(object parameter)
    {
      return true;
    }

    public event EventHandler CanExecuteChanged;
  }
}