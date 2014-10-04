namespace Alienlab.DMT
{
  using System;
  using System.Collections.Specialized;
  using System.Data.SqlClient;
  using System.Security.Principal;
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

      try
      {
        var physicalPath = viewModel.PhysicalPath;
        Assert.IsNotNull(physicalPath, string.Empty);

        try
        {
          var server = new Server(connection);

          // test connection
          Assert.IsNotNull(server.Databases.Count, string.Empty);

          try
          {
            var serviceAccount = GetServiceAccount(server);
            Assert.IsNotNull(serviceAccount, "serviceAccount");

            try
            {
              SecurityHelper.EnsureFilePermissions(physicalPath, serviceAccount);
            }
            catch (Exception ex)
            {
              var inner = ex.InnerException;
              if (inner != null)
              {
                var inner2 = inner.InnerException;
                if (inner2 != null && inner2.Message.ToLowerInvariant().Contains(".ldf\""))
                {
                  this.HandleError("It looks like the database's log file failed to attach, but the most likely the database itself was attached with newly created .LDF file. ", ex);
                  Application.Current.MainWindow.Close();
                }
              }

              this.HandleError("Cannot ensure security permissions, but will try to attach anyway. ", ex);
            }

            try
            {
              server.AttachDatabase(viewModel.LogicalName, new StringCollection { physicalPath });
              Application.Current.MainWindow.Close();
            }
            catch (Exception ex)
            {
              this.HandleError("Cannot attach the database. ", ex);
            }
          }
          catch (Exception ex)
          {
            this.HandleError("Cannot retrieve SQL server metadata. ", ex);
          }
        }
        catch (Exception ex)
        {
          this.HandleError("Cannot connect to the server. ", ex);
        }
      }
      catch (Exception ex)
      {
        this.HandleError("The database file path is not specified. ", ex);
      }
    }

    private void HandleError(string message, Exception ex)
    {
      MessageBox.Show(message + this.PrintError(ex), "Database Management Tool", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private static SecurityIdentifier GetServiceAccount(Server server)
    {
      var serviceAccountSidProperty = server.Properties["ServiceAccountSid"];
      Assert.IsNotNull(serviceAccountSidProperty, "The serviceAccountSid property is null");

      var serviceAccountSidBytes = serviceAccountSidProperty.Value as byte[];
      Assert.IsNotNull(serviceAccountSidBytes, "serviceAccountSidBytes");

      var serviceAccount = new SecurityIdentifier(serviceAccountSidBytes, 0);

      return serviceAccount;
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