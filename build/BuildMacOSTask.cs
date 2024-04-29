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
        var buildX64 = context.IsUniversalBinary || RuntimeInformation.ProcessArchitecture is not Architecture.Arm or Architecture.Arm64;
        var buildArm64 = context.IsUniversalBinary || RuntimeInformation.ProcessArchitecture is Architecture.Arm or Architecture.Arm64;

        if (buildX64)
            BuildX64(context);

        if(buildArm64)
            BuildArm64(context);

        
        


        // // Generate common build directory paths for each architectures build artifacts
        // var x866BuildDirectory = $"{absoluteArtifactDir}/osx-x86_64";
        // var arm64BuildDirectory = $"{absoluteArtifactDir}/osx-arm64";

        // if (buildX64)
        // {
        //     // Create the build settings used by each library build
        //     var buildSettings = new BuildSettings()
        //     {
        //         ShellCommand = "zsh",
        //         PrefixFlag = x866BuildDirectory,
        //         PkgConfigPath = $"{x866BuildDirectory}/lib/pkgconfig",
        //         HostFlag = "x86_64-apple-darwin",
        //         CFlags = $"-w -arch x86_64 -I{x866BuildDirectory}/include",
        //         CPPFlags = $"-arch x86_64 -I{x866BuildDirectory}/include",
        //         CXXFlags = "-arch x86_84",
        //         LDFlags = $"-arch x86_64 -L{x866BuildDirectory}/lib"
        //     };

        //     // Get the configuration flags that will be used for the FFMpeg build
        //     var x8664FFMpegConfigureFlags = GetFFMpegConfigureFlags(context, "osx-x86_64");

        //     // Build each library in correct order
        //     BuildOgg(context, buildSettings);
        //     BuildVorbis(context, buildSettings);
        //     BuildLame(context, buildSettings);
        //     BuildFFMpeg(context, buildSettings, x8664FFMpegConfigureFlags);
        // }

        // if (buildArm64)
        // {
        //     // Create the build settings used by each library build
        //     var buildSettings = new BuildSettings()
        //     {
        //         ShellCommand = "zsh",
        //         PrefixFlag = arm64BuildDirectory,
        //         PkgConfigPath = $"{arm64BuildDirectory}/lib/pkgconfig",
        //         HostFlag = "aarch64-apple-darwin",
        //         CFlags = $"-w -arch arm64 -I{arm64BuildDirectory}/include",
        //         CPPFlags = $"-arch arm64 -I{arm64BuildDirectory}/include",
        //         CXXFlags = "-arch arm64",
        //         LDFlags = $"-arch arm64 -L{arm64BuildDirectory}/lib"
        //     };

        //     // Get the configuration flags that will be used for the FFMpeg build
        //     var arm64FFMpegConfigureFlags = GetFFMpegConfigureFlags(context, "osx-arm64");

        //     BuildOgg(context, buildSettings);
        //     BuildVorbis(context, buildSettings);
        //     BuildLame(context, buildSettings);
        //     BuildFFMpeg(context, buildSettings, arm64FFMpegConfigureFlags);
        // }

        // // Move the build binary from the build directory to the artifact directory.
        // // If this is a universal build, we'll need to combine both binaries using lipo and output the result of that.
        // if (buildX64 && buildArm64)
        // {
        //     context.StartProcess("lipo", new ProcessSettings()
        //     {
        //         Arguments = $"-create {x866BuildDirectory}/bin/ffmpeg {arm64BuildDirectory}/bin/ffmpeg -output {absoluteArtifactDir}/ffmpeg"
        //     });
        // }
        // else if (buildX64)
        // {
        //     context.CopyFile($"{x866BuildDirectory}/bin/ffmpeg", $"{absoluteArtifactDir}/ffmpeg");
        // }
        // else
        // {
        //     context.CopyFile($"{arm64BuildDirectory}/bin/ffmpeg", $"{absoluteArtifactDir}/ffmpeg");
        // }
    }

    private static void BuildX64(BuildContext context)
    {
        // Absolute path to the artifact directory is needed for flags since they don't allow relative path
        var absoluteArtifactDir = context.MakeAbsolute(new DirectoryPath(context.ArtifactsDir));
        var cFlagsExport = "export CFLAGS=\"-w -arch x86_64\";";
        var ccFlagsExport = "export CCFLAGS=\"-arch x86_64\";";
        var ldFlagsExport = "export LDFLAGS=\"-arch x86_64\";";
        var pkgConfigExport = "";
        var exports = $"{cFlagsExport}{ccFlagsExport}{ldFlagsExport}{pkgConfigExport}";

        var configureFlags = GetFFMpegConfigureFlags(context, "windows-x64");
        var processSettings = new ProcessSettings() { WorkingDirectory = "./ffmpeg" };
        var shellCommandPath = "zsh";

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

    private static void BuildArm64(BuildContext context)
    {
        // Absolute path to the artifact directory is needed for flags since they don't allow relative path
        var absoluteArtifactDir = context.MakeAbsolute(new DirectoryPath(context.ArtifactsDir));
        var cFlagsExport = "export CFLAGS=\"-w -arch arm64\";";
        var ccFlagsExport = "export CCFLAGS=\"-arch arm64\";";
        var ldFlagsExport = "export LDFLAGS=\"-arch arm64\";";
        var pkgConfigExport = "";
        var exports = $"{cFlagsExport}{ccFlagsExport}{ldFlagsExport}{pkgConfigExport}";

        var configureFlags = GetFFMpegConfigureFlags(context, "windows-x64");
        var processSettings = new ProcessSettings() { WorkingDirectory = "./ffmpeg" };
        var shellCommandPath = "zsh";

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
