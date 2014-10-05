namespace Alienlab.DMT.Detach
{
  using System;
  using System.Linq;
  using System.Windows.Input;
  using Alienlab.DMT.Common;

  public class DetachDatabaseViewModel
  {
    private string physicalPath;
    public DataSourceViewModel DataSourceViewModel { get; set; }

    public ICommand DoDetach { get; set; }
    
    public string PhysicalPath
    {
      get
      {
        var paramWords = Environment.GetCommandLineArgs().Skip(2).ToArray();
        if (paramWords.Any())
        {
          return string.Join(" ", paramWords);
        }

        return null;
      }
    }
  }
}