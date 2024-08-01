using UnityEngine;
using System.Collections.Generic;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject[] prefabs; // Prefablar dizisi
    public int numberOfPrefabsToSpawn = 20; // Spawnlanacak prefab sayýsý
    public Transform mazeParent; // Labirent objesinin parent transform'u

    private List<Vector3> spawnPositions = new List<Vector3>();

    void Start()
    {
        // Labirentin duvarlarý arasýndaki uygun pozisyonlarý belirleyin
        foreach (Transform child in mazeParent)
        {
            // Duvar olmayan pozisyonlarý listeye ekleyin
            if (child.CompareTag("Ground"))
            {
                spawnPositions.Add(child.position);
            }
        }

        // Pozisyonlarýn dolu olup olmadýðýný kontrol edin
        if (spawnPositions.Count == 0)
        {
            Debug.LogError("Hiç spawn pozisyonu bulunamadý. 'Ground' etiketi doðru ayarlandýðýndan emin olun.");
            return;
        }

        // Prefablari rastgele pozisyonlara yerleþtirin
        for (int i = 0; i < numberOfPrefabsToSpawn; i++)
        {
            if (spawnPositions.Count == 0)
            {
                Debug.LogWarning("Yeterli spawn pozisyonu yok.");
                break;
            }

            int randomIndex = Random.Range(0, spawnPositions.Count);
            Vector3 spawnPosition = spawnPositions[randomIndex];
            spawnPositions.RemoveAt(randomIndex); // Ayný pozisyona tekrar yerleþtirmemek için pozisyonu listeden çýkar

            if (prefabs.Length == 0)
            {
                Debug.LogError("Hiç prefab bulunamadý. 'Prefabs' dizisi doðru ayarlandýðýndan emin olun.");
                return;
            }

            int randomPrefabIndex = Random.Range(0, prefabs.Length);
            Instantiate(prefabs[randomPrefabIndex], spawnPosition, Quaternion.identity);

            Debug.Log("Prefab " + randomPrefabIndex + " pozisyonuna spawnlandý: " + spawnPosition);
        }
    }
}
