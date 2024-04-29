namespace BuildScripts;

[TaskName("Build Windows")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildToolTask))]
public sealed class BuildWindowsTask : BuildTaskBase
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnWindows();

    public override void Run(BuildContext context)
    {
        // Absolute path to the artifact directory is needed for flags since they don't allow relative path
        var absoluteArtifactDir = context.MakeAbsolute(new DirectoryPath(context.ArtifactsDir)).ToString();
        var cFlagsExport = "export CFLAGS=\"-w\";";
        var ccFlagsExport = "export CCFLAGS=\"x86_64-w64-mingw32-gcc\";";
        var ldFlagsExport = "export LDFLAGS=\"--static\";";
        var pathExport = "export PATH=\"/usr/bin:/mingw64/bin:$PATH\";";
        var pkgConfigExport = "export PKG_CONFIG_PATH=\"/mingw64/lib/pkgconfig:$PKG_CONFIG_PATH\";";
        var exports = $"{pathExport}{cFlagsExport}{ccFlagsExport}{ldFlagsExport}{pkgConfigExport}";

        var configureFlags = GetFFMpegConfigureFlags(context, "windows-x64");
        var processSettings = new ProcessSettings() { WorkingDirectory = "./ffmpeg" };
        var shellCommandPath = @"C:\msys64\usr\bin\bash.exe";

        // Ensure clean start if we're running locally and testing over and over
        if (context.BuildSystem().IsLocalBuild)
        {
            processSettings.Arguments = $"-c \"{exports} make distclean\"";
            context.StartProcess(shellCommandPath, processSettings);
        }

        // Run configure to build make file
        processSettings.Arguments = $"-c \"{exports} ./configure --bindir={absoluteArtifactDir} {configureFlags}\"";
        context.StartProcess(shellCommandPath, processSettings);

        // Run make
        processSettings.Arguments = $"-c \"{exports} make -j{Environment.ProcessorCount}\"";
        context.StartProcess(shellCommandPath, processSettings);

        // Run make install
        processSettings.Arguments = $"-c \"{exports} make install\"";
        context.StartProcess(shellCommandPath, processSettings);
    }
}
