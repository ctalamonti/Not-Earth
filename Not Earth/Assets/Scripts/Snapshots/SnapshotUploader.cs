using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;
 

public class SnapshotUploader : MonoBehaviour
{
    //TeamDrive drive = new TeamDrive();
    
    /// <summary>
    /// How many screenshots have been taken so far 
    /// </summary>
    private static uint screenshotCount = 1;
    void Start()
    {
        screenshotCount = 0;
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

    /// <summary>
    /// Takes an image as bytes and uploads it to google drive
    /// </summary>
    /// <param name="toUpload">The bytes to upload</param>
    public static void UploadScreenshot(byte[] toUpload)
    {    
        Debug.Log("Attempting upload");
        // Creates the file object to be uploaded
        var file = new UnityGoogleDrive.Data.File { Name = "Screenshot" + screenshotCount + ".png", Content = toUpload };
        Debug.Log("Sending request");
        // Sets the folder the file needs to go into
        file.Parents = new List<string>();
        file.Parents.Add("188H2_KwT6oAUNxiCj7jhhlohMqgiCE2E");// Change this ID to change the folder
        // Create the request to send to Google Drive
        GoogleDriveFiles.CreateRequest request = new GoogleDriveFiles.CreateRequest(file);
        // Let this work with team drives plox
        request.SupportsTeamDrives = true;
        // Send the request - upload the file
        request.Send();
        Debug.Log("Request sent");
        // Increment the screenshot count
        ++screenshotCount;

    }

    /// <summary>
    /// Creates a folder and gives us a folder name
    /// </summary>
    /// <param name="folderName">making the name for the folder</param>
    /// <returns>folder id</returns>
    public static void  CreateFolder(string folderName)
    {
        var fileMetadata = new UnityGoogleDrive.Data.File() // Creates file
        {
            Name = folderName// sets up folder name

        };
        
        fileMetadata.Parents = new List<string>();
        fileMetadata.Parents.Add("188H2_KwT6oAUNxiCj7jhhlohMqgiCE2E");// Change this ID to change the folder
        fileMetadata.MimeType = "application/vnd.google-apps.folder"; // giving it folder type

        GoogleDriveFiles.CreateRequest request = new GoogleDriveFiles.CreateRequest(fileMetadata);
        var folder = request.Send();
       // Debug.Log(folder.GoogleDriveRequest.);// sending folder to drive
        //return folder.GoogleDriveRequest.ResponseData.Id;  // requesting id
    }
}
