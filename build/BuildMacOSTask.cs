using System.Runtime.InteropServices;

namespace BuildScripts;

[TaskName("Build macOS")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildToolTask))]
public sealed class BuildMacOSTask : BuildTaskBase
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnMacOs();

    public override void Run(BuildContext context)
    {
        // Determine which mac architecture(s) to build for.
        var buildx8664 = context.IsUniversalBinary || RuntimeInformation.ProcessArchitecture is not Architecture.Arm or Architecture.Arm64;
        var buildArm64 = context.IsUniversalBinary || RuntimeInformation.ProcessArchitecture is Architecture.Arm or Architecture.Arm64;

        // Absolute path to the artifact directory is needed for flags since they don't allow relative path
        var absoluteArtifactDir = context.MakeAbsolute(new DirectoryPath(context.ArtifactsDir));

        // Generate common build directory paths for each architectures build artifacts
        var x866BuildDirectory = $"{absoluteArtifactDir}/osx-x86_64";
        var arm64BuildDirectory = $"{absoluteArtifactDir}/osx-arm64";

        if (buildx8664)
        {
            // Create the build settings used by each library build
            var buildSettings = new BuildSettings()
            {
                ShellCommand = "zsh",
                PrefixFlag = x866BuildDirectory,
                PkgConfigPath = $"{x866BuildDirectory}/lib/pkgconfig",
                HostFlag = "x86_64-apple-darwin",
                CFlags = $"-w -arch x86_64 -I{x866BuildDirectory}/include",
                CPPFlags = $"-arch x86_64 -I{x866BuildDirectory}/include",
                CXXFlags = "-arch x86_84",
                LDFlags = $"-arch x86_64 -L{x866BuildDirectory}/lib"
            };

            // Get the configuration flags that will be used for the FFMpeg build
            var x8664FFMpegConfigureFlags = GetFFMpegConfigureFlags(context, "osx-x86_64");

            // Build each library in correct order
            BuildOgg(context, buildSettings);
            BuildVorbis(context, buildSettings);
            BuildLame(context, buildSettings);
            BuildFFMpeg(context, buildSettings, x8664FFMpegConfigureFlags);
        }

        if (buildArm64)
        {
            // Create the build settings used by each library build
            var buildSettings = new BuildSettings()
            {
                ShellCommand = "zsh",
                PrefixFlag = arm64BuildDirectory,
                PkgConfigPath = $"{arm64BuildDirectory}/lib/pkgconfig",
                HostFlag = "aarch64-apple-darwin",
                CFlags = $"-w -arch arm64 -I{arm64BuildDirectory}/include",
                CPPFlags = $"-arch arm64 -I{arm64BuildDirectory}/include",
                CXXFlags = "-arch arm64",
                LDFlags = $"-arch arm64 -L{arm64BuildDirectory}/lib"
            };

            // Get the configuration flags that will be used for the FFMpeg build
            var arm64FFMpegConfigureFlags = GetFFMpegConfigureFlags(context, "osx-arm64");

            BuildOgg(context, buildSettings);
            BuildVorbis(context, buildSettings);
            BuildLame(context, buildSettings);
            BuildFFMpeg(context, buildSettings, arm64FFMpegConfigureFlags);
        }

        // Move the build binary from the build directory to the artifact directory.
        // If this is a universal build, we'll need to combine both binaries using lipo and output the result of that.
        if (buildx8664 && buildArm64)
        {
            context.StartProcess("lipo", new ProcessSettings()
            {
                Arguments = $"-create {x866BuildDirectory}/bin/ffmpeg {arm64BuildDirectory}/bin/ffmpeg -output {absoluteArtifactDir}/ffmpeg"
            });
        }
        else if (buildx8664)
        {
            context.CopyFile($"{x866BuildDirectory}/bin/ffmpeg", $"{absoluteArtifactDir}/ffmpeg");
        }
        else
        {
            context.CopyFile($"{arm64BuildDirectory}/bin/ffmpeg", $"{absoluteArtifactDir}/ffmpeg");
        }
    }
}
