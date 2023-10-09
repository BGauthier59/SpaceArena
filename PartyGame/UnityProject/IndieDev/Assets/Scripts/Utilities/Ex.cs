using System.Collections.Generic;
using UnityEngine;

public class Ex : MonoBehaviour
{
    private static Vector3 QuadraticBeziersCurve(Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 p4 = Vector3.Lerp(p1, p2, t);
        Vector3 p5 = Vector3.Lerp(p2, p3, t);
        return Vector3.Lerp(p4, p5, t);
    }

    public static Vector3 CubicBeziersCurve(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
    {
        Vector3 p5 = QuadraticBeziersCurve(p1, p2, p3, t);
        Vector3 p6 = QuadraticBeziersCurve(p2, p3, p4, t);
        return Vector3.Lerp(p5, p6, t);
    }
    
    public static List<T> ShuffleList<T>(List<T> list)
    {
        List<T> rngList = new List<T>(0);
        int x = list.Count;
        for (int i = 0; i < x; i++)
        {
            int rng = Random.Range(0, list.Count);
            rngList.Add(list[rng]);
            list.RemoveAt(rng);
        }
        return rngList;
    }
    
    public static string ConvertSecondsInMinutes(int seconds)
    {
        var minutes = 0;
        while (seconds >= 60)
        {
            minutes++;
            seconds -= 60;
        }

        var secondsText = "";
        if (seconds < 10) secondsText = "0";

        secondsText += seconds.ToString();

        return $"{minutes}:{secondsText}";
    }
}


