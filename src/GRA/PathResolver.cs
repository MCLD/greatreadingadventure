using System;
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

        public string ResolveContentPath(string filePath = default)
        {
            string path = _config[ConfigurationKey.ContentPath];
            if (string.IsNullOrEmpty(path))
            {
                path = "content";
            }
            if (!string.IsNullOrEmpty(filePath))
            {
                if (!path.EndsWith("/") && !filePath.StartsWith("/"))
                {
                    path += "/";
                }
                path += filePath;
            }
            return path;
        }

        public string ResolveContentFilePath(string filePath = default)
        {
            string path = null;
            if (!string.IsNullOrEmpty(_config[ConfigurationKey.ContentDirectory]))
            {
                path = _config[ConfigurationKey.ContentDirectory];
            }
            else
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), "shared", "content");
            }
            if (!string.IsNullOrEmpty(filePath))
            {
                return Path.Combine(path, filePath);
            }
            else
            {
                return path;
            }
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
                if (!path.EndsWith("/") && !filePath.StartsWith("/"))
                {
                    path += "/";
                }
                path += filePath;
            }
            return path;
        }

        public string ResolvePrivateFilePath(string filePath = default)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "shared", "private");

            if (!string.IsNullOrEmpty(filePath))
            {
                return Path.Combine(path, filePath);
            }
            else
            {
                return path;
            }
        }

        public string ResolvePrivateTempFilePath(string filePath = default)
        {
            var tempPath = ResolvePrivateFilePath("temp");

            if(!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            if (string.IsNullOrEmpty(filePath))
            {
                return Path.Combine(tempPath, Path.GetRandomFileName());
            }
            else
            {
                return Path.Combine(tempPath, filePath);
            }
        }
    }
}
