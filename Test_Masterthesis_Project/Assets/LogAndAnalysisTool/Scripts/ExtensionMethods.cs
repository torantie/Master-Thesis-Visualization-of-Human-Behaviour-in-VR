using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class ExtensionMethods
{
    /// <summary>
    /// https://answers.unity.com/questions/1134997/string-to-vector3.html
    /// </summary>
    /// <param name="a_vector"></param>
    /// <returns></returns>
    public static Vector3 StringToVector3(this string a_vector)
    {
        if (a_vector == null)
        {
            return default;
        }

        // Remove the parentheses
        if (a_vector.StartsWith("(") && a_vector.EndsWith(")"))
        {
            a_vector = a_vector.Substring(1, a_vector.Length - 2);
        }
        else
        {
            return default;
        }

        // split the items
        string[] sArray = a_vector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0], CultureInfo.InvariantCulture),
            float.Parse(sArray[1], CultureInfo.InvariantCulture),
            float.Parse(sArray[2], CultureInfo.InvariantCulture));

        return result;
    }

    public static Quaternion StringToQuaternion(this string a_quaternion)
    {
        if (a_quaternion == null)
        {
            return default;
        }

        // Remove the parentheses
        if (a_quaternion.StartsWith("(") && a_quaternion.EndsWith(")"))
        {
            a_quaternion = a_quaternion.Substring(1, a_quaternion.Length - 2);
        }
        else
        {
            return default;
        }

        // split the items
        string[] sArray = a_quaternion.Split(',');

        // store as a Vector3
        Quaternion result = new Quaternion(
            float.Parse(sArray[0], CultureInfo.InvariantCulture),
            float.Parse(sArray[1], CultureInfo.InvariantCulture),
            float.Parse(sArray[2], CultureInfo.InvariantCulture),
            float.Parse(sArray[3], CultureInfo.InvariantCulture));

        return result;
    }

    public static List<T> ParseToEnumCollection<T>(this List<string> a_eventsAsString) where T : struct, Enum
    {
        var enums = new List<T>();
        foreach (var eventAsString in a_eventsAsString)
        {
            if (eventAsString.TryParseToEnum<T>(out var parsedSelectedItem))
            {
                enums.Add(parsedSelectedItem);
            }
        }

        return enums;
    }

    public static bool TryParseToEnum<T>(this string a_enumAsString, out T a_enum) where T : struct, Enum
    {
        if (Enum.TryParse<T>(a_enumAsString, true, out a_enum))
        {
            return true;
        }
        else
        {
            Debug.LogWarningFormat("Cannot parse string {0} to LoggingDataPointType.", a_enumAsString);
            return false;
        }
    }

}
