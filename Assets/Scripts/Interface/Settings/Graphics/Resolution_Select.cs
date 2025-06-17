using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Resolution_Select : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    private List<Vector2Int> customResolutions = new List<Vector2Int>()
    {
        new Vector2Int(2560, 1440), // 1440p
        new Vector2Int(1920, 1080), // 1080p
        new Vector2Int(1280, 720),  // 720p
        new Vector2Int(800, 600),   // 600p
    };

    void Start()
    {
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < customResolutions.Count; i++)
        {
            string option = customResolutions[i].x + " x " + customResolutions[i].y;
            options.Add(option);

            if (customResolutions[i].x == Screen.currentResolution.width &&
                customResolutions[i].y == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
    }

    public void ChangeResolution(int index)
    {
        Vector2Int selected = customResolutions[index];
        Screen.SetResolution(selected.x, selected.y, Screen.fullScreen);
    }

    public void SetFullscreenOn()
    {
        Screen.fullScreen = true;
    }

    public void SetFullscreenOff()
    {
        Screen.fullScreen = false;
    }
}
