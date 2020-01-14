using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class GameManager : MonoBehaviour
{
    public string folder;
    [Tooltip("A camera chileded to the entire player object that follows the player camera")]
    public Camera screenshotCamera;

    // Update is called once per frame
    void Update()
    {
      
        // Starts or stops boat movement
        // TODO: Get starting onto a timer/player controlled
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BoatMovement.isMoving = !BoatMovement.isMoving;
        }

        // Takes the screenshot
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
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

        // Sets what will happen when the screenshot is taken
        ScreenshotHelper.iSetMainOnCapturedCallback((Texture2D texture2d) => {
            FilePathName fpn = new FilePathName();
            string fileName = fpn.SaveTextureAs(texture2d, FilePathName.AppPath.PersistentDataPath, "Snapshots", false);
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
        Debug.Log("Taking Screenshot...");
        // Sets the mode to use when capturing
        ScreenshotHelper.SetRenderMode(ScreenshotHelper.RenderMode.OnUpdateRender);
        // Actually takes the screenshot
        ScreenshotHelper.iCaptureWithCamera(screenshotCamera);
    }

   
}
