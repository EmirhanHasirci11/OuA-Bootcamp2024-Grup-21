using UnityEngine;
using UnityEngine.SceneManagement; // Oyun sahnelerini kontrol etmek i�in

public class GameTimer : MonoBehaviour
{
    public float gameDuration = 60.0f; // Oyun s�resi (saniye)
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
            // Zaman� g�ncelle
            timeRemaining -= Time.deltaTime;

            // S�re dolduysa
            if (timeRemaining <= 0)
            {
                EndGame();
            }
        }
    }

    void EndGame()
    {
        isGameOver = true;
        Debug.Log("S�re doldu! Oyun bitti.");

        // Oyun bitirme i�lemleri burada yap�labilir
        // �rne�in, sahneyi yeniden y�kleme veya ana men�ye d�nme
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Mevcut sahneyi yeniden y�kle
    }
}
