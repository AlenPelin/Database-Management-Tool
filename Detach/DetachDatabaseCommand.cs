namespace Alienlab.DMT.Detach
{
  using System;
  using System.Data.SqlClient;
  using System.Windows;
  using System.Windows.Input;
  using Alienlab.DMT.Common;
  using Microsoft.SqlServer.Management.Smo;

  public class DetachDatabaseCommand : AbstractCommand, ICommand
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
        Helper.HandleError("The database file path is not specified. ");
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

        Helper.HandleError("The database is not attached to the specified SQL server. ");
        return null;
      }
      catch (Exception ex)
      {
        Helper.HandleError("The error occurred while looking for the database in the specified SQL server. ", ex);
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
        Helper.HandleError("Cannot detach the database. ", ex);
      }
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