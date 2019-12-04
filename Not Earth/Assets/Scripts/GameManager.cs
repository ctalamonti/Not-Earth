using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Tooltip("The camera the player is using to look around")]
    public Camera camera;

    // Update is called once per frame
    void Update()
    {
        ScreenshotHelper.iSetMainOnCapturedCallback((Texture2D texture2d)=>{
            FilePathName fpn = new FilePathName();
            string fileName = fpn.SaveTextureAs(texture2d, FilePathName.AppPath.PersistentDataPath, "Snapshots", false);
            byte[] bytes = fpn.ReadFileToBytes(fileName);
            SnapshotUploader.UploadScreenshot(bytes);
        });
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BoatMovement.isMoving = !BoatMovement.isMoving;
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ScreenshotHelper.SetRenderMode(ScreenshotHelper.RenderMode.OnUpdateRender);
            ScreenshotHelper.iCaptureWithCamera(camera);
        }
    }
    
    
}
