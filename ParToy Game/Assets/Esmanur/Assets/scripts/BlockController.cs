using UnityEngine;

public class BlockController : MonoBehaviour
{
    private int hitCount = 0;
    public int hitsToBreak = 3;

    public void TakeHit()
    {
        hitCount++;
        if (hitCount >= hitsToBreak)
        {
            BreakBlock();
        }
    }

    void BreakBlock()
    {
        // Buraya blok k�r�lma efektleri veya par�alanma animasyonlar� ekleyebilirsin
        Destroy(gameObject);
    }
}
