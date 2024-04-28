namespace BuildScripts;

[TaskName("Build Linux")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildToolTask))]
public sealed class BuildLinuxTask : BuildTaskBase
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnLinux();

    public override void Run(BuildContext context)
    {
        // Absolute path to the artifact directory is needed for flags since they don't allow relative path
        var absoluteArtifactDir = context.MakeAbsolute(new DirectoryPath(context.ArtifactsDir));

        // Generate common build directory path
        var buildDirectory = $"{absoluteArtifactDir}/linux-x86_64";

        // Create the build settings used by each library build
        var buildSettings = new BuildSettings
        {
            ShellCommand = "sh",
            PrefixFlag = buildDirectory,
            PkgConfigPath = $"{buildDirectory}/lib/pkgconfig",
            HostFlag = "x86_64-linux-gnu",
            CFlags = $"-w -I{buildDirectory}/include",
            CPPFlags = $"-I{buildDirectory}/include",
            LDFlags = $"-L{buildDirectory}/lib --static"
        };

        // Get the configuration flags that will be used for the FFMpeg build
        var ffmpegConfigureFlags = GetFFMpegConfigureFlags(context, "linux-x86_64");

        // Build each library in correct order
        BuildOgg(context, buildSettings);
        BuildVorbis(context, buildSettings);
        BuildLame(context, buildSettings);
        BuildFFMpeg(context, buildSettings, ffmpegConfigureFlags);

        // Move the built binary from the build directory to the artifact directory
        context.MoveFile($"{buildDirectory}/bin/ffmpeg", $"{absoluteArtifactDir}/ffmpeg");
    }
}
