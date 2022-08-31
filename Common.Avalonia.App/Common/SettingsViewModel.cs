using HanumanInstitute.Common.Services;
using ReactiveUI;

namespace HanumanInstitute.Common.Avalonia.App;

/// <summary>
/// Shared ViewModel for the settings page.
/// </summary>
/// <typeparam name="TSettings">The data type of application settings.</typeparam>
public abstract class SettingsViewModel<TSettings> : OkCancelViewModel
    where TSettings : SettingsDataBase, new()
{
    private readonly IFluentAvaloniaTheme _fluentTheme;
    protected readonly ISettingsProvider<TSettings> _settingsProvider;

    /// <summary>
    /// Initializes a new instance of the SettingsViewModel class.
    /// </summary>
    protected SettingsViewModel(ISettingsProvider<TSettings> settingsProvider, IFluentAvaloniaTheme fluentTheme)
    {
        _settingsProvider = settingsProvider;
        _fluentTheme = fluentTheme;

        // ReSharper disable once VirtualMemberCallInConstructor
        Settings = CloneSettings(_settingsProvider.Value);
        ThemeList.SelectedValue = Settings.Theme;
        CheckForUpdateList.SelectedValue = Settings.CheckForUpdates;
    }

    /// <summary>
    /// Returns a copy of application settings that are being edited.
    /// </summary>
    public TSettings Settings { get; }

    /// <inheritdoc />
    protected override bool SaveSettings()
    {
        DialogResult = true;

        Settings.Theme = ThemeList.SelectedValue;
        Settings.CheckForUpdates = CheckForUpdateList.SelectedValue;
        Cloning.CopyAllFields(Settings, _settingsProvider.Value);
        _fluentTheme.RequestedTheme = Settings.Theme.ToString();

        _settingsProvider.Save();
        return true;
    }

    /// <summary>
    /// Customizes the way settings are cloned before editing a copy.
    /// </summary>
    /// <param name="value">The settings object to clone.</param>
    /// <returns>The cloned object.</returns>
    protected virtual TSettings CloneSettings(TSettings value) => Cloning.ShallowClone(value);

    /// <summary>
    /// Gets the list of themes for display.
    /// </summary>
    public ListItemCollectionView<AppTheme> ThemeList { get; } = new()
    {
        { AppTheme.Light, "Light" },
        { AppTheme.Dark, "Dark" },
        { AppTheme.HighContrast, "HighContrast" }
    };

    /// <summary>
    /// Gets the list of update intervals for display.
    /// </summary>
    public ListItemCollectionView<UpdateInterval> CheckForUpdateList { get; } = new()
    {
        { UpdateInterval.Daily, "Daily" },
        { UpdateInterval.Weekly, "Weekly" },
        { UpdateInterval.Biweekly, "Biweekly" },
        { UpdateInterval.Monthly, "Monthly" },
        { UpdateInterval.Never, "Never" }
    };

    /// <summary>
    /// Restores default settings.
    /// </summary>
    public RxCommandUnit RestoreDefault => _restoreDefault ??= ReactiveCommand.Create(RestoreDefaultImpl);
    private RxCommandUnit? _restoreDefault;
    /// <summary>
    /// When overriden in a derived class, restores default settings.
    /// </summary>
    protected abstract void RestoreDefaultImpl();
}
