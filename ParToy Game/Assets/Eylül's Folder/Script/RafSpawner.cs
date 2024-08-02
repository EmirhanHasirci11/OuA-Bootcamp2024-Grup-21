using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RafSpawner : MonoBehaviour
{
    public float width; // Spawnlanacak alanın genişliği
    public float spawnYPosition; // Spawnlanma Y konumu
    public GameObject Raflar; // Raf prefabı
    public float rafWidth; // Rafın genişliği

    private float previousRafX; // Bir önceki rafın x konumu
    private float spawnInterval = 2f; // Başlangıçta raf spawnlama aralığı
    public float speedIncreaseRate = 0.05f; // Hız artış oranı
    public float maxSpeed = 10.0f; // Maksimum hız sınırı
    private float currentSpeed = 1f; // Başlangıç hızı

    void Start()
    {
        // Başlangıçta rastgele bir x değeri atayın
        previousRafX = Random.Range(-width, width);

        // Coroutine'i başlat
        StartCoroutine(SpawnObject());
    }

    void Update()
    {
        // Hızın artmasını sağla
        currentSpeed += speedIncreaseRate * Time.deltaTime;

        // Hızı maksimum hızla sınırla
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

        // Spawnlama aralığını güncelle (hız arttıkça aralığı azalt)
        spawnInterval = Mathf.Max(0.5f, 2f - (currentSpeed / 2)); // Başlangıçta daha uzun aralıkla spawnlama
    }

    IEnumerator SpawnObject()
    {
        while (true)
        {
            float newX;

            // Yeni raf x koordinatını belirlerken bir önceki rafın genişliğinden daha uzak olmasını sağla
            do
            {
                newX = Random.Range(-width, width);
            } while (Mathf.Abs(newX - previousRafX) < rafWidth);

            // Yeni rafı yarat ve 90 derece döndür
            GameObject raf = Instantiate(Raflar, new Vector3(newX, spawnYPosition, 0), Quaternion.Euler(0, 90, 0));

            // Rafın aşağı inme hızını güncelle
            //RafHareket rafMover = raf.GetComponent<RafHareket>();
            //if (rafMover != null)
            //{
            //    rafMover.SetSpeed(currentSpeed);
            //}

            // Bir önceki rafın x konumunu güncelle
            previousRafX = newX;

            yield return new WaitForSeconds(3.0f); // Hızlandırılmış spawnlama aralığı
        }
    }

}

