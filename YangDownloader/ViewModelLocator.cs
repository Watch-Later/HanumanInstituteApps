﻿using Avalonia.Controls;
using HanumanInstitute.BassAudio;
using HanumanInstitute.Downloads;
using HanumanInstitute.FFmpeg;
using HanumanInstitute.MediaPlayer.Avalonia.Bass;
using HanumanInstitute.MvvmDialogs.Avalonia;
using HanumanInstitute.YangDownloader.Business;
using Splat;

namespace HanumanInstitute.YangDownloader;

/// <summary>
/// This class contains static references to all the view models in the
/// application and provides an entry point for the bindings.
/// </summary>
public static class ViewModelLocator
{
    /// <summary>
    /// Initializes a new instance of the ViewModelLocator class.
    /// </summary>
    static ViewModelLocator()
    {
        var container = Locator.CurrentMutable;
            
        // Services
        container.AddCommonServices()
            .AddBassAudio()
            .AddDownloads();
        container.Register(() => (IDialogService)new DialogService(new DialogManager(
            viewLocator: new ViewLocator(),
            dialogFactory: new DialogFactory().AddMessageBox()),
            viewModelFactory: t => Locator.Current.GetService(t)));
        container.Register(() => (IBassDevice)BassDevice.Instance);
        container.Register<IEncoderService>(() => new EncoderService());
        SplatRegistrations.Register<IMediaMuxer, MediaMuxer>();
        SplatRegistrations.Register<IMediaEncoder, MediaEncoder>();
        SplatRegistrations.Register<IMediaInfoReader, MediaInfoReader>();
        SplatRegistrations.Register<IMediaScript, MediaScript>();
            
        // ViewModels
        SplatRegistrations.Register<MainViewModel>();
        SplatRegistrations.Register<AboutViewModel>();
        SplatRegistrations.Register<EncodeSettingsViewModel>();

        // Business
        SplatRegistrations.RegisterLazySingleton<ISettingsProvider<AppSettingsData>, AppSettingsProvider>();
        SplatRegistrations.RegisterLazySingleton<IAppPathService, AppPathService>();
            
        SplatRegistrations.SetupIOC();
    }

    public static MainViewModel Main => Locator.Current.GetService<MainViewModel>()!;
    public static AboutViewModel About => Locator.Current.GetService<AboutViewModel>()!;
    public static EncodeSettingsViewModel EncodeSettings => Locator.Current.GetService<EncodeSettingsViewModel>()!;

    public static void Cleanup()
    {
    }
}
