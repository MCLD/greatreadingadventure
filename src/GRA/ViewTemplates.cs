﻿using System;
using System.Collections.Generic;
using System.IO;

namespace GRA
{
    public static class ViewTemplates
    {
        public static readonly string Dashboard = "Dashboard";
        public static readonly string AccessClosed = "IndexAccessClosed";
        public static readonly string ExitAccessClosed = "ExitAccessClosed";
        public static readonly string BeforeRegistration = "IndexBeforeRegistration";
        public static readonly string ExitBeforeRegistration = "ExitBeforeRegistration";
        public static readonly string ProgramEnded = "IndexProgramEnded";
        public static readonly string ExitProgramEnded = "ExitProgramEnded";
        public static readonly string ProgramOpen = "IndexProgramOpen";
        public static readonly string ExitProgramOpen = "ExitProgramOpen";
        public static readonly string RegistrationOpen = "IndexRegistrationOpen";
        public static readonly string ExitRegistrationOpen = "ExitRegistrationOpen";

        public static IEnumerable<string> CopyToShared()
        {
            var issues = new List<string>();

            var viewsPath = Path.Combine(Directory.GetCurrentDirectory(), "Views");
            var sharedViewsPath
                = Path.Combine(Directory.GetCurrentDirectory(), "shared", "views");
            var templatePath
                = Path.Combine(Directory.GetCurrentDirectory(), "shared", "templates");
            var contentPath
                = Path.Combine(Directory.GetCurrentDirectory(), "shared", "content");

            Directory.CreateDirectory(sharedViewsPath);
            Directory.CreateDirectory(templatePath);
            Directory.CreateDirectory(contentPath);

            foreach (var supportFile in Directory
                .EnumerateFiles(viewsPath, "_*.cshtml", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    string filename = Path.GetFileName(supportFile);
                    File.Copy(supportFile, Path.Combine(sharedViewsPath, filename), true);
                    File.Copy(supportFile, Path.Combine(templatePath, filename), true);
                }
                catch (Exception ex)
                {
                    issues.Add($"Could not copy {supportFile} to {sharedViewsPath}: {ex.Message}");
                }
            }

            var viewPath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Home");
            templatePath = Path.Combine(templatePath, "Home");

            Directory.CreateDirectory(templatePath);

            foreach (var file in
                Directory.EnumerateFiles(viewPath, "*.cshtml", SearchOption.TopDirectoryOnly))
            {
                string destinationPath = Path.Combine(templatePath, Path.GetFileName(file));
                try
                {
                    File.Copy(file, destinationPath, true);
                }
                catch (Exception ex)
                {
                    issues.Add($"Problem copying {file} to {destinationPath}: {ex.Message}");
                }
            }

            var defaultFaviconPath = Path.Combine(Directory.GetCurrentDirectory(),
                "assets",
                "defaultfavicon");

            if (!Directory.Exists(defaultFaviconPath))
            {
                issues.Add($"Can't copy social images: {defaultFaviconPath} doesn't exist.");
            }
            else
            {
                foreach (var filePath in Directory.EnumerateFiles(defaultFaviconPath, "*.*"))
                {
                    var contentPathFile = Path.Combine(contentPath,
                        Path.GetFileName(filePath));
                    if (!File.Exists(contentPathFile))
                    {
                        try
                        {
                            File.Copy(filePath, contentPathFile);
                        }
                        catch (Exception ex)
                        {
                            issues.Add($"Can't copy social file {filePath} to content path: {ex.Message}");
                        }
                    }
                }
            }

            var defaultImagesPath = Path.Combine(Directory.GetCurrentDirectory(),
                "assets",
                "defaultimages");

            if (!Directory.Exists(defaultImagesPath))
            {
                issues.Add($"Can't copy images: {defaultImagesPath} doesn't exist.");
            }
            else
            {
                foreach (var filePath in Directory.EnumerateFiles(defaultImagesPath, "*.*"))
                {
                    var contentPathFile = Path.Combine(contentPath,
                        Path.GetFileName(filePath));
                    if (!File.Exists(contentPathFile))
                    {
                        try
                        {
                            File.Copy(filePath, contentPathFile);
                        }
                        catch (Exception ex)
                        {
                            issues.Add($"Can't copy image file {filePath} to content path: {ex.Message}");
                        }
                    }
                }
            }

            return issues;
        }
    }
}
