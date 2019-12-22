using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticSnapshot : MonoBehaviour
{
    public GameObject snapShot;

    public Vector3 snapShotPosition = new Vector3(0, 0, 0);

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Event 2")
        {
            ScreenshotHelper.SetRenderMode(ScreenshotHelper.RenderMode.OnUpdateRender);
            ScreenshotHelper.iCaptureWithCamera(GetComponent<Camera>());
        }
        
    }



}
