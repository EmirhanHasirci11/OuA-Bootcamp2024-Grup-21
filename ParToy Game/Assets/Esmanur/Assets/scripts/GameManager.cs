using UnityEngine;
using UnityEngine.SceneManagement; // Oyun sahnelerini kontrol etmek için

public class GameTimer : MonoBehaviour
{
    public float gameDuration = 60.0f; // Oyun süresi (saniye)
    private float timeRemaining;
    private bool isGameOver = false;

    void Start()
    {
        timeRemaining = gameDuration;
    }

    void Update()
    {
        if (!isGameOver)
        {
            // Zamaný güncelle
            timeRemaining -= Time.deltaTime;

            // Süre dolduysa
            if (timeRemaining <= 0)
            {
                EndGame();
            }
        }
    }

    void EndGame()
    {
        isGameOver = true;
        Debug.Log("Süre doldu! Oyun bitti.");

        // Oyun bitirme iþlemleri burada yapýlabilir
        // Örneðin, sahneyi yeniden yükleme veya ana menüye dönme
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Mevcut sahneyi yeniden yükle
    }
}
