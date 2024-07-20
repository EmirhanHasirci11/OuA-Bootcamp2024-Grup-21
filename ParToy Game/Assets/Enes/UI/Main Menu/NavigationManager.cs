using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
        this.gameObject.SetActive(true);
    }
}
