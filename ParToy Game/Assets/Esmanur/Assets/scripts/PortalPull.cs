using UnityEngine;
using System.Collections;

public class PortalTrigger : MonoBehaviour
{
    public Transform teleportDestination;
    public float pullSpeed = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("cube"))
        {
            StartCoroutine(PullAndTeleport(other.transform));
        }
    }

    private IEnumerator PullAndTeleport(Transform player)
    {
        Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();

        // Isýnlama öncesi yerçekimini devre dýþý býrak
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = true;
        }

        // Çekim iþlemi sýrasýnda karakteri portaldan çek
        while (Vector3.Distance(player.position, transform.position) > 0.1f)
        {
            player.position = Vector3.MoveTowards(player.position, transform.position, pullSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        // Karakteri hedef pozisyona yerleþtir
        player.position = teleportDestination.position;

        // Isýnlama sonrasý yerçekimini yeniden etkinleþtir
        if (playerRigidbody != null)
        {
            // Yeni pozisyonda fizik hesaplamalarý için kýsa bir bekleme süresi ekle
            yield return new WaitForFixedUpdate();

            playerRigidbody.isKinematic = false;
            playerRigidbody.velocity = Vector3.zero; // Hýzýný sýfýrla
            playerRigidbody.angularVelocity = Vector3.zero; // Açýsal hýzýný sýfýrla
            playerRigidbody.useGravity = true; // Yerçekimini yeniden etkinleþtir
        }
    }
}
