using GRA.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GRA
{
    public class PathResolver : IPathResolver
    {
        private readonly IConfigurationRoot _config;
        public PathResolver(IConfigurationRoot config)
        {
            _config = config;
        }

        public string ResolveContentPath(string filePath)
        {
            string path = _config[ConfigurationKey.ContentPath];
            if (!path.EndsWith("/"))
            {
                path += "/";
            }
            return path + filePath;
        }
    }
}
