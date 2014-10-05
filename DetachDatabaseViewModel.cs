namespace Alienlab.DMT
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Windows.Input;
  using Microsoft.Win32;

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