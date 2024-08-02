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
        // Buraya blok kýrýlma efektleri veya parçalanma animasyonlarý ekleyebilirsin
        Destroy(gameObject);
    }
}
