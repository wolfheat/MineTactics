using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class SetPasswords
{

#if UNITY_EDITOR

    static SetPasswords()
    {

        PlayerSettings.keystorePass = "beatles1";
        PlayerSettings.keyaliasPass = "beatles1";
    }

#endif
}