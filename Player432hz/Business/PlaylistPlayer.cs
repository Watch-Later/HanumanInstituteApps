﻿using System;
using System.Collections.Generic;
using System.Linq;
using HanumanInstitute.Common.Services;
using ReactiveUI;

namespace HanumanInstitute.Player432hz.Business;

/// <summary>
/// Manages the playback of a list of media files.
/// </summary>
public class PlaylistPlayer : ReactiveObject, IPlaylistPlayer
{
    private readonly IFileSystemService _fileSystem;

    public PlaylistPlayer(IFileSystemService fileSystem)
    {
        _fileSystem = fileSystem;
    }

    /// <summary>
    /// Gets the list of files currently playing.
    /// </summary>
    public IList<string> Files { get; } = new List<string>();

    /// <summary>
    /// Gets the path of the file currently playing.
    /// </summary>
    public string NowPlaying
    {
        get => _nowPlaying;
        set => this.RaiseAndSetIfChanged(ref _nowPlaying, value, nameof(NowPlaying));
    }
    private string _nowPlaying = string.Empty;

    /// <summary>
    /// Gets the display title of the file currently playing.
    /// </summary>
    public string NowPlayingTitle
    {
        get => _nowPlayingTitle;
        set => this.RaiseAndSetIfChanged(ref _nowPlayingTitle, value, nameof(NowPlayingTitle));
    }
    private string _nowPlayingTitle = string.Empty;

    private readonly Random _random = new Random();

    /// <summary>
    /// Starts the playback of specified list of media files.
    /// </summary>
    /// <param name="list">The list of file paths to play.</param>
    /// <param name="current">If specified, playback will start with specified file.</param>
    public void Play(IEnumerable<string>? list, string? current)
    {
        Files.Clear();
        if (list != null)
        {
            Files.AddRange(list);
        }
        NowPlaying = string.Empty;
        NowPlaying = current ?? string.Empty;
        SetTitle();
        if (string.IsNullOrEmpty(current))
        {
            PlayNext();
        }
    }

    /// <summary>
    /// Starts playing the next media file from the list.
    /// </summary>
    public void PlayNext()
    {
        if (Files.Any())
        {
            var pos = _random.Next(Files.Count);
            if (Files[pos] == NowPlaying)
            {
                pos = _random.Next(Files.Count);
            }
            NowPlaying = string.Empty;
            NowPlaying = Files[pos];
            SetTitle();
        }
    }

    private void SetTitle()
    {
        NowPlayingTitle = !string.IsNullOrEmpty(NowPlaying) ? _fileSystem.Path.GetFileName(NowPlaying) : string.Empty;
    }
}
