using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class GameManager : MonoBehaviour
{
    public string folder;
    [Tooltip("The camera the player is using to look around")]
    public Camera camera;

    // Update is called once per frame
    void Update()
    {
        // Sets what will happen when the screenshot is taken
        ScreenshotHelper.iSetMainOnCapturedCallback((Texture2D texture2d)=>{
            FilePathName fpn = new FilePathName();
            string fileName = fpn.SaveTextureAs(texture2d, FilePathName.AppPath.PersistentDataPath, "Snapshots", false);
            byte[] bytes = fpn.ReadFileToBytes(fileName);
            SnapshotUploader.UploadScreenshot(bytes);
        });
        
        // Starts or stops boat movement
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BoatMovement.isMoving = !BoatMovement.isMoving;
        }
        
        // Takes the screenshot
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ScreenshotHelper.SetRenderMode(ScreenshotHelper.RenderMode.OnUpdateRender);
            ScreenshotHelper.iCaptureWithCamera(camera);
        }
    }
    private void Awake()
    {
        SnapshotUploader.CreateFolder("Photos");
        

    }

   
}
