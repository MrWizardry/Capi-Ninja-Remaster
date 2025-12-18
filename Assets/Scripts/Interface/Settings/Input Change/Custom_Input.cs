using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Custom_Input
{
    public static Dictionary<string, KeyCode> keyBindings = new Dictionary<string, KeyCode>();

    static Custom_Input()
    {
        LoadBindings();
    }

    public static bool GetKeyDown(string action)
    {
        if (keyBindings.ContainsKey(action))
        {
            return Input.GetKeyDown(keyBindings[action]);
        }
        return false;
    }

     public static bool GetKeyUp(string action)
    {
        if (keyBindings.ContainsKey(action))
            return Input.GetKeyUp(keyBindings[action]);
        return false;
    }

    public static bool GetKey(string action)
    {
        if (keyBindings.ContainsKey(action))
            return Input.GetKey(keyBindings[action]);
        return false;
    }

    public static void SetKey(string action, KeyCode key)
    {
        keyBindings[action] = key;
        PlayerPrefs.SetString(action, key.ToString());
    }

    public static void LoadBindings()
    {
        Debug.Log("Carregando bindings...");
        SetDefault("Jump", KeyCode.Space);
        SetDefault("Attack", KeyCode.Mouse0);
        SetDefault("Grapple", KeyCode.Mouse1);
        SetDefault("Left", KeyCode.A);
        SetDefault("Right", KeyCode.D);
        SetDefault("Pause", KeyCode.Escape);
        
    }

    private static void SetDefault(string action, KeyCode defaultKey)
    {
        string savedKey = PlayerPrefs.GetString(action, defaultKey.ToString());
        if (System.Enum.TryParse(savedKey, out KeyCode key))
        {
            keyBindings[action] = key;
        }
    }
}
