#if NETSTANDARD2_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.AspNetCore.Hosting;


namespace DotNet.Util
{
    /// <summary>
    /// PathProvider
    /// </summary>
    public class PathProvider : IPathProvider
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        /// <summary>
        /// PathProvider
        /// </summary>
        /// <param name="environment"></param>
        public PathProvider(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
        }
        /// <summary>
        /// MapPath
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string MapPath(string path)
        {
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, path);
            return filePath;
        }
    }
}
#endif