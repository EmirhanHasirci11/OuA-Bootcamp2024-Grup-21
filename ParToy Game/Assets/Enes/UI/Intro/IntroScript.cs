using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScript : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void handleIntro(GameObject timeline)
    {
        animator.SetTrigger("out");
        timeline.SetActive(true);
        Invoke(nameof(disableIntroPanel), 2f);
    }
    private void disableIntroPanel()
    {
        this.gameObject.SetActive(false);
    }

    public void DeactivateIntroCamera(GameObject camera)
    {
        camera.gameObject.SetActive(false);
    }
}
