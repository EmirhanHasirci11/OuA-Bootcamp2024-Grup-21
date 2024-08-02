using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public int hitsToBreak = 3; // Kýrýlmak için gerekli vuruþ sayýsý
    private int currentHits = 0;

    public void Hit()
    {
        currentHits++;
        Debug.Log("Nesneye vuruldu! Vuruþ sayýsý: " + currentHits);

        if (currentHits >= hitsToBreak)
        {
            Break();
        }
    }

    private void Break()
    {
        // Kýrýlma efektlerini ve iþlemlerini buraya ekleyin
        Debug.Log("Nesne kýrýldý!");
        Destroy(gameObject); // Nesneyi yok et
    }
}
