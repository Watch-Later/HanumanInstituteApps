﻿namespace HanumanInstitute.BassAudio;

/// <summary>
/// Provides audio pitch-detection. The data is cached for 1 day.
/// </summary>
public interface IPitchDetector
{
    /// <summary>
    /// Measures the pitch of an audio file, between 424hz and 448hz. Most music is between 440-442hz.
    /// </summary>
    /// <param name="filePath">The path of the file to measure.</param>
    /// <param name="useCache">True to cache the data, false to calculate without cache.</param>
    /// <returns>The audio pitch between 424 and 448.</returns>
    Task<float> GetPitchAsync(string filePath, bool useCache = true);
    /// <summary>
    /// Measures the pitch of an audio file, between 424hz and 448hz. Most music is between 440-442hz.
    /// </summary>
    /// <param name="filePath">The path of the file to measure.</param>
    /// <param name="useCache">True to cache the data, false to calculate without cache.</param>
    /// <returns>The audio pitch between 424 and 448.</returns>
    float GetPitch(string filePath, bool useCache = true);
    /// <summary>
    /// Gets or sets the list of sample rates at which analysis will be done. The full operation will be repeated for each frequency.
    /// This can make a significant performance difference with GetPitch().
    /// With GetPitchAsync, the multiple operations can be done in separate tasks.
    /// </summary>
    ICollection<int> AnalyzeSampleRates { get; set; }
}
