namespace Alienlab.DMT
{
  using System.Windows.Input;

  public class AttachDatabaseViewModel
  {
    public DataSourceViewModel DataSourceViewModel { get; set; }

    public ICommand DoAttach { get; set; }

    public string LogicalName { get; set; }
  }
}