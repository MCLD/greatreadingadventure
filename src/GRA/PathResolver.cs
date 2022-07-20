using System.IO;
using GRA.Abstract;
using Microsoft.Extensions.Configuration;

namespace GRA
{
    public class PathResolver : IPathResolver
    {
        private readonly IConfiguration _config;

        public PathResolver(IConfiguration config)
        {
            _config = config;
        }

        public string ResolveContentFilePath(string filePath = default)
        {
            string path;
            if (!string.IsNullOrEmpty(_config[ConfigurationKey.ContentDirectory]))
            {
                path = _config[ConfigurationKey.ContentDirectory];
            }
            else
            {
                path = Path.Combine(_config[ConfigurationKey.InternalContentPath],
                    "shared",
                    "content");
            }

            return string.IsNullOrEmpty(filePath)
                ? path
                : Path.Combine(path, filePath);
        }

        public string ResolveContentPath(string filePath = default)
        {
            string path = _config[ConfigurationKey.ContentPath];
            if (string.IsNullOrEmpty(path))
            {
                path = "content";
            }
            if (!string.IsNullOrEmpty(filePath))
            {
                if (!path.EndsWith("/", System.StringComparison.OrdinalIgnoreCase)
                    && !filePath.StartsWith("/", System.StringComparison.OrdinalIgnoreCase))
                {
                    path += '/';
                }
                path += filePath;
            }
            return path;
        }

        public string ResolvePrivateFilePath(string filePath = default)
        {
            string path = Path.Combine(_config[ConfigurationKey.InternalContentPath],
                "shared",
                "private");

            return string.IsNullOrEmpty(filePath)
                ? path
                : Path.Combine(path, filePath);
        }

        public string ResolvePrivatePath(string filePath = default)
        {
            string path = _config[ConfigurationKey.ContentPath];
            if (string.IsNullOrEmpty(path))
            {
                path = "private";
            }
            if (!string.IsNullOrEmpty(filePath))
            {
                if (!path.EndsWith("/", System.StringComparison.OrdinalIgnoreCase)
                    && !filePath.StartsWith("/", System.StringComparison.OrdinalIgnoreCase))
                {
                    path += '/';
                }
                path += filePath;
            }
            return path;
        }

        public string ResolvePrivateTempFilePath(string filePath = default)
        {
            var tempPath = ResolvePrivateFilePath("temp");

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            return string.IsNullOrEmpty(filePath)
                ? Path.Combine(tempPath, Path.GetRandomFileName())
                : Path.Combine(tempPath, filePath);
        }
    }
}
