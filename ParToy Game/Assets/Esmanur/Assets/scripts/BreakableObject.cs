using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public int hitsToBreak = 3; // K�r�lmak i�in gerekli vuru� say�s�
    private int currentHits = 0;

    public void Hit()
    {
        currentHits++;
        Debug.Log("Nesneye vuruldu! Vuru� say�s�: " + currentHits);

        if (currentHits >= hitsToBreak)
        {
            Break();
        }
    }

    private void Break()
    {
        // K�r�lma efektlerini ve i�lemlerini buraya ekleyin
        Debug.Log("Nesne k�r�ld�!");
        Destroy(gameObject); // Nesneyi yok et
    }
}
