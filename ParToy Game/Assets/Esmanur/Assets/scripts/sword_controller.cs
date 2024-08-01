using UnityEngine;

public class SwordController : MonoBehaviour
{
    public GameObject block; // Kýrmak istediðiniz blok
    private BlockController blockController;
    public Transform player; // Karakterin Transform'u
    public float attackDistance = 2.0f; // Saldýrý mesafesi

    void Start()
    {
        blockController = block.GetComponent<BlockController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Attack();
        }
    }

    void Attack()
    {
        // Karakterin blok ile olan mesafesini kontrol et
        float distance = Vector3.Distance(block.transform.position, player.position);

        if (distance <= attackDistance)
        {
            // Buraya animasyon veya saldýrý efektleri ekleyebilirsin
            blockController.TakeHit();
        }
    }
}
