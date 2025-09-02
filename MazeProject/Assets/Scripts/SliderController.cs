using System;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Action<float> SliderValueChange;

    private void Start()
    {
        Slider slider = gameObject.GetComponent<Slider>();
        slider.onValueChanged.AddListener(SlideChange);

        slider.wholeNumbers = true;
        slider.minValue = 3;
        slider.maxValue = 51;
    }

    void SlideChange(float value)
    {
        if (value % 2 == 0)
            value++;

        SliderValueChange.Invoke(value);
    }
}
