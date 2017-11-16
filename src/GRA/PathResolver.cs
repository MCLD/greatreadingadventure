using GRA.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace GRA
{
    public class PathResolver : IPathResolver
    {
        private readonly IConfigurationRoot _config;
        public PathResolver(IConfigurationRoot config)
        {
            _config = config;
        }

        public string ResolveContentPath(string filePath = default(string))
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

        public string ResolveContentFilePath(string filePath = default(string))
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
    }
}
