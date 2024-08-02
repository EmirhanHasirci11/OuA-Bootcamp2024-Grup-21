using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NewBehaviourScript : MonoBehaviour
{
    public GameObject silah;
    public Transform silahTutmaNoktasi;
 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bazooka"))
        {
            Destroy(other.gameObject);
            silah.SetActive(true);
            silah.transform.SetParent(silahTutmaNoktasi);
            silah.transform.localPosition = Vector3.zero;
            silah.transform.localRotation = Quaternion.identity;
        }
        else if (other.CompareTag("mine"))
        {
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("shield"))
        {
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("mermi"))
        {
            Destroy(other.gameObject);
        }
        
    }

}
