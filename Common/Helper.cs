namespace Alienlab.DMT.Common
{
  using System;
  using System.Windows;

  public static class Helper
  {
    public static void HandleError(string message, Exception ex = null)
    {
      MessageBox.Show(message + PrintError(ex), "Database Management Tool", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private static string PrintError(Exception exception)
    {
      return exception == null ? String.Empty : exception.Message + Environment.NewLine + (exception.InnerException != null ? "Inner exception: " + PrintError(exception.InnerException) : String.Empty);
    }
  }
}