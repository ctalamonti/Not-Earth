using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{

    private CinemachineVirtualCamera vcam;
    private CinemachineBasicMultiChannelPerlin noise;
    bool normalShake = true;

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        NormalShake();
    }

    //when the camera collides with a trigger object, the Shake Sequence will be activated 
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CameraTrigger")
        {
            StartCoroutine("StartShake");
        }
    }

    /*
    void Update()
    {
        //this will manually activate the Intense Shake for a short time before returning to normal
        if (Input.GetKeyDown(KeyCode.S) && normalShake)
        {
            StartCoroutine("StartShake");
        }
    }
    */
    

    //sets the values for the normal amount of shake; can be adjusted
    void NormalShake()
    {
        noise.m_AmplitudeGain = 2f;
        noise.m_FrequencyGain = 0.5f;
        normalShake = true;
    }

    //sets the values for the intense amount of shake; can be adjusted
    void XtremeShake()
    {
        noise.m_AmplitudeGain = 3f;
        noise.m_FrequencyGain = 1.5f;
        normalShake = false;
    }

    //triggers the more intense shake for a few seconds before returning to normal; time can be adjusted
    IEnumerator StartShake()
    {
        XtremeShake();
        yield return new WaitForSeconds(3f);
        NormalShake();
    }
}


//https://forum.unity.com/threads/how-to-enable-camera-noise-via-script.495032/
//this site was what i based most of this code on, was v helpful
