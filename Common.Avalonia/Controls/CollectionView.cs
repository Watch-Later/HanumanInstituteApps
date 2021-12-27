﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace HanumanInstitute.Common.Avalonia;

/// <inheritdoc />
public class CollectionView<T> : ICollectionView<T>
{
    /// <summary>
    /// Initializes a new instance of the ListCollectionView class.
    /// </summary>
    public CollectionView()
    {
        Source.CollectionChanged += Source_CollectionChanged;
    }
    
    /// <summary>
    /// Initializes a new instance of the ListCollectionView class, using a supplied collection that implements IList<typeparamref name="T"/>.
    /// </summary>
    /// <param name="list">The underlying collection, which must implement System.Collections.IList<typeparamref name="T"/>.</param>
    public CollectionView(IEnumerable<T> list) : this()
    {
        Source.AddRange(list.ToList());
        CurrentItem = Source.FirstOrDefault()!;
    }

    /// <inheritdoc />
    public ObservableCollectionWithRange<T> Source { get; private set; } = new ObservableCollectionWithRange<T>();
    
    private void Source_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SetAndCoercePosition(CurrentPosition);
    }

    /// <inheritdoc />
    public int CurrentPosition
    {
        get => _currentPosition;
        set => SetAndCoercePosition(value);
    }
    private int _currentPosition = -1;

    private void SetAndCoercePosition(int position)
    {
        if (position > -1 || _currentPosition > -1)
        {
            _currentPosition = Math.Max(-1, Math.Min(Source.Count - 1, position));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentItem)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentPosition)));
        }
    }

    /// <inheritdoc />
    public T? CurrentItem
    {
        get => _currentPosition > -1 ? Source.ElementAtOrDefault(_currentPosition) : default;
        set => SetAndCoercePosition(value != null ? Source.IndexOf(value) : -1);
    }

    /// <inheritdoc />
    public void MoveCurrentToFirst() => CurrentItem = Source.FirstOrDefault();

    /// <inheritdoc />
    public void MoveCurrentToLast() => CurrentItem = Source.LastOrDefault();

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        using var iterator = Source.GetEnumerator();
        while (iterator.MoveNext())
        {
            yield return iterator.Current!;
        }
    }
    
    public IEnumerator GetEnumerator()
    {
        using var iterator = Source.GetEnumerator();
        while (iterator.MoveNext())
        {
            yield return iterator.Current!;
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
