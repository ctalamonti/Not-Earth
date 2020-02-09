using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBase : MonoBehaviour
{
    bool trigger = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            trigger = true;
        }
    }
    protected virtual void DoAction()
    {
        
    }
    private void Update()
    {
        if (trigger)
        {
            DoAction();
        }
    }
}
