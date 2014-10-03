namespace Alienlab.DMT
{
  using System;
  using System.Collections.Specialized;
  using System.Data.SqlClient;
  using System.Windows;
  using System.Windows.Input;
  using Microsoft.SqlServer.Management.Common;
  using Microsoft.SqlServer.Management.Smo;

  public class AttachDatabaseCommand : ICommand
  {
    /// <summary>
    /// Defines the method to be called when the command is invoked.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
    public void Execute(object parameter)
    {
      var viewModel = (AttachDatabaseViewModel)parameter;
      var connectionString = GetConnectionString(viewModel);
      if (connectionString == null)
      {
        return;
      }

      if (string.IsNullOrEmpty(viewModel.LogicalName))
      {
        MessageBox.Show("The logical name is not provided", "Database Management Tool");
        return;
      }

      var connection = new ServerConnection(connectionString.DataSource, connectionString.UserID, connectionString.Password)
      {
        ConnectTimeout = 1        
      };

      var server = new Server(connection);
      try
      {
        server.AttachDatabase(viewModel.LogicalName, new StringCollection { viewModel.PhysicalPath });
      }
      catch (Exception ex)
      {
        MessageBox.Show("An error ocurred. " + PrintError(ex), "Database Management Tool", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private static SqlConnectionStringBuilder GetConnectionString(AttachDatabaseViewModel viewModel)
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
      return exception.Message + Environment.NewLine + (exception.InnerException != null ? "Inner exception: " + this.PrintError(exception.InnerException) : string.Empty);
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