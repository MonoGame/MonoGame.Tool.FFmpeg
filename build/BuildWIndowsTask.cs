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

        // Since we are using mingw to build, paths need to be in unix format
        // e.g. C:\Users\MonoGame\Desktop\ => /c/Users/MonoGame/Desktop
        absoluteArtifactDir = absoluteArtifactDir.Replace("\\", "/");
        absoluteArtifactDir = $"/{absoluteArtifactDir[0]}{absoluteArtifactDir[2..]}";

        // Generate common build directory path
        var buildDirectory = $"{absoluteArtifactDir}/windows-x86_64";
        context.CreateDirectory(buildDirectory);

        // Create the build settings used by each library build
        var buildSettings = new BuildSettings
        {
            ShellCommand = @"C:\msys64\usr\bin\bash",
            PrefixFlag = buildDirectory,
            HostFlag = "x86_64-w64-mingw32",
            PkgConfigPath = $"{buildDirectory}/lib/pkgconfig",
            Path = "/usr/bin:/mingw64/bin",
            CCFlags = "x86_64-w64-mingw32-gcc",
            CFlags = $"-w -I{buildDirectory}/include",
            CPPFlags = $"-I{buildDirectory}/include",
            LDFlags = $"-L{buildDirectory}/lib --static"
        };

        // Get the configuration flags that will be used for the FFMpeg build
        var ffmpegConfigureFlags = GetFFMpegConfigureFlags(context, "windows-x86_64");

        // Build each library in correct order
        BuildOgg(context, buildSettings);
        BuildVorbis(context, buildSettings);
        BuildLame(context, buildSettings);
        BuildFFMpeg(context, buildSettings, ffmpegConfigureFlags);

        // Move the built binary from the build directory to the artifact directory
        // Note: For some reason, unlike the linux and mac builds, the windows build will not copy the binary using the
        // unix like path created. It fails to find it. Instead, a relative path is used here for windows.
        context.CopyFile($"{context.ArtifactsDir}/windows-x86_64/bin/ffmpeg.exe", $"{context.ArtifactsDir}/ffmpeg.exe");
    }
}
