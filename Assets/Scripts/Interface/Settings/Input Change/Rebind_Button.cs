using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum InputAction
{
    Jump,
    Dash,
    Attack,
    Defend,
    Left,
    Right,
    Pause
}

public class Rebind_Button : MonoBehaviour
{
    public InputAction action; // Isso agora aparece como dropdown no Inspector
    public TMP_Text keyText;

    private bool waitingForKey = false;

    void Start()
    {
        keyText.text = Custom_Input.keyBindings[action.ToString()].ToString();
    }

    public void StartRedind()
    {
        StartCoroutine(WaitForKey());
    }

    private IEnumerator WaitForKey()
    {
        waitingForKey = true;
        keyText.text = "Pressione uma tecla...";
        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                Custom_Input.SetKey(action.ToString(), key);
                keyText.text = key.ToString();
                break;
            }
        }

        waitingForKey = false;
    }
}
