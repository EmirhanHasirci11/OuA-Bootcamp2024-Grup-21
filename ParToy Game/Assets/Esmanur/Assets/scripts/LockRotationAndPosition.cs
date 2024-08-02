using UnityEngine;

public class LockRotationAndPosition : MonoBehaviour
{
    // Sabitlenmesi gereken Y eksenindeki pozisyon
    public float fixedYPosition;

    void Update()
    {
        // Nesnenin pozisyonunu sadece Y ekseninde güncelleyebilmek için sabitle
        Vector3 currentPosition = transform.position;
        currentPosition.y = fixedYPosition; // Y eksenindeki pozisyon sabitlenir
        transform.position = currentPosition;

        // Nesnenin rotasýný X ve Z eksenlerinde sýfýrla, Y eksenindeki rotasyon serbest
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.x = 0;
        currentRotation.z = 0;
        transform.eulerAngles = currentRotation;
    }
}
