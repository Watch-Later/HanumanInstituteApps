﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using HanumanInstitute.Player432hz.ViewModels;
using Moq;
using MvvmDialogs;
using MvvmDialogs.Avalonia;
using MvvmDialogs.FrameworkDialogs;
using Xunit;

namespace HanumanInstitute.Player432hz.Tests.ViewModels;

public class PlaylistViewModelTests
{
    public PlaylistViewModelTests() {}

    public Mock<IDialogService> MockDialogService => _mockDialogService ??= SetupMockDialogService();
    private Mock<IDialogService>? _mockDialogService;
    private Mock<IDialogService> SetupMockDialogService()
    {
        var result = new Mock<IDialogService>();
        result.Setup(x => x.AppSettings).Returns(new AppDialogSettings());
        result.Setup(x => x.FrameworkDialogFactory).Returns(MockFrameworkDialogFactory.Object);
        return result;
    }

    public Mock<IFrameworkDialogFactory> MockFrameworkDialogFactory => _mockFrameworkDialogFactory ??= SetupFrameworkDialogFactory();
    private Mock<IFrameworkDialogFactory> _mockFrameworkDialogFactory;
    private Mock<IFrameworkDialogFactory> SetupFrameworkDialogFactory()
    {
        var result = new Mock<IFrameworkDialogFactory>();
        var dialog = Mock.Of<IFrameworkDialog<string?>>();
        result.Setup(x => x.ShowAsync<OpenFolderDialogSettings, string?>(It.IsAny<INotifyPropertyChanged>(), It.IsAny<OpenFolderDialogSettings>(), It.IsAny<AppDialogSettingsBase>()).Result)
            .Returns(_dialogResult);
        return result;
    }

    public Mock<IFilesListViewModel> MockFileList => _mockFileList ??= new Mock<IFilesListViewModel>();
    private Mock<IFilesListViewModel>? _mockFileList;

    public PlaylistViewModel Model => _model ??= new PlaylistViewModel(MockDialogService.Object, MockFileList.Object);
    private PlaylistViewModel? _model;

    private const string DialogFolderPath = "C:\\";
    private string? _dialogResult = string.Empty; // Moq async result won't work if this is null

    // Must be set before using Model.
    private void SetDialogFolder() => _dialogResult = DialogFolderPath;

    private void AddFolders(int count)
    {
        for (var i = 0; i < count; i++)
        {
            Model.Folders.Source.Add(i.ToString(CultureInfo.InvariantCulture));
        }
    }

    [Fact]
    public void CanAddFolderCommand_ReturnsTrue()
    {
        var result = Model.AddFolderCommand.CanExecute(null);

        Assert.True(result);
    }

    [Fact]
    public void AddFolderCommand_Execute_CallsDialogService()
    {
        Model.AddFolderCommand.Execute(null);

        MockFrameworkDialogFactory.Verify(x => x.ShowAsync<OpenFolderDialogSettings, string?>(It.IsAny<INotifyPropertyChanged>(), It.IsAny<OpenFolderDialogSettings>(), It.IsAny<AppDialogSettingsBase>()));
    }

    [Fact]
    public void AddFolderCommand_ExecuteCancel_NoAction()
    {
        var listCount = Model.Folders.Source.Count;
        // MockDialogService.ShowFolderBrowserDialog() returns false by default.

        Model.AddFolderCommand.Execute(null);

        Assert.Equal(listCount, Model.Folders.Source.Count);
    }

    [Fact]
    public void AddFolderCommand_SelectDir_FolderAddedToList()
    {
        SetDialogFolder();
        var listCount = Model.Folders.Source.Count;

        Model.AddFolderCommand.Execute(null);

        Assert.Equal(listCount + 1, Model.Folders.Source.Count);
        Assert.Equal(DialogFolderPath, Model.Folders.Source.Last());
    }

    [Fact]
    public void AddFolderCommand_SelectDir_FilesListSetPathsCalled()
    {
        // var listCount = Model.Folders.Source.Count;
        SetDialogFolder();

        Model.AddFolderCommand.Execute(null);

        MockFileList.Verify(x => x.SetPaths(It.IsAny<IEnumerable<string>>()), Times.Once);
    }

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(int.MinValue, false)]
    [InlineData(int.MaxValue, true)]
    public void CanRemoveFolderCommand_WithSelectedIndex_ReturnsTrueIfSelectedIndexValid(int selectedIndex, bool expected)
    {
        AddFolders(2); // List contains 2 elements.
        Model.Folders.CurrentPosition = selectedIndex;

        var result = Model.RemoveFolderCommand.CanExecute(null);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void RemoveFolderCommand_NoSelectedIndex_NoAction()
    {
        var listCount = 2;
        AddFolders(listCount);

        Model.RemoveFolderCommand.Execute(null);

        Assert.Equal(listCount, Model.Folders.Source.Count);
    }

    [Fact]
    public void RemoveFolderCommand_EmptyList_NoAction()
    {
        Model.RemoveFolderCommand.Execute(null);

        Assert.Empty(Model.Folders.Source);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void RemoveFolderCommand_ValidSelectedIndex_RemoveAtSelectedIndex(int selectedIndex)
    {
        var listCount = 3;
        AddFolders(listCount);
        Model.Folders.CurrentPosition = selectedIndex;
        var selectedItem = Model.Folders.CurrentItem;

        Model.RemoveFolderCommand.Execute(null);

        Assert.Equal(listCount - 1, Model.Folders.Source.Count);
        Assert.DoesNotContain(selectedItem, Model.Folders.Source);
    }

    [Theory]
    [InlineData(1, 0, -1)]
    [InlineData(2, 0, 0)]
    [InlineData(3, 2, 1)]
    public void RemoveFolderCommand_LastSelected_SetValidSelectedIndex(int count, int sel, int newSel)
    {
        AddFolders(count);
        Model.Folders.CurrentPosition = sel;

        Model.RemoveFolderCommand.Execute(null);

        Assert.Equal(newSel, Model.Folders.CurrentPosition);
    }

    [Fact]
    public void RemoveFolderCommand_ValidSelectedIndex_FilesListSetPathsCalled()
    {
        AddFolders(1);
        Model.Folders.MoveCurrentToFirst();
        MockFileList.Reset();

        Model.RemoveFolderCommand.Execute(null);

        MockFileList.Verify(x => x.SetPaths(It.IsAny<IEnumerable<string>>()), Times.Once);
    }

    //private void AddFiles(PlaylistViewModel model, int count)
    //{
    //    for (var i = 0; i < count; i++)
    //    {
    //        Model.Files.Add(i.ToString());
    //    }
    //}

    //[Fact]
    //public void LoadFiles_HasFolders_FileLocatorCalledWithFolders()
    //{
    //    var model = SetupModel();
    //    AddFolders(model, 1);

    //    Model.LoadFiles();

    //    _mockFileLocator.Verify(x => x.GetAudioFiles(Model.Folders));
    //}

    //[Fact]
    //public void LoadFiles_NoParam_FilesListContainsResult()
    //{
    //    var model = SetupModel();
    //    var files = new[] { "a", "b", "c" };
    //    _mockFileLocator.Setup(x => x.GetAudioFiles(It.IsAny<IEnumerable<string>>())).Returns(files);

    //    Model.LoadFiles();

    //    Assert.Equal(files.Select(x => x), Model.Files.Select(x => x));
    //}
}
