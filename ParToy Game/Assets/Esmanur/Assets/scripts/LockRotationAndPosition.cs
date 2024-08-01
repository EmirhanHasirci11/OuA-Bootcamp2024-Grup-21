using UnityEngine;

public class LockRotationAndPosition : MonoBehaviour
{
    // Sabitlenmesi gereken Y eksenindeki pozisyon
    public float fixedYPosition;

    void Update()
    {
        // Nesnenin pozisyonunu sadece Y ekseninde g�ncelleyebilmek i�in sabitle
        Vector3 currentPosition = transform.position;
        currentPosition.y = fixedYPosition; // Y eksenindeki pozisyon sabitlenir
        transform.position = currentPosition;

        // Nesnenin rotas�n� X ve Z eksenlerinde s�f�rla, Y eksenindeki rotasyon serbest
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.x = 0;
        currentRotation.z = 0;
        transform.eulerAngles = currentRotation;
    }
}
