using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class SetPasswords
{

#if UNITY_EDITOR

    static SetPasswords()
    {

        PlayerSettings.Android.keystorePass = "beatles1";
        PlayerSettings.Android.keyaliasPass = "beatles1";
    }

#endif
}