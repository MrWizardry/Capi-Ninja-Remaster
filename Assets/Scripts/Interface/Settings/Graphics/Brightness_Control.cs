using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Brightness_Control : MonoBehaviour
{
    public Image imageBlacken;
    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = imageBlacken.color.a;
        slider.onValueChanged.AddListener(SetBrightnessLevel);
    }

    public void SetBrightnessLevel(float sliderVal)
    {
        Color color = imageBlacken.color;
        color.a = sliderVal;
        imageBlacken.color = color;
    }
}
