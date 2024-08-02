using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public async void OnStartGameClick()
    {
        await SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex+1);
    }
}
