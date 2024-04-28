namespace BuildScripts;

public class CommandExecutionHelper
{
    private readonly ProcessSettings _processSettings;
    private readonly BuildContext _buildContext;
    private readonly BuildSettings _buildSettings;

    public CommandExecutionHelper(BuildContext buildContext, ProcessSettings processSettings, BuildSettings buildSettings)
    {
        _buildContext = buildContext;
        _processSettings = processSettings;
        _buildSettings = buildSettings;
    }

    public void ExecuteCommand(string command)
    {
        var pathEnv = string.IsNullOrEmpty(_buildSettings.Path) ?
                      string.Empty :
                    $"export PATH={_buildSettings.Path}:$PATH;";

        var pkgConfigEnv = string.IsNullOrEmpty(_buildSettings.PkgConfigPath) && !command.Contains("./configure") ?
                           string.Empty :
                           $"export PKG_CONFIG_PATH={_buildSettings.PkgConfigPath}:$PKG_CONFIG_PATH;";

        var processArgs = $"-c \"{pathEnv} {pkgConfigEnv} {command}\"";

        _processSettings.Arguments = processArgs;
        _buildContext.StartProcess(_buildSettings.ShellCommand, _processSettings);
    }
}