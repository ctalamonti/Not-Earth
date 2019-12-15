using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class GameManager : MonoBehaviour
{

    [Tooltip("The camera the player is using to look around")]
    public Camera camera;

    [Tooltip("Reference to the left hand controller events")]
    public VRTK_ControllerEvents leftEvents;

    [Tooltip("Reference to the right hand controller events")]
    public VRTK_ControllerEvents rightEvents;

    
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
        
        if (leftEvents.triggerClicked || rightEvents.triggerClicked || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ScreenshotHelper.SetRenderMode(ScreenshotHelper.RenderMode.OnUpdateRender);
            ScreenshotHelper.iCaptureWithCamera(camera);
        }
    }
    private void Awake()
    {
       folder = SnapshotUploader.CreateFolder("Photos");

    }

    public string folder;
}
