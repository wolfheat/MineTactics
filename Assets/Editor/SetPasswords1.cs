using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class SetVersion
{
#if UNITY_EDITOR

    static SetVersion()
    {

        PlayerSettings.Android.keystorePass = "beatles1";
        PlayerSettings.Android.keyaliasPass = "beatles1";
    }

#endif
}