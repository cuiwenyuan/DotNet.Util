using System;
using NewLife;

class Program
{
    static void Main()
    {
        string targetValue = "A1B2C3D4";
        var len = targetValue.Length / 2;
        for (int x = 0; x < len; x++)
        {
            int i = (targetValue.Substring(x * 2, 2), 16).ToInt();
            Console.WriteLine($"pair ({targetValue.Substring(x * 2, 2)}, 16).ToInt() = {i}");
        }
        Console.WriteLine("---");
        var tuple = (("FF", 16));
        Console.WriteLine($"NewLife.Utility.ToInt((FF,16), 0) = {NewLife.Utility.ToInt(tuple, 0)}");
        Console.WriteLine($"NewLife.Utility.ToInt(\"FF\", 0) = {NewLife.Utility.ToInt("FF", 0)}");
    }
}
