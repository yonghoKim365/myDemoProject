using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class JsonCustomData
{
    public JsonCustomData() { }
    public JsonCustomData(string str)
    {
        str = str.Replace("[", "");
        str = str.Replace("]", "");
        str = str.Replace(" ", "");

        string[] arr = str.Split(',');
        for (int i = 0; i < arr.Length; i++)
            list.Add(arr[i]);
    }

    public int Count { get { return list.Count; } }
    public string this[int index] { get { return list[index]; } }
    public string this[string index] { get { return GetField(index); } }

    string GetField(string index)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == index)
                return list[i];
        }

        return "";
    }

    public override string ToString()
    {
        string Msg = "";
        for (int i = 0; i < list.Count; i++)
        {
            if (i != 0)
                Msg += ",";

            Msg += list[i];
        }

        return Msg;
    }

    public List<string> list = new List<string>();
}