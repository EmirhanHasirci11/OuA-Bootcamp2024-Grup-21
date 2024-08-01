using UnityEngine;
using TMPro;

public class CollectItems : MonoBehaviour
{
    public int itemCount = 0;
    public TextMeshProUGUI itemCountText;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        Debug.Log("CollectItems Start - Constraints: " + rb.constraints);

        if (itemCountText == null)
        {
            Debug.LogError("ItemCountText is not assigned in the Inspector.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectible"))
        {
            itemCount++;
            UpdateItemCountText();
            Destroy(other.gameObject);
        }
    }

    void UpdateItemCountText()
    {
        if (itemCountText != null)
        {
            itemCountText.text = "Toplanan Nesne Sayýsý: " + itemCount.ToString();
        }
        else
        {
            Debug.LogWarning("Item Count Text is not assigned.");
        }
    }
}
