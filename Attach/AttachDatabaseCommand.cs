namespace Alienlab.DMT.Attach
{
  using System;
  using System.Collections.Specialized;
  using System.Data.SqlClient;
  using System.Security.Principal;
  using System.Windows;
  using System.Windows.Input;
  using Alienlab.DMT.Common;
  using Microsoft.SqlServer.Management.Smo;

  public class AttachDatabaseCommand : AbstractCommand, ICommand
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

      var logicalName = viewModel.LogicalName;
      if (string.IsNullOrEmpty(logicalName))
      {
        Helper.HandleError("The database logical name is not provided. ");
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

      var serviceAccount = this.GetServiceAccount(server);
      if (serviceAccount != null)
      {
        this.EnsureFilePermissions(physicalPath, serviceAccount);
      }

      this.AttachDatabase(server, logicalName, physicalPath);
    }

    private void AttachDatabase(Server server, string logicalName, string physicalPath)
    {
      try
      {
        server.AttachDatabase(logicalName, new StringCollection { physicalPath });
        Application.Current.MainWindow.Close();
      }
      catch (Exception ex)
      {
        Helper.HandleError("Cannot attach the database. ", ex);
      }
    }

    private void EnsureFilePermissions(string physicalPath, SecurityIdentifier serviceAccount)
    {
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
            Helper.HandleError("It looks like the database's log file failed to attach, but the most likely the database itself was attached with newly created .LDF file. ", ex);
            Application.Current.MainWindow.Close();
          }
        }

        Helper.HandleError("Cannot ensure security permissions, but will try to attach anyway. ", ex);
      }
    }

    private SecurityIdentifier GetServiceAccount(Server server)
    {
      try
      {
        if (server.Properties.Contains("ServiceAccountSid"))
        {
          var serviceAccountSidProperty = server.Properties["ServiceAccountSid"];
          Assert.IsNotNull(serviceAccountSidProperty, "serviceAccountSidProperty");

          var serviceAccountSidBytes = serviceAccountSidProperty.Value as byte[];
          Assert.IsNotNull(serviceAccountSidBytes, "serviceAccountSidBytes");

          return new SecurityIdentifier(serviceAccountSidBytes, 0);
        }

        if (server.Properties.Contains("ServiceAccount"))
        {
          var servuceAccountProperty = server.Properties["ServiceAccount"];
          Assert.IsNotNull(servuceAccountProperty, "servuceAccountProperty");

          var serviceAccount = servuceAccountProperty.Value as string;
          Assert.IsNotNull(serviceAccount, "serviceAccount");

          switch (serviceAccount)
          {
            case @"NT AUTHORITY\NETWORKSERVICE":
              return new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null);

            case @"NT AUTHORITY\LOCAL SERVICE":
              return new SecurityIdentifier(WellKnownSidType.LocalServiceSid, null);

            case @"NT AUTHORITY\SYSTEM":
              return new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);

            case @"BUILTIN\Administrators":
              return new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
          }
        }

        return null;
      }
      catch (Exception ex)
      {
        Helper.HandleError("Cannot retrieve SQL server metadata. ", ex);
        return null;
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