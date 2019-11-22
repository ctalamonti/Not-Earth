using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;

public class SnapshotUploader : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Attempting upload");
            byte[] bytes = new byte[10];
            var file = new UnityGoogleDrive.Data.File { Name = "Screenshot.png", Content = bytes };
            Debug.Log("Sending request");
            GoogleDriveFiles.Create(file).Send();
            Debug.Log("Request sent");
        }
    }
}
