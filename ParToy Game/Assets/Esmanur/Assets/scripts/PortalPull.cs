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

        // Is�nlama �ncesi yer�ekimini devre d��� b�rak
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = true;
        }

        // �ekim i�lemi s�ras�nda karakteri portaldan �ek
        while (Vector3.Distance(player.position, transform.position) > 0.1f)
        {
            player.position = Vector3.MoveTowards(player.position, transform.position, pullSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        // Karakteri hedef pozisyona yerle�tir
        player.position = teleportDestination.position;

        // Is�nlama sonras� yer�ekimini yeniden etkinle�tir
        if (playerRigidbody != null)
        {
            // Yeni pozisyonda fizik hesaplamalar� i�in k�sa bir bekleme s�resi ekle
            yield return new WaitForFixedUpdate();

            playerRigidbody.isKinematic = false;
            playerRigidbody.velocity = Vector3.zero; // H�z�n� s�f�rla
            playerRigidbody.angularVelocity = Vector3.zero; // A��sal h�z�n� s�f�rla
            playerRigidbody.useGravity = true; // Yer�ekimini yeniden etkinle�tir
        }
    }
}
