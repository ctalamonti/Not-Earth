// Taken from kalineh at https://gist.github.com/kalineh/1eccb28c600ae98516ea577c0afb9339 on 1/14/2020
using UnityEngine;
using System.Collections;

public class CameraSmooth
    : MonoBehaviour
{
    public Camera cameraTarget;
    public Camera cameraSelf;
    public bool enableSmooth = true;

    [Range(0.0f, 12.0f)]
    public float lerpPositionRate = 8.0f;
    [Range(0.0f, 12.0f)]
    public float lerpRotationRate = 4.0f;

    public void Start()
    {
        if (!cameraSelf)
            cameraSelf = GetComponent<Camera>();

        //cameraTarget = PlayerCameraSupport.GetCamera();

        // just make sure smooth camera set to None (Main Display)
        // vive will render the both eyes camera, and main game window will show smooth
        cameraSelf.stereoTargetEye = StereoTargetEyeMask.None;
        cameraSelf.targetDisplay = 0;
        cameraSelf.fieldOfView = 60;

        cameraSelf.nearClipPlane = cameraTarget.nearClipPlane;
        cameraSelf.farClipPlane = cameraTarget.farClipPlane;
        cameraSelf.transform.position = cameraTarget.transform.position;
        cameraSelf.transform.rotation = cameraTarget.transform.rotation;

        cameraTarget.targetDisplay = 0;
    }

    public void FixedUpdate()
    {
        if (!cameraTarget)
            return;

        var posRate = lerpPositionRate;
        var rotRate = lerpRotationRate;

        if (enableSmooth)
        {
            transform.position = Vector3.Lerp(transform.position, cameraTarget.transform.position, Mathf.Clamp01(posRate * Time.fixedDeltaTime));
            transform.rotation = Quaternion.Slerp(transform.rotation, cameraTarget.transform.rotation, Mathf.Clamp01(rotRate * Time.fixedDeltaTime));
        }
        else
        {
            transform.position = cameraTarget.transform.position;
            transform.rotation = cameraTarget.transform.rotation;
        }
    }
}