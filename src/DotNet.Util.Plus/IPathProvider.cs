using System;
using System.Collections.Generic;
using System.Text;

namespace DotNet.Util
{
    /// <summary>
    /// IPathProvider
    /// </summary>
    public partial interface IPathProvider
    {
        /// <summary>
        /// MapPath
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string MapPath(string path);
    }
}
