using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string folder;
    [Tooltip("A camera chileded to the entire player object that follows the player camera")]
    public Camera screenshotCamera;

    [Tooltip("The amount 0 to 1 that the trigger needs to be pushed to take a screenshot")]
    public float snapshotThreshold = 0.9f;
    
    // Update is called once per frame
    void Update()
    {
        
        // Sets a bool if both stick buttons are pressed at the same time
        bool startRide = OVRInput.Get(OVRInput.Button.PrimaryThumbstick) &&
                         OVRInput.Get(OVRInput.Button.SecondaryThumbstick);
        
        // Starts or stops boat movement
        // TODO: Get starting onto a timer/player controlled
        if (Input.GetKeyDown(KeyCode.Space) || startRide)
        {
            BoatMovement.isMoving = !BoatMovement.isMoving;
        }

        // Sets a bool if the triggers are pushed down far enough
        bool triggerPushed =
            (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > snapshotThreshold) ||
            (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > snapshotThreshold);

        // Takes the screenshot
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || triggerPushed)
        {
            TakeScreenshot();
        }
    }
    private void Awake()
    {
        //SnapshotUploader.CreateFolder("Photos");

        // Takes a picture using the blank callback so the camera shown to the spectators and screenshotted from is correct.
        // There is a better way to do this, but I do not know it. Just delete the obviously incorrect photo (it is just plain grey)
        // TODO: Fix the upload of the wrong photo/find a way to set the default spectator camera
        TakeScreenshot();

        screenshotCamera.gameObject.SetActive(false);
        
        // Sets what will happen when the screenshot is taken
        ScreenshotHelper.iSetMainOnCapturedCallback((Texture2D texture2d) => {
            FilePathName fpn = new FilePathName();
            string fileName = fpn.SaveTextureAs(texture2d, FilePathName.AppPath.TemporaryCachePath, "Snapshots", false);
            byte[] bytes = fpn.ReadFileToBytes(fileName);
            SnapshotUploader.UploadScreenshot(bytes);
            fpn.DeleteFile(fileName);
        });
    }

    /// <summary>
    /// Takes a screenshot
    /// </summary>
    public void TakeScreenshot()
    {
        screenshotCamera.gameObject.SetActive(true);
        Debug.Log("Taking Screenshot...");
        // Sets the mode to use when capturing
        ScreenshotHelper.SetRenderMode(ScreenshotHelper.RenderMode.OnUpdateRender);
        // Actually takes the screenshot
        ScreenshotHelper.iCaptureWithCamera(screenshotCamera);
        //screenshotCamera.gameObject.SetActive(false);
    }

   
}
