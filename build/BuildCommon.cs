namespace BuildScripts;

static class BuildCommon
{
    // There is an issue with FFmpeg that causes incompatible reduced quality audio (MGCB medium & low settings)
    // to be produced for XAudio2 playback. For reference a ticket has been opened at https://trac.ffmpeg.org/ticket/9397#ticket
    // (samples per block should be a power of 2, however FFmpeg incorrectly forces this requirement on the block size
    // instead). For now, we'll just patch out the encoder's power of 2 check.
    public static void FFmpegApplyXAudio2Fix(BuildContext context)
    {
        string filePath = "./ffmpeg/libavcodec/adpcmenc.c";
        List<string> lines = context.FileReadLines(filePath).ToList();
        int lineNumber = lines.FindIndex(x => x.Contains("av_log(avctx, AV_LOG_ERROR, \"block size must be power of 2\\n\");"));
        if (lineNumber != -1)
        {
            lines.RemoveRange(lineNumber, 2);
            context.FileWriteLines(filePath, lines.ToArray());
        }
    }
}
