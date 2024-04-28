using System.Runtime.InteropServices;

namespace BuildScripts;

public abstract class BuildTaskBase : FrostingTask<BuildContext>
{
    protected static void BuildOgg(BuildContext context, BuildSettings buildSettings)
    {
        var processSettings = new ProcessSettings()
        {
            WorkingDirectory = "./ogg",
            EnvironmentVariables = buildSettings.GetEnvironmentVariables()
        };

        var commandExecutor = new CommandExecutionHelper(context, processSettings, buildSettings);

        // Ensure clean start if we're running locally and testing over and over
        commandExecutor.ExecuteCommand("make distclean");

        // Run autogen.sh to create configuration files
        commandExecutor.ExecuteCommand("./autogen.sh");

        // Run configure to build make file
        commandExecutor.ExecuteCommand($"./configure --prefix=\"{buildSettings.PrefixFlag}\" --host=\"{buildSettings.HostFlag}\" --disable-shared");

        // Run make
        commandExecutor.ExecuteCommand($"make -j{Environment.ProcessorCount}");

        // Run make install
        commandExecutor.ExecuteCommand("make install");
    }

    protected static void BuildVorbis(BuildContext context, BuildSettings buildSettings)
    {
        var processSettings = new ProcessSettings()
        {
            WorkingDirectory = "./vorbis",
            EnvironmentVariables = buildSettings.GetEnvironmentVariables()
        };

        var commandExecutor = new CommandExecutionHelper(context, processSettings, buildSettings);

        // Ensure clean start if we're running locally and testing over and over
        commandExecutor.ExecuteCommand("make distclean");

        // Run autogen.sh to create configuration files
        commandExecutor.ExecuteCommand("./autogen.sh");

        // Run configure to build make file
        commandExecutor.ExecuteCommand($"./configure --prefix=\"{buildSettings.PrefixFlag}\" --host=\"{buildSettings.HostFlag}\" --disable-examples --disable-docs --disable-shared");

        // Run make
        commandExecutor.ExecuteCommand($"make -j{Environment.ProcessorCount}");

        // Run make install
        commandExecutor.ExecuteCommand("make install");
    }

    protected static void BuildLame(BuildContext context, BuildSettings buildSettings)
    {
        var processSettings = new ProcessSettings()
        {
            WorkingDirectory = "./lame",
            EnvironmentVariables = buildSettings.GetEnvironmentVariables()
        };

        var commandExecutor = new CommandExecutionHelper(context, processSettings, buildSettings);

        // Ensure clean start if we're running locally and testing over and over
        commandExecutor.ExecuteCommand("make distclean");

        // Run configure to build make file
        commandExecutor.ExecuteCommand($"./configure --prefix='{buildSettings.PrefixFlag}' --host=\"{buildSettings.HostFlag}\" --disable-frontend --disable-decoder --disable-shared");

        // Run make
        commandExecutor.ExecuteCommand($"make -j{Environment.ProcessorCount}");

        // Run make install
        commandExecutor.ExecuteCommand("make install");
    }

    protected static void BuildFFMpeg(BuildContext context, BuildSettings buildSettings, string configureFlags)
    {
        var processSettings = new ProcessSettings()
        {
            WorkingDirectory = "./ffmpeg",
            EnvironmentVariables = buildSettings.GetEnvironmentVariables()
        };

        var commandExecutor = new CommandExecutionHelper(context, processSettings, buildSettings);

        // Ensure clean start if we're running locally and testing over and over
        commandExecutor.ExecuteCommand("make distclean");

        // Run configure to build make file
        commandExecutor.ExecuteCommand($"./configure --prefix=\"{buildSettings.PrefixFlag}\" {configureFlags}");

        // Run make
        commandExecutor.ExecuteCommand($"make -j{Environment.ProcessorCount}");

        // Run make install
        commandExecutor.ExecuteCommand("make install");
    }

    protected static string GetFullPathToArtifactDirectory(BuildContext context)
    {
        string fullPath = System.IO.Path.GetFullPath(context.ArtifactsDir);

        if (context.IsRunningOnWindows())
        {
            // Windows uses mingw for compilation and expects paths to be in unix format
            // e.g. C:\Users\MonoGame\Desktop\ => /c/Users/MonoGame/Desktop
            fullPath = fullPath.Replace("\\", "/");
            fullPath = $"/{fullPath[0]}{fullPath[2..]}";
        }

        return fullPath;
    }

    protected static string GetFFMpegConfigureFlags(BuildContext context, string rid)
    {
        var ignoreCommentsAndNewLines = (string line) => !line.StartsWith('#') && !line.StartsWith(' ');
        var configureFlags = context.FileReadLines("ffmpeg.config").Where(ignoreCommentsAndNewLines);
        var osConfigureFlags = context.FileReadLines($"ffmpeg.{rid}.config").Where(ignoreCommentsAndNewLines);
        return string.Join(' ', configureFlags) + " " + string.Join(' ', osConfigureFlags);
    }
}
