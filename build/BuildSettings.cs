namespace BuildScripts;

/// <summary>
/// Provides settings to be used when building libraries.
/// </summary>
public class BuildSettings
{
    /// <summary>
    /// Gets or Sets the command used to start a shell process to execute shell commands. If the command is not part of 
    /// $PATH then full path to the shell executable should be provided.
    /// </summary>
    public string ShellCommand { get; set; } = string.Empty;

    /// <summary>
    /// Gets or Sets the value to use for the CFLAGS environment variable.
    /// </summary>
    public string CFlags { get; set; } = string.Empty;

    /// <summary>
    /// Gets or Sets the value to use for the CCFLAGS environment variable.
    /// </summary>
    public string CCFlags { get; set; } = string.Empty;

    /// <summary>
    /// Gets or Sets the value to use for the CXXFLAGS environment variable.
    /// </summary>
    public string CXXFlags { get; set; } = string.Empty;

    /// <summary>
    /// Gets or Sets the value to use for the CPPFLAGS environment variable.
    /// </summary>
    public string CPPFlags { get; set; } = string.Empty;

    /// <summary>
    /// Gets or Sets the value to use for the LDFLAGS environment variable.
    /// </summary>
    public string LDFlags { get; set; } = string.Empty;

    /// <summary>
    /// Gets or Sets additional values to add to the existing $PATH variable.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or Sets the value to use for the --prefix flag when calling the configure scripts.
    /// </summary>
    public string PrefixFlag { get; set; } = string.Empty;

    /// <summary>
    /// Gets or Sets the value to use for hte --host flag when calling the configure scripts.
    /// </summary>
    public string HostFlag { get; set; } = string.Empty;

    /// <summary>
    /// Gets or Sts the value to use for the PKG_CONFIG_PATH environment variable.
    /// </summary>
    public string PkgConfigPath { get; set; } = string.Empty;

    /// <summary>
    /// Returns a key-value pair dictionary of the environment variables to set for a build.
    /// </summary>
    public IDictionary<string, string> GetEnvironmentVariables()
    {
        var environmentVariables = new Dictionary<string, string>();

        // Only add those that have a value otherwise it could cause issues for builds.
        if (!string.IsNullOrEmpty(CFlags))
            environmentVariables.Add("CFLAGS", CFlags);

        if (!string.IsNullOrEmpty(CCFlags))
            environmentVariables.Add("CCFLAGS", CCFlags);

        if (!string.IsNullOrEmpty(CXXFlags))
            environmentVariables.Add("CXXFLAGS", CXXFlags);

        if (!string.IsNullOrEmpty(CPPFlags))
            environmentVariables.Add("CPPFLAGS", CPPFlags);

        if (!string.IsNullOrEmpty(LDFlags))
            environmentVariables.Add("LDFLAGS", LDFlags);

        return environmentVariables;
    }
}
