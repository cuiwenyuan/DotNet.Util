using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Util;

namespace DotNet.Test._452
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Oracle
            var connectionString = "";
            var dbHelper = DbHelperFactory.Create(CurrentDbType.Oracle, connectionString);
            var commandText = string.Empty;
            dbHelper.ExecuteNonQuery(commandText);
        }
    }
}
