using System;
using System.Collections.Generic;
using System.Text;

namespace DotNet.Util
{
    public partial interface IPathProvider
    {
        string MapPath(string path);
    }
}
