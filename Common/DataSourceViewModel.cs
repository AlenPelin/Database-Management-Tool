namespace Alienlab.DMT.Common
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.IO;
  using System.Linq;

  public class DataSourceViewModel : INotifyPropertyChanged
  {
    [NotNull]
    protected readonly string FilePath = GetFilePath();

    [NotNull]
    private readonly IList<string> connectionStrings;

    private int selectedIndex;

    [CanBeNull]
    private string text;

    public DataSourceViewModel()
    {
      var collection = this.LoadConnectionStrings();
      Assert.IsNotNull(collection, "collection");

      collection.Add("<New>");

      this.connectionStrings = collection;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public virtual int SelectedIndex
    {
      get
      {
        return this.selectedIndex;
      }
      set
      {
        if (value >= 0)
        {
          this.selectedIndex = value;
        }
      }
    }

    [NotNull]
    public virtual IList<string> ConnectionStrings
    {
      get
      {
        return this.connectionStrings;
      }
    }

    protected virtual void SaveConnectionStrings()
    {
      File.WriteAllLines(this.FilePath, this.ConnectionStrings.Take(this.ConnectionStrings.Count - 1));
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([NotNull] string propertyName)
    {
      Assert.ArgumentNotNull(propertyName, "propertyName");

      var handler = this.PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    [CanBeNull]
    public virtual string Text
    {
      get
      {
        return this.text;
      }
      set
      {
        if (string.IsNullOrEmpty(value))
        {
          if (this.SelectedIndex == this.ConnectionStrings.Count - 1)
          {
            // user tries to delete the <New> item, we should not allow doing that
            this.text = "<New>";
            return;
          }

          this.ConnectionStrings.RemoveAt(this.SelectedIndex);
          this.SelectedIndex -= 1;
          this.text = this.connectionStrings[this.SelectedIndex];
        }
        else
        {
          if (this.SelectedIndex == this.ConnectionStrings.Count - 1)
          {
            if (value.Equals("<New>", StringComparison.OrdinalIgnoreCase))
            {
              this.text = value;
              return;
            }

            var index = this.ConnectionStrings.Count - 1;
            this.ConnectionStrings.Insert(index >= 0 ? index : 0, value);
          }
          else
          {
            if (this.ConnectionStrings[this.SelectedIndex].Equals(value, StringComparison.OrdinalIgnoreCase))
            {
              this.text = value;
              return;
            }

            this.ConnectionStrings[this.SelectedIndex] = value;
          }

          this.text = value;
        }

        this.SaveConnectionStrings();
        this.OnPropertyChanged("Text");
      }
    }

    protected ObservableCollection<string> LoadConnectionStrings()
    {
      if (File.Exists(this.FilePath))
      {
        return new ObservableCollection<string>(File.ReadAllLines(this.FilePath));
      }

      return new ObservableCollection<string>();
    }

    private static string GetFilePath()
    {
      var path = Environment.ExpandEnvironmentVariables("%APPDATA%\\Alienlab\\Database Management Tool\\DataSources.config");
      var dir = Path.GetDirectoryName(path);
      if (!Directory.Exists(dir))
      {
        Directory.CreateDirectory(dir);
      }

      return path;
    }
  }
}