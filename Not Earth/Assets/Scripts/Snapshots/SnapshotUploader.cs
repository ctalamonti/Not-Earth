using System.Collections.Generic;
using System.IO;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityGoogleDrive;


public class SnapshotUploader : MonoBehaviour
{
    //TeamDrive drive = new TeamDrive();

    private static AmazonS3Client client;
    
    /// <summary>
    /// How many screenshots have been taken so far 
    /// </summary>
    private static int screenshotCount = -1;
    void Start()
    {
        screenshotCount = -1;
        AWSCredentials credentials = new BasicAWSCredentials("", "");
        client = new AmazonS3Client(credentials);
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
        // Increment the screenshot count
        ++screenshotCount;
        Debug.Log("Screenshot Count: " + screenshotCount);
        // Prevent the first, grey screenshot form being uploaded
        if (screenshotCount == 0) return;
        Debug.Log("Attempting upload");
        // Creates the file object to be uploaded
        FileStream fileStream =
            File.Create("Screenshot" + screenshotCount + ".png");
        fileStream.Write(toUpload, 0, 0);
        PostObject("Screenshot" + screenshotCount + ".png");
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
    
    private static void PostObject(string fileName)
    {
        Debug.Log("Retrieving the file");

        var stream = new FileStream(Application.persistentDataPath +
                                    Path.DirectorySeparatorChar + fileName,
            FileMode.Open, FileAccess.Read, FileShare.Read);

        Debug.Log("Creating request object");
        var request = new PostObjectRequest()
        {
            Bucket = "notearth",
            Key = fileName,
            InputStream = stream,
            CannedACL = S3CannedACL.Private
        };

        Debug.Log("Making HTTP post call");
        
        client.PostObjectAsync(request, (responseObj) =>
        {
            if (responseObj.Exception == null)
            {
                Debug.Log(string.Format("\nobject {0} posted to bucket {1}",
                    responseObj.Request.Key, responseObj.Request.Bucket));
            }
            else
            {
                Debug.Log("Exception while posting the result object");
                Debug.Log(string.Format("\n receieved error {0}" +
                    responseObj.Response.HttpStatusCode.ToString()));
            }
        });
    }
}
