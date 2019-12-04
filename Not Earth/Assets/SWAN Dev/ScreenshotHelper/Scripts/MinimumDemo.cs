
using UnityEngine;

public class MinimumDemo : MonoBehaviour 
{
	public Camera m_Camera;
	public MeshRenderer m_CubeMeshRenderer;

	public void TakeScreenshot()
	{
		ScreenshotHelper.iCaptureWithCamera(m_Camera, (texture2D)=>{
			// Clear the old texture if exist.
			if(m_CubeMeshRenderer.material.mainTexture != null)
			{
				Destroy(m_CubeMeshRenderer.material.mainTexture);
			}

			// Set the new (captured) screenshot texture to the cube renderer.
			m_CubeMeshRenderer.material.mainTexture = texture2D;

			string savePath = new FilePathName().SaveTextureAs(texture2D);

			Debug.Log("Result - Texture resolution: " + texture2D.width + " x " + texture2D.height + "\nSaved at: " + savePath);
		});
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.S))
		{
			TakeScreenshot();
		}

		if(Input.GetKeyDown(KeyCode.F))
		{
			ScreenshotHelper.iCaptureScreen((texture2D)=>{
				// Clear the old texture if exist.
				if(m_CubeMeshRenderer.material.mainTexture != null)
				{
					Destroy(m_CubeMeshRenderer.material.mainTexture);
				}

				// Set the new (captured) screenshot texture to the cube renderer.
				m_CubeMeshRenderer.material.mainTexture = texture2D;

				string savePath = new FilePathName().SaveTextureAs(texture2D);

				Debug.Log("Result - Texture resolution: " + texture2D.width + " x " + texture2D.height + "\nSaved at: " + savePath);
			});
		}
	}
}
