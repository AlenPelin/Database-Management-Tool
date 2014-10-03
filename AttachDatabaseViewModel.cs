namespace Alienlab.DMT
{
  using System;
  using System.Collections.Specialized;
  using System.Dynamic;
  using System.Linq;
  using System.Windows.Input;

  public class AttachDatabaseViewModel
  {
    public DataSourceViewModel DataSourceViewModel { get; set; }

    public ICommand DoAttach { get; set; }

    public string LogicalName { get; set; }

    public string PhysicalPath
    {
      get
      {
        return Environment.GetCommandLineArgs().Skip(1).FirstOrDefault();
      }
    }
  }
}