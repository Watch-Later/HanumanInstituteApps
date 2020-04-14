﻿using System;
using YoutubeExplode.Videos.Streams;

namespace HanumanInstitute.DownloadManager {
    public class BestFormatInfo {
        public IVideoStreamInfo BestVideo { get; set; }
        public IAudioStreamInfo BestAudio { get; set; }
        public TimeSpan Duration { get; set; }
        public string StatusText { get; set; }
    }
}
