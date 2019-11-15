// Code Taken From https://www.youtube.com/watch?v=d-56p770t0U&list=LLuki3dtfKBRB7qZUcZcQH_A&index=2&t=0s
RequireComponent(typeof(Camera))]
public class SnapshotCamera : MonoBehaviour
{
	Cameran snapCam;

	int resWidth = 256;
	int resHeight = 256;

    void Awake()
	{
		snapCam = GetComponent<SnapshotCamera>();
        if(snapCam.targetTexture == null)
		{
			snapCam.targetTexture = new RenderTexture(resWidth, resHeight, 24);
		}
        else
		{
			resWidth = snapCam.targetTexture.width;
			resHeight = snapCam.targetTexture.height;
		}
		snapCam.gameobject.SetActive(false);
	}
    
    public void CallTakeSnapshot()
	{
		snapCam.gameObject.SetActive(true);
	}
     void LateUpdate()
	{
        if(snapCam.gameObject.acticeInHierarchy)
		{

			Texture2D snapshot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
			snapCam.Render();
			RenderTexture.Active = snapCamt.targetTexture;
			snapshot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
			byte[] bytes = snapshot.EncodeToPNG();
			string fileName = SnapshotName();
			System.IO.File.WriteAllBytes(fileName, bytes);
			Debug.log("Snapshot Taken");
			snapCam.gameObject.SetActive(false);
		}
	}
    // Create a snapshot folder in unity when making this game/experince

    string snapshotName()
	{
        return string.Format("{0}/Snapshots/snap_{1}x{2}_{3}.png",
			Application.dataPath,
			resWidth,
			resHeight,
            System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
	}
}
