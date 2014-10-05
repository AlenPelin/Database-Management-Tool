namespace Alienlab.DMT.Common
{
  using System;
  using System.Data.SqlClient;
  using Microsoft.SqlServer.Management.Common;
  using Microsoft.SqlServer.Management.Smo;

  public abstract class AbstractCommand
  {
    public Server Connect(SqlConnectionStringBuilder connectionString)
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
        Helper.HandleError("Cannot connect to the server. ", ex);
        return null;
      }
    }
  }
}