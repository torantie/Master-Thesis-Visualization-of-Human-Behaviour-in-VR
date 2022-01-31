using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class UnityStartup : MonoBehaviour
{
    static UnityStartup()
    {
        SerializationHelper.SetCurrentCultureDateTimeFormat();
    }
}
