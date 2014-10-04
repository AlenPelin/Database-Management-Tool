namespace Alienlab.DMT
{
  using System;
  using System.Collections.ObjectModel;
  using Alienlab.DMT.Properties;

  public class Assert
  {    
    // ReSharper disable once CodeAnnotationAnalyzer
    public static void ArgumentNotNull([CanBeNull] object value, [NotNull] string name)
    {
      if (value == null)
      {
        throw new ArgumentNullException(name);
      }
    }

    public static void IsNotNull(object value, string message)
    {
      if (value == null)
      {
        throw new InvalidOperationException(message);
      }
    }

    public static void IsTrue(bool condition, string message)
    {
      if (!condition)
      {
        throw new InvalidOperationException(message);
      }
    }
  }
}