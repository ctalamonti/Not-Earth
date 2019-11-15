public SnapshotCamera snapcam;
// Change the Keycode to whatever We decide to use on the VR headset
void Update()
{
    if(Input.GetKeyDown(KeyCode.Space))
	{
		snapCam.CallTakeSnapshot();

	}
}
//Add player to playerobject and make sure that snapshot camera is assigned to adhere.
//Must refresh the Snapshots folder everytime that you want to see your picture