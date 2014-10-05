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

        return this.physicalPath ?? (this.physicalPath = this.ChooseDatabaseFile());
      }
    }

    private string ChooseDatabaseFile()
    {
      var openFileDialog = new OpenFileDialog
      {
        AddExtension = true,
        ReadOnlyChecked = true,
        ShowReadOnly = true,
        Filter = "MS SQL Server Database File (*.MDF)|*.mdf",
        InitialDirectory = Path.GetPathRoot(Environment.ExpandEnvironmentVariables(Environment.SystemDirectory)),
        Multiselect = false,
        Title = "Choose MS SQL Server Database .MDF File"
      };

      var result = openFileDialog.ShowDialog();
      
      return result.HasValue && result.Value ? openFileDialog.FileName : null;
    }
  }
}