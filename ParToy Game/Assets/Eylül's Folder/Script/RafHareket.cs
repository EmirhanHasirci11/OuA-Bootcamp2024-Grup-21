using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class RafHareket : MonoBehaviour
{
    private float speed = 0.5f; // Başlangıç hızı

    void Update()
    {
        // Rafı aşağı doğru hareket ettir
        transform.position += UnityEngine.Vector3.down * speed * Time.deltaTime;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
