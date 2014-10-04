namespace Alienlab.DMT
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Security.AccessControl;
  using System.Security.Principal;
  using Alienlab.DMT.Properties;

  public static class SecurityHelper
  {
    private static readonly Type SecurityIdentifier = typeof(SecurityIdentifier);

    private static readonly IdentityReference Everyone = new SecurityIdentifier("S-1-1-0").Translate(typeof(NTAccount));

    public static bool CompareTo(this IdentityReference left, IdentityReference right)
    {
      return left != null && right != null ? left.Translate(SecurityIdentifier).ToString().Equals(right.Translate(SecurityIdentifier).ToString(), StringComparison.OrdinalIgnoreCase) : left == right;
    }

    /// <summary>
    /// The get rules.
    /// </summary>
    /// <param name="rules">
    /// The rules. 
    /// </param>
    /// <param name="identity">
    /// The identity. 
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    private static IEnumerable<AuthorizationRule> GetRules([NotNull] AuthorizationRuleCollection rules, [NotNull] IdentityReference identity)
    {
      Assert.ArgumentNotNull(rules, "rules");
      Assert.ArgumentNotNull(identity, "identity");

      try
      {
        return rules.Cast<AuthorizationRule>().Where(rule => rule.IdentityReference.CompareTo(identity) || rule.IdentityReference.CompareTo(Everyone));
      }
      catch
      {
        return new AuthorizationRule[0];
      }
    }

    private static bool HasDirectoryPermissions(string path, IdentityReference identity, FileSystemRights permissions)
    {
      DirectoryInfo dirInfo = new DirectoryInfo(path);
      DirectorySecurity dirSecurity = dirInfo.GetAccessControl(AccessControlSections.All);
      AuthorizationRuleCollection rules = dirSecurity.GetAccessRules(true, true, typeof(NTAccount));

      return HasPermissions(rules, identity, permissions);
    }

    private static bool HasFilePermissions(string path, IdentityReference identity, FileSystemRights permissions)
    {
      var dirInfo = new FileInfo(path);
      var dirSecurity = dirInfo.GetAccessControl(AccessControlSections.All);
      AuthorizationRuleCollection rules = dirSecurity.GetAccessRules(true, true, typeof(NTAccount));

      return HasPermissions(rules, identity, permissions);
    }

    /// <summary>
    /// The has permissions.
    /// </summary>
    /// <param name="rules">
    ///   The rules. 
    /// </param>
    /// <param name="identity">
    ///   The identity. 
    /// </param>
    /// <param name="permissions"></param>
    /// <returns>
    /// The has permissions. 
    /// </returns>
    private static bool HasPermissions([NotNull] AuthorizationRuleCollection rules, [NotNull] IdentityReference identity, FileSystemRights permissions)
    {
      Assert.ArgumentNotNull(rules, "rules");
      Assert.ArgumentNotNull(identity, "identity");
      try
      {
        return GetRules(rules, identity).Any(rule => (((FileSystemAccessRule)rule).FileSystemRights & permissions) > 0);
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    /// <summary>
    /// The get identity reference.
    /// </summary>
    /// <param name="name">
    /// The name. 
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IdentityReference GetIdentityReference([NotNull] string name)
    {
      Assert.ArgumentNotNull(name, "name");

      return new SecurityIdentifier(name).Translate(typeof(NTAccount));
    }

    /// <summary>
    /// The ensure permissions.
    /// </summary>
    /// <param name="path">
    /// The path. 
    /// </param>
    /// <param name="identity">
    /// The identity. 
    /// </param>
    private static void EnsureDirectoryPermissions([NotNull] string path, [NotNull] IdentityReference identity)
    {
      Assert.ArgumentNotNull(path, "path");
      Assert.ArgumentNotNull(identity, "identity");

      DirectoryInfo dirInfo = new DirectoryInfo(path);
      DirectorySecurity dirSecurity = dirInfo.GetAccessControl(AccessControlSections.All);
      AuthorizationRuleCollection rules = dirSecurity.GetAccessRules(true, true, typeof(NTAccount));

      if (!HasPermissions(rules, identity, FileSystemRights.FullControl))
      {
        var rule = new FileSystemAccessRule(identity, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow);
        dirSecurity.AddAccessRule(rule);
        dirInfo.SetAccessControl(dirSecurity);

        dirSecurity = dirInfo.GetAccessControl(AccessControlSections.All);
        rules = dirSecurity.GetAccessRules(true, true, typeof(NTAccount));
        Assert.IsTrue(HasPermissions(rules, identity, FileSystemRights.FullControl), "The Full Control access to the '" + path + "' folder isn't permitted for " + identity.Value + ". Please fix it and then restart the process");
      }
    }

    /// <summary>
    /// The ensure permissions.
    /// </summary>
    /// <param name="path">
    /// The path. 
    /// </param>
    /// <param name="identity">
    /// The identity. 
    /// </param>
    public static void EnsureFilePermissions([NotNull] string path, [NotNull] IdentityReference identity)
    {
      Assert.ArgumentNotNull(path, "path");
      Assert.ArgumentNotNull(identity, "identity");

      var fileInfo = new FileInfo(path);
      var dirSecurity = fileInfo.GetAccessControl(AccessControlSections.All);
      AuthorizationRuleCollection rules = dirSecurity.GetAccessRules(true, true, typeof(NTAccount));

      if (!HasPermissions(rules, identity, FileSystemRights.FullControl))
      {
        var rule = new FileSystemAccessRule(identity, FileSystemRights.FullControl, AccessControlType.Allow);
        dirSecurity.AddAccessRule(rule);
        fileInfo.SetAccessControl(dirSecurity);

        dirSecurity = fileInfo.GetAccessControl(AccessControlSections.All);
        rules = dirSecurity.GetAccessRules(true, true, typeof(NTAccount));

        Assert.IsTrue(HasPermissions(rules, identity, FileSystemRights.FullControl), "The Full Control access to the '" + path + "' file isn't permitted for " + identity.Value + ". Please fix it and then restart the process");
      }
    }

    public static bool HasPermissions(string path, string identity, FileSystemRights permissions)
    {
      Assert.ArgumentNotNull(path, "path");
      Assert.ArgumentNotNull(identity, "identity");
      Assert.ArgumentNotNull(permissions, "permissions");

      if (Directory.Exists(path))
      {
        return HasDirectoryPermissions(path, GetIdentityReference(identity), permissions);
      }

      if (File.Exists(path))
      {
        return HasFilePermissions(path, GetIdentityReference(identity), permissions);
      }

      throw new InvalidOperationException("File or directory not found: " + path);
    }
  }
}