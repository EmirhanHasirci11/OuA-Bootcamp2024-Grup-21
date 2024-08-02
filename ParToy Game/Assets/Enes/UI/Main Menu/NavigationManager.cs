using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    private Animator mainAnimator;

    private void Start()
    {
        mainAnimator = GetComponent<Animator>();
    }

    // ACTIVATING CAMERAS
    public void ActivateCamera(GameObject camera)
    {
        camera.SetActive(true);
    }

    public void DeactivateCamera(GameObject camera)
    {
        camera.SetActive(false);
    }

    // HANDLE ANIMATIONS
    public void AnimationToFeatures(Animator animator)
    {
        mainAnimator.SetTrigger("out");
        animator.SetTrigger("in");
    }

    public void AnimationToMainMenu(Animator animator)
    {
        mainAnimator.SetTrigger("in");
        animator.SetTrigger("out");
    }
}
