using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggertest : MonoBehaviour
{
    public GameObject leeskPrefab;

    private Vector3 leeskPosition = new Vector3(-4.5f, -4, 15);

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "event1")
        {
            Instantiate(leeskPrefab, leeskPosition, Quaternion.identity);
        }
    }
}
