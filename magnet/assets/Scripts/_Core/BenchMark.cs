using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 구간 사이의 시간을 체크
/// </summary>
public class BenchMark
{
    static Dictionary<string, float> marks = new Dictionary<string, float>();
    
    public static void Mark(string mark)
    {
        if (marks.ContainsKey(mark))
            marks[mark] = Time.realtimeSinceStartup;
        else
            marks.Add(mark, Time.realtimeSinceStartup);
    }

    public static void BenchTime(string start, string end)
    {
        if (marks.ContainsKey(start) && marks.ContainsKey(end))
        {
            Debug.Log(start + " to " + end + " time: " + (marks[end] - marks[start]).ToString("#.####"));
        }
    }
}
