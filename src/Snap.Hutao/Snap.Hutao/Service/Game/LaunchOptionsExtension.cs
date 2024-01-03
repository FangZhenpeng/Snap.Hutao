﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.PathAbstraction;
using System.Collections.Immutable;
using System.IO;

namespace Snap.Hutao.Service.Game;

internal static class LaunchOptionsExtension
{
    public static bool TryGetGamePathAndGameDirectory(this LaunchOptions options, out string gamePath, [NotNullWhen(true)] out string? gameDirectory)
    {
        gamePath = options.GamePath;

        gameDirectory = Path.GetDirectoryName(gamePath);
        if (string.IsNullOrEmpty(gameDirectory))
        {
            return false;
        }

        return true;
    }

    public static bool TryGetGameDirectoryAndGameFileName(this LaunchOptions options, [NotNullWhen(true)] out string? gameDirectory, [NotNullWhen(true)] out string? gameFileName)
    {
        string gamePath = options.GamePath;

        gameDirectory = Path.GetDirectoryName(gamePath);
        if (string.IsNullOrEmpty(gameDirectory))
        {
            gameFileName = default;
            return false;
        }

        gameFileName = Path.GetFileName(gamePath);
        if (string.IsNullOrEmpty(gameFileName))
        {
            return false;
        }

        return true;
    }

    public static bool TryGetGamePathAndGameFileName(this LaunchOptions options, out string gamePath, [NotNullWhen(true)] out string? gameFileName)
    {
        gamePath = options.GamePath;

        gameFileName = Path.GetFileName(gamePath);
        if (string.IsNullOrEmpty(gameFileName))
        {
            return false;
        }

        return true;
    }

    public static bool TryGetGamePathAndFilePathByName(this LaunchOptions options, string fileName, out string gamePath, [NotNullWhen(true)] out string? filePath)
    {
        if (options.TryGetGamePathAndGameDirectory(out gamePath, out string? gameDirectory))
        {
            filePath = Path.Combine(gameDirectory, fileName);
            return true;
        }

        filePath = default;
        return false;
    }

    public static ImmutableList<GamePathEntry> GetGamePathEntries(this LaunchOptions options, out GamePathEntry? entry)
    {
        string gamePath = options.GamePath;

        if (string.IsNullOrEmpty(gamePath))
        {
            entry = default;
            return options.GamePathEntries;
        }

        if (options.GamePathEntries.SingleOrDefault(entry => string.Equals(entry.Path, options.GamePath, StringComparison.OrdinalIgnoreCase)) is { } existed)
        {
            entry = existed;
            return options.GamePathEntries;
        }

        entry = GamePathEntry.Create(options.GamePath);
        return [.. options.GamePathEntries, entry];
    }

    public static ImmutableList<GamePathEntry> RemoveGamePathEntry(this LaunchOptions options, GamePathEntry? entry, out GamePathEntry? selected)
    {
        if (entry is not null)
        {
            if (string.Equals(options.GamePath, entry.Path, StringComparison.OrdinalIgnoreCase))
            {
                options.GamePath = string.Empty;
            }

            options.GamePathEntries = options.GamePathEntries.Remove(entry);
        }

        return options.GetGamePathEntries(out selected);
    }

    public static ImmutableList<GamePathEntry> UpdateGamePathAndRefreshEntries(this LaunchOptions options, string gamePath)
    {
        options.GamePath = gamePath;
        ImmutableList<GamePathEntry> entries = options.GetGamePathEntries(out _);
        options.GamePathEntries = entries;
        return entries;
    }
}