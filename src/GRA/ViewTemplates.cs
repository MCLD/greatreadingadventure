using System;
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

        public static void CopyToShared(string contentRoot, Serilog.ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            var viewsPath = Path.Combine(contentRoot, "Views");
            var sharedViewsPath = Path.Combine(contentRoot, "shared", "views");
            var templatePath = Path.Combine(contentRoot, "shared", "templates");
            var contentPath = Path.Combine(contentRoot, "shared", "content");

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
                catch (IOException ioex)
                {
                    logger.Error("Could not copy {SourceFile} to {DestinationPath}: {ErrorMessage}",
                        supportFile,
                        sharedViewsPath,
                        ioex.Message);
                }
            }

            var viewPath = Path.Combine(contentRoot,
                "Views",
                "Home");
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
                catch (IOException ioex)
                {
                    logger.Error("Could not copy {SourceFile} to {DestinationPath}: {ErrorMessage}",
                        file,
                        destinationPath,
                        ioex.Message);
                }
            }

            var customWwwroot = Path.Combine(contentPath, "wwwroot");

            if (Directory.Exists(customWwwroot))
            {
                var copied = new List<string>();
                foreach (var filePath in Directory.EnumerateFiles(customWwwroot, "*.*"))
                {
                    var wwwrootPath = Path.Combine(contentRoot, "wwwroot", filePath);
                    try
                    {
                        File.Copy(filePath, wwwrootPath);
                        copied.Add(filePath);
                    }
                    catch (IOException ioex)
                    {
                        logger.Error("Could not copy {SourceFile} to {DestinationPath}: {ErrorMessage}",
                            filePath,
                            wwwrootPath,
                            ioex.Message);
                    }
                }
                if (copied.Count > 0)
                {
                    logger.Information("Copied {Count} files from {SourcePath} to {DestinationPath}: {Files}",
                        copied.Count,
                        customWwwroot,
                        Path.Combine(contentRoot, "wwwroot"),
                        string.Join(", ", copied));
                }
            }

            var defaultImagesPath
                = Path.Combine(contentRoot,
                    "assets",
                    "defaultimages");

            if (!Directory.Exists(defaultImagesPath))
            {
                logger.Error("Could not copy images: {SourcePath} does not exist",
                    defaultImagesPath);
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
                        catch (IOException ioex)
                        {
                            logger.Error("Could not copy {SourceFile} to {DestinationPath}: {ErrorMessage}",
                                filePath,
                                contentPath,
                                ioex.Message);
                        }
                    }
                }
            }
        }
    }
}
