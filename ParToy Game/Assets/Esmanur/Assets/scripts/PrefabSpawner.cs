using UnityEngine;
using System.Collections.Generic;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject[] prefabs; // Prefablar dizisi
    public int numberOfPrefabsToSpawn = 20; // Spawnlanacak prefab say�s�
    public Transform mazeParent; // Labirent objesinin parent transform'u

    private List<Vector3> spawnPositions = new List<Vector3>();

    void Start()
    {
        // Labirentin duvarlar� aras�ndaki uygun pozisyonlar� belirleyin
        foreach (Transform child in mazeParent)
        {
            // Duvar olmayan pozisyonlar� listeye ekleyin
            if (child.CompareTag("Ground"))
            {
                spawnPositions.Add(child.position);
            }
        }

        // Pozisyonlar�n dolu olup olmad���n� kontrol edin
        if (spawnPositions.Count == 0)
        {
            Debug.LogError("Hi� spawn pozisyonu bulunamad�. 'Ground' etiketi do�ru ayarland���ndan emin olun.");
            return;
        }

        // Prefablari rastgele pozisyonlara yerle�tirin
        for (int i = 0; i < numberOfPrefabsToSpawn; i++)
        {
            if (spawnPositions.Count == 0)
            {
                Debug.LogWarning("Yeterli spawn pozisyonu yok.");
                break;
            }

            int randomIndex = Random.Range(0, spawnPositions.Count);
            Vector3 spawnPosition = spawnPositions[randomIndex];
            spawnPositions.RemoveAt(randomIndex); // Ayn� pozisyona tekrar yerle�tirmemek i�in pozisyonu listeden ��kar

            if (prefabs.Length == 0)
            {
                Debug.LogError("Hi� prefab bulunamad�. 'Prefabs' dizisi do�ru ayarland���ndan emin olun.");
                return;
            }

            int randomPrefabIndex = Random.Range(0, prefabs.Length);
            Instantiate(prefabs[randomPrefabIndex], spawnPosition, Quaternion.identity);

            Debug.Log("Prefab " + randomPrefabIndex + " pozisyonuna spawnland�: " + spawnPosition);
        }
    }
}
