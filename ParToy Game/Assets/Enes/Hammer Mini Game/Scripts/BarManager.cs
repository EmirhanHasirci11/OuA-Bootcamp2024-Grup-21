using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarManager : MonoBehaviour
{
    public Slider slider;

    public void SetTimeBar(float time)
    {
        slider.value = time;
    }
}
