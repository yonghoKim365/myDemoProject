using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public class LocalJsonLoader
{
    public static string LoadJsonFile(string fileName, bool Decrypt = true)
    {
        string path = System.Environment.CurrentDirectory + fileName;
        //Console.WriteLine(path);

        if (!File.Exists(path))
        {
            Console.WriteLine("null");
            return null;
        }

        string info = "";
        info = File.ReadAllText(path);

        return info;
    }
}
