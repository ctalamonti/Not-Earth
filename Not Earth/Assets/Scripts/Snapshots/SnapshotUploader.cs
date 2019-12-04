using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

public class SnapshotUploader : MonoBehaviour
{
    //TeamDrive drive = new TeamDrive();
    
    void Start()
    {
        //TODO: Get this working with team drives.
        /*
        GoogleDriveTeamDrives.ListRequest request = GoogleDriveTeamDrives.List();
        var data = request.Send();
        TeamDriveList drives =  data.GoogleDriveRequest.ResponseData;
        foreach (var drive in drives.TeamDrives)
        {
            Debug.Log("Drive Name: " + drive.Name);
            Debug.Log("Drive ID: " + drive.Id);
            Debug.Log("-----------------------");
        }
        */
    }

    public static void UploadScreenshot(byte[] toUpload)
    {            
        Debug.Log("Attempting upload");
        var file = new UnityGoogleDrive.Data.File { Name = "Screenshot.png", Content = toUpload };
        Debug.Log("Sending request");
        GoogleDriveFiles.CreateRequest request = new GoogleDriveFiles.CreateRequest(file);
        request.SupportsTeamDrives = true;
        request.Send();
        Debug.Log("Request sent");
        
    }
}
