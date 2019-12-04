/// <summary>
/// By SwanDEV 2017
/// </summary>

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class ScreenshotHelper : MonoBehaviour
{
	public UnityEvent m_MainOnCaptured;

    /// <summary>
    /// The RenderMode for capturing image using camera. (For camera capture methods)
    /// </summary>
    public RenderMode m_RenderMode = RenderMode.OnUpdateRender;
    public enum RenderMode
    {
        /// <summary>
        /// Support Unity standard render pipeline only. Suggested if you didn't configure your project for using Scriptable Render Pipeline (LWRP/HDRP).
        /// </summary>
        OnRenderImage = 0,

        /// <summary>
        /// Support both Unity standard render pipeline and Scriptable Render Pipeline (LWRP/HDRP). 
        /// Support Anti-Aliasing for camera capture methods, even the Anti-Aliasing option is disabled in the Unity QualitySettings.
        /// </summary>
        OnUpdateRender,
    }

    /// <summary>
    /// [OnUpdateRender mode camera capture methods Only] The anti-aliasing level for the resulting texture, 
    /// the greater value results in smoother object edges. Valid value: 1(OFF), 2, 4, 8
    /// (The greater anti-aliasing level and higher resolution together may cause the incorrect output of the textures, please adjust the values as appropriate)
    /// </summary>
    [Range(1, 8)] public int m_AntiAliasingLevel = 4;

    private bool _isBeingCaptureScreen = false;
	private Texture2D _texture2D = null;
	private RenderTexture _renderTexture = null;
	public Text m_DebugText;

    private static ScreenshotHelper _instance = null;
	public static ScreenshotHelper Instance
	{
		get{
            if (_instance == null)
            {
                _instance = new GameObject("[ScreenshotHelper]").AddComponent<ScreenshotHelper>();
            }
            return _instance;
		}
	}

    /// <summary>
    /// Clear the instance of ScreenshotHelper:
    /// Destroy the stored textures, remove callbacks, remove script from camera, and unload all assets that are not used. 
    /// </summary>
    public void Clear(bool clearCallback = true, bool clearTextures = true)
	{
        if (clearCallback)
        {
            if (m_MainOnCaptured != null)
            {
                m_MainOnCaptured.RemoveAllListeners();
                m_MainOnCaptured = null;
            }
        }

        if (clearTextures)
        {
            if (_texture2D != null)
            {
                ClearTexture2D(ref _texture2D);
            }

            if (_renderTexture != null)
            {
                ClearRenderTexture(ref _renderTexture);
            }
        }

        // Remove camera render script from camera
        UnRegisterAllRenderCameras();

        // Clear all assets(including textures, sprites) that are not used (not referenced or selected).
        Resources.UnloadUnusedAssets();
    }

	private void Awake()
	{
        if (_instance == null) _instance = this;
	}

	private void _InitMainOnCaptured()
	{
		if(m_MainOnCaptured == null) m_MainOnCaptured = new UnityEvent();
		m_MainOnCaptured.RemoveAllListeners();
	}

	/// <summary>
	/// Set the main onCaptured callback for receiving all images from all capture methods.
	/// </summary>
	/// <param name="mainOnCaptured">The callback to be fired at each capture.</param>
	public void SetMainOnCapturedCallback(Action mainOnCaptured)
	{
		_InitMainOnCaptured();
		m_MainOnCaptured.AddListener(delegate {
			mainOnCaptured();
		});
	}

	/// <summary>
	/// Set the main onCaptured callback for receiving all images from all capture methods. Return the captured images as Texture2D.
	/// </summary>
	/// <param name="mainOnCaptured">The callback to be fired at each capture, return a Texture2D.</param>
	public void SetMainOnCapturedCallback(Action<Texture2D> mainOnCaptured)
	{
		_InitMainOnCaptured();
		m_MainOnCaptured.AddListener(delegate {
			mainOnCaptured(_texture2D);
		});
	}

	/// <summary>
	/// Set the main onCaptured callback for receiving all images from all capture methods. Return the captured images as Sprite.
	/// </summary>
	/// <param name="mainOnCaptured">The callback to be fired at each capture, return a Sprite.</param>
	public void SetMainOnCapturedCallback(Action<Sprite> mainOnCaptured)
	{
		_InitMainOnCaptured();
		m_MainOnCaptured.AddListener(delegate {
			mainOnCaptured(GetCurrentSprite());
		});
	}

	/// <summary>
	/// Set the main onCaptured callback for receiving all images from all capture methods. Return the captured images as RenderTexture.
	/// </summary>
	/// <param name="mainOnCaptured">The callback to be fired at each capture, return a RenderTexture.</param>
	public void SetMainOnCapturedCallback(Action<RenderTexture> mainOnCaptured)
	{
		_InitMainOnCaptured();
		m_MainOnCaptured.AddListener(delegate {
			mainOnCaptured(_renderTexture);
		});
    }

    /// <summary>
    /// Capture the full screen, return a Texture2D in the callback.
    /// </summary>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    public void CaptureScreen(Action<Texture2D> onCapturedCallback = null)
    {
        StartCoroutine(_TakeFullscreen(onCapturedCallback, null, null));
    }

    /// <summary>
    /// Capture the full screen, return a Sprite in the callback.
    /// </summary>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    public void CaptureScreen_Sprite(Action<Sprite> onCapturedCallback = null)
    {
        StartCoroutine(_TakeFullscreen(null, onCapturedCallback, null));
    }

    /// <summary>
    /// Capture the full screen, return a RenderTexture in the callback.
    /// </summary>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    public void CaptureScreen_RenderTexture(Action<RenderTexture> onCapturedCallback = null)
    {
        StartCoroutine(_TakeFullscreen(null, null, onCapturedCallback));
    }

    /// <summary>
    /// Capture a portion of the screen at specific screen position, return a Texture2D in the callback.
    /// </summary>
    /// <param name="screenPosition">Screen position.</param>
    /// <param name="imageSize">The target image size.</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    public void Capture(Vector2 screenPosition, Vector2 imageSize, Action<Texture2D> onCapturedCallback = null)
    {
        if (_isBeingCaptureScreen)
        {
            Debug.LogWarning("Screenshot being captured, please wait for at least 1 frame for starting another capture!");
            return;
        }
        _isBeingCaptureScreen = true;

        Rect rect = new Rect(screenPosition, imageSize);
        StartCoroutine(_ReadPixelWithRect(rect, onCapturedCallback, null, null, true));
    }

    /// <summary>
    /// Capture a portion of the screen at specific screen position, return a Sprite in the callback.
    /// </summary>
    /// <param name="screenPosition">Screen position.</param>
    /// <param name="imageSize">The target image size.</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    public void Capture_Sprite(Vector2 screenPosition, Vector2 imageSize, Action<Sprite> onCapturedCallback = null)
    {
        if (_isBeingCaptureScreen)
        {
            Debug.LogWarning("Screenshot being captured, please wait for at least 1 frame for starting another capture!");
            return;
        }
        _isBeingCaptureScreen = true;

        Rect rect = new Rect(screenPosition, imageSize);
        StartCoroutine(_ReadPixelWithRect(rect, null, onCapturedCallback, null, true));
    }

    /// <summary>
    /// Capture a portion of the screen at specific screen position, return a RenderTexture in the callback.
    /// </summary>
    /// <param name="screenPosition">Screen position.</param>
    /// <param name="imageSize">The target image size.</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    public void Capture_RenderTexture(Vector2 screenPosition, Vector2 imageSize, Action<RenderTexture> onCapturedCallback = null)
    {
        if (_isBeingCaptureScreen)
        {
            Debug.LogWarning("Screenshot being captured, please wait for at least 1 frame for starting another capture!");
            return;
        }
        _isBeingCaptureScreen = true;

        Rect rect = new Rect(screenPosition, imageSize);
        StartCoroutine(_ReadPixelWithRect(rect, null, null, onCapturedCallback, true));
    }

    private Rect _ConstraintRectWithScreen(Rect rect)
    {
        int ScreenWidth = Screen.width;
        int ScreenHeight = Screen.height;

        // Size correction
        if (rect.width > ScreenWidth) rect.size = new Vector2(ScreenWidth, rect.height);
        if (rect.height > ScreenHeight) rect.size = new Vector2(rect.width, ScreenHeight);

        //Debug.Log("Capture() imageSize: " + rect.size.ToString() + " Screen W: " + ScreenWidth + " Screen H: " + ScreenHeight);

        // Position correction
        if (rect.x + rect.width / 2 > ScreenWidth)
            rect.position = new Vector2(rect.x - (rect.x + rect.width / 2 - ScreenWidth), rect.y);
        if (rect.x - rect.width / 2 < 0)
            rect.position = new Vector2(rect.x + (rect.width / 2 - rect.x), rect.y);
        if (rect.y + rect.height / 2 > ScreenHeight)
            rect.position = new Vector2(rect.x, rect.y - (rect.y + rect.height / 2 - ScreenHeight));
        if (rect.y - rect.height / 2 < 0)
            rect.position = new Vector2(rect.x, rect.y + (rect.height / 2 - rect.y));

        UpdateDebugText("Capture position: " + rect.position + " | imageSize: " + rect.size);
        return rect;
    }

    /// <summary>
    /// Capture image with the view of the target camera. Return a Texture2D in the callback.
    /// </summary>
    /// <param name="camera">Target Camera.</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    /// <param name="width">The width that forcing the image to fill in.</param>
    /// <param name="height">The height that forcing the image to fill in.</param>
    public void CaptureWithCamera(Camera camera, Action<Texture2D> onCapturedCallback = null, int width = 0, int height = 0)
    {
        UpdateDebugText(camera.name + " rect: " + camera.pixelWidth + " x " + camera.pixelHeight);

        RegisterRenderCamera(camera);
        CameraRenderBase cameraBase = _SetupCameraRenderBase(camera);

        if (cameraBase != null)
        {
            cameraBase.SetOnCaptureCallback((Texture2D tex) =>
            {
                _OnCallbacks(tex, onCapturedCallback, null, null);
            }, width, height);
        }
        else
        {
            Debug.LogWarning("Require this camera to be registered with method RegisterCaptureCamera!");
        }
    }

    /// <summary>
    /// Capture image with the view of the target camera. Return a Sprite in the callback.
    /// </summary>
    /// <param name="camera">Target Camera.</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
	/// <param name="targetWidth">The width for the texture.</param>
	/// <param name="targetHeight">The height for the texture.</param>
    public void CaptureWithCamera_Sprite(Camera camera, Action<Sprite> onCapturedCallback = null, int targetWidth = 0, int targetHeight = 0)
    {
        UpdateDebugText(camera.name + " rect: " + camera.pixelWidth + " x " + camera.pixelHeight);

        RegisterRenderCamera(camera);
        CameraRenderBase cameraBase = _SetupCameraRenderBase(camera);

        if (cameraBase != null)
        {
            cameraBase.SetOnCaptureCallback((Texture2D tex) =>
            {
                _OnCallbacks(tex, null, onCapturedCallback, null);
            }, targetWidth, targetHeight);
        }
        else
        {
            Debug.LogWarning("Require this camera to be registered with method RegisterCaptureCamera!");
        }
    }

    /// <summary>
    /// Capture image with the view of the target camera. Return a RenderTexture in the callback.
    /// </summary>
    /// <param name="camera">Target Camera.</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
	/// <param name="targetWidth">The width for the texture.</param>
	/// <param name="targetHeight">The height for the texture.</param>
    public void CaptureWithCamera_RenderTexture(Camera camera, Action<RenderTexture> onCapturedCallback = null, int targetWidth = 0, int targetHeight = 0)
    {
        UpdateDebugText(camera.name + " rect: " + camera.pixelWidth + " x " + camera.pixelHeight);

        RegisterRenderCamera(camera);
        CameraRenderBase cameraBase = _SetupCameraRenderBase(camera);

        if (cameraBase != null)
        {
            cameraBase.SetOnCaptureCallback((Texture2D tex) =>
            {
                _OnCallbacks(tex, null, null, onCapturedCallback);
            }, targetWidth, targetHeight);
        }
        else
        {
            Debug.LogWarning("Require this camera to be registered with method RegisterCaptureCamera!");
        }
    }

    /// <summary>
    /// Capture image with the view of the target camera. Return a Texture2D in the callback.
    /// </summary>
    /// <param name="camera">Target Camera.</param>
    /// <param name="scale">Apply this scale to capture image. (Scale the image size down to the minimum of 0.1X and up to 4X)</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    public void CaptureWithCamera(Camera camera, float scale, Action<Texture2D> onCapturedCallback = null)
    {
        UpdateDebugText(camera.name + " rect: " + camera.pixelWidth + " x " + camera.pixelHeight);

        RegisterRenderCamera(camera);
        CameraRenderBase cameraBase = _SetupCameraRenderBase(camera);

        if (cameraBase != null)
        {
            cameraBase.SetOnCaptureCallback((Texture2D tex) =>
            {
                _OnCallbacks(tex, onCapturedCallback, null, null);
            }, scale);
        }
        else
        {
            Debug.LogWarning("Require this camera to be registered with method RegisterCaptureCamera!");
        }
    }

    /// <summary>
    /// Capture image with the view of the target camera. Return a Sprite in the callback.
    /// </summary>
    /// <param name="camera">Target Camera.</param>
    /// <param name="scale">Apply this scale to capture image. (Scale the image size down to the minimum of 0.1X and up to 4X)</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    public void CaptureWithCamera_Sprite(Camera camera, float scale, Action<Sprite> onCapturedCallback = null)
    {
        UpdateDebugText(camera.name + " rect: " + camera.pixelWidth + " x " + camera.pixelHeight);

        RegisterRenderCamera(camera);
        CameraRenderBase cameraBase = _SetupCameraRenderBase(camera);

        if (cameraBase != null)
        {
            cameraBase.SetOnCaptureCallback((Texture2D tex) =>
            {
                _OnCallbacks(tex, null, onCapturedCallback, null);
            }, scale);
        }
        else
        {
            Debug.LogWarning("Require this camera to be registered with method RegisterCaptureCamera!");
        }
    }

    /// <summary>
    /// Capture image with the view of the target camera. Return a RenderTexture in the callback.
    /// </summary>
    /// <param name="camera">Target Camera.</param>
    /// <param name="scale">Apply this scale to capture image. (Scale the image size down to the minimum of 0.1X and up to 4X)</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    public void CaptureWithCamera_RenderTexture(Camera camera, float scale, Action<RenderTexture> onCapturedCallback = null)
    {
        UpdateDebugText(camera.name + " rect: " + camera.pixelWidth + " x " + camera.pixelHeight);

        RegisterRenderCamera(camera);
        CameraRenderBase cameraBase = _SetupCameraRenderBase(camera);

        if (cameraBase != null)
        {
            cameraBase.SetOnCaptureCallback((Texture2D tex) =>
            {
                _OnCallbacks(tex, null, null, onCapturedCallback);
            }, scale);
        }
        else
        {
            Debug.LogWarning("Require this camera to be registered with method RegisterCaptureCamera!");
        }
    }

    /// <summary>
    /// Capture image with the view of the target camera. Return a RenderTexture in the callback.
    /// </summary>
    /// <param name="camera">Target Camera.</param>
    /// <param name="onCapturedCallback">On captured callback.</param>
    /// <param name="width">The width that forcing the image to fill in.</param>
    /// <param name="height">The height that forcing the image to fill in.</param>
    /// <param name="blitToNewTexture">Create and return a new RenderTexture, so that will not be removed by the Clear method.</param>
    public void CaptureRenderTextureWithCamera(Camera camera, Action<RenderTexture> onCapturedCallback = null, int width = 0, int height = 0, bool blitToNewTexture = true)
	{
		UpdateDebugText(camera.name + " rect: " + camera.pixelWidth + " x " + camera.pixelHeight);

		RegisterRenderCamera(camera);
        CameraRenderBase cameraBase = _SetupCameraRenderBase(camera);

		if(cameraBase != null)
		{
            cameraBase.SetOnCaptureCallback((RenderTexture rTex) =>
            {
                _renderTexture = rTex;
                if (onCapturedCallback != null) onCapturedCallback(_renderTexture);
                if (m_MainOnCaptured != null) m_MainOnCaptured.Invoke();
            }, width, height, blitToNewTexture);
		}
		else
		{
			Debug.LogWarning("Require this camera to be registered with method RegisterCaptureCamera!");
		}
	}

    /// <summary>
    /// Capture image with the view of the target camera. Return a RenderTexture in the callback.
    /// </summary>
    /// <param name="camera">Target Camera.</param>
    /// <param name="scale">Apply this scale to capture image. (Scale the image size down to the minimum of 0.1X and up to 4X)</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    /// <param name="blitToNewTexture">Create and return a new RenderTexture, so that will not be removed by the Clear method.</param>
    public void CaptureRenderTextureWithCamera(Camera camera, float scale, Action<RenderTexture> onCapturedCallback = null, bool blitToNewTexture = true)
    {
        UpdateDebugText(camera.name + " rect: " + camera.pixelWidth + " x " + camera.pixelHeight);

        RegisterRenderCamera(camera);
        CameraRenderBase cameraBase = _SetupCameraRenderBase(camera);

        if (cameraBase != null)
        {
            cameraBase.SetOnCaptureCallback((RenderTexture rTex) =>
            {
                _renderTexture = rTex;
                if (onCapturedCallback != null) onCapturedCallback(_renderTexture);
                if (m_MainOnCaptured != null) m_MainOnCaptured.Invoke();
            }, scale, blitToNewTexture);
        }
        else
        {
            Debug.LogWarning("Require this camera to be registered with method RegisterCaptureCamera!");
        }
    }

    private CameraRenderBase _SetupCameraRenderBase(Camera targetCamera)
    {
        CameraRenderBase cameraBase = targetCamera.gameObject.GetComponent<CameraRenderBase>();
        cameraBase.m_AntiAliasingLevel = m_AntiAliasingLevel;
        return cameraBase;
    }

    /// <summary>
    /// Get the currently stored Texture2D.
    /// (If you did not take any screenshot before, this will return a null)
    /// </summary>
    public Texture2D GetCurrentTexture()
	{
		return _texture2D;
	}

	/// <summary>
	/// Return the sprite that converts from the current stored texture2D.
	/// (If you did not take any screenshot before, this will return a null)
	/// </summary>
	public Sprite GetCurrentSprite()
	{
        if (_cachedSprite && _cachedSprite.texture == _texture2D) return _cachedSprite;
        _cachedSprite = ToSprite(GetCurrentTexture());
        Resources.UnloadUnusedAssets();
        return _cachedSprite;
	}
    private Sprite _cachedSprite = null;

    /// <summary>
    /// Get the currently stored RenderTexture.
    /// (If you did not take any screenshot before, this will return a null)
    /// </summary>
    public RenderTexture GetCurrentRenderTexture()
	{
        if (_renderTexture && _cachedTexture2D == _texture2D) return _renderTexture;
        _cachedTexture2D = _texture2D;
        _renderTexture = new RenderTexture(_texture2D.width, _texture2D.height, 24);
        _renderTexture.antiAliasing = m_AntiAliasingLevel;
        Graphics.Blit(_texture2D, _renderTexture);
        Resources.UnloadUnusedAssets();
        return _renderTexture;
	}
    private Texture2D _cachedTexture2D = null;

    private void _OnCallbacks(Texture2D texture2D, Action<Texture2D> onCapturedTexture2D, Action<Sprite> onCapturedSprite, Action<RenderTexture> onCapturedRenderTexture)
    {
        _texture2D = texture2D;

        if (onCapturedTexture2D != null) onCapturedTexture2D(GetCurrentTexture());
        if (onCapturedSprite != null) onCapturedSprite(GetCurrentSprite());
        if (onCapturedRenderTexture != null) onCapturedRenderTexture(GetCurrentRenderTexture());
        if (m_MainOnCaptured != null) m_MainOnCaptured.Invoke();

        // Clear all assets(including textures, sprites) that are not used (not referenced or selected).
        Resources.UnloadUnusedAssets();
    }

	private void _ProceedReadPixels(Rect targetRect, Action<Texture2D> onCapturedTexture2D, Action<Sprite> onCapturedSprite, Action<RenderTexture> onCapturedRenderTexture)
	{
		//size correction for target rect
		if(targetRect.width > Screen.width) targetRect.width = Screen.width;
		if(targetRect.height > Screen.height) targetRect.height = Screen.height;

		_texture2D = new Texture2D((int)targetRect.width, (int)targetRect.height, TextureFormat.RGB24, false);
		Rect rect = new Rect(targetRect.position.x-targetRect.width/2, targetRect.position.y-targetRect.height/2, targetRect.width, targetRect.height);
		_texture2D.ReadPixels(rect, 0, 0);
		_texture2D.Apply();

        _isBeingCaptureScreen = false;

        _OnCallbacks(_texture2D, onCapturedTexture2D, onCapturedSprite, onCapturedRenderTexture);

		UpdateDebugText("Capture screenPosition: (" + targetRect.position.x + ", " + targetRect.position.y + ") | imageSize: (" + targetRect.width + ", " + targetRect.height + ")");
	}

	private IEnumerator _TakeFullscreen(Action<Texture2D> onCapturedTexture2D, Action<Sprite> onCapturedSprite, Action<RenderTexture> onCapturedRenderTexture)
	{
		//ensure to Read Pixels inside drawing frame
		yield return new WaitForEndOfFrame();
		Rect targetRect = new Rect(Screen.width/2, Screen.height/2, Screen.width, Screen.height);
		_ProceedReadPixels(targetRect, onCapturedTexture2D, onCapturedSprite, onCapturedRenderTexture);
	}

	private IEnumerator _ReadPixelWithRect(Rect targetRect, Action<Texture2D> onCapturedTexture2D, Action<Sprite> onCapturedSprite, Action<RenderTexture> onCapturedRenderTexture, bool constraintTargetRectWithScreen = false)
	{
		// Ensure to Read Pixels inside drawing frame
		yield return new WaitForEndOfFrame();
        if (constraintTargetRectWithScreen) targetRect = _ConstraintRectWithScreen(targetRect);
        _ProceedReadPixels(targetRect, onCapturedTexture2D, onCapturedSprite, onCapturedRenderTexture);
	}

    /// <summary>
    /// Attach a camera render script on the camera to capture image from camera.
    /// </summary>
    /// <param name="camera">Target Camera.</param>
    public void RegisterRenderCamera(Camera camera)
	{
        switch(m_RenderMode)
        {
            case RenderMode.OnRenderImage:
                if (camera != null && camera.gameObject.GetComponent<CameraOnUpdateRender>() != null)
                {
                    camera.gameObject.GetComponent<CameraOnUpdateRender>().Clear();
                }
                if (camera != null && camera.gameObject.GetComponent<CameraOnRender>() == null)
                {
                    camera.gameObject.AddComponent<CameraOnRender>();
                }
                break;

            case RenderMode.OnUpdateRender:
                if (camera != null && camera.gameObject.GetComponent<CameraOnRender>() != null)
                {
                    camera.gameObject.GetComponent<CameraOnRender>().Clear();
                }
                if (camera != null && camera.gameObject.GetComponent<CameraOnUpdateRender>() == null)
                {
                    camera.gameObject.AddComponent<CameraOnUpdateRender>();
                }
                break;
        }
	}

	/// <summary>
	/// Clear the instance of camera render and remove the script.
	/// </summary>
	/// <param name="camera">Target Camera.</param>
	public void UnRegisterRenderCamera(Camera camera)
	{
		if(camera != null && camera.gameObject.GetComponent<CameraOnRender>() != null)
		{
			camera.gameObject.GetComponent<CameraOnRender>().Clear();
		}
        if (camera != null && camera.gameObject.GetComponent<CameraOnUpdateRender>() != null)
        {
            camera.gameObject.GetComponent<CameraOnUpdateRender>().Clear();
        }
    }

	/// <summary>
	/// Clear the instance of camera render on all cameras, and remove the script.
	/// </summary>
	public void UnRegisterAllRenderCameras()
	{
		Camera[] cameras = Camera.allCameras;
		if(cameras != null)
		{
			foreach(Camera cam in cameras)
			{
				UnRegisterRenderCamera(cam);
			}
		}
	}


    #region ----- Static Methods -----
    /// <summary>
    /// Set the RenderMode for capturing image using camera. (For camera capture methods Only)
    /// </summary>
    public static void SetRenderMode(RenderMode renderMode)
    {
        Instance.m_RenderMode = renderMode;
    }

    /// <summary>
    /// [Camera capture methods Only] The anti-aliasing level for the resulting texture, 
    /// the greater value results in the edges of the image look smoother. Available value: 1(OFF), 2, 4, 8
    /// (The greater value of anti-aliasing and resolution together may cause a wrong output of the texture, please adjust the values as need.)
    /// </summary>
    public static int AntiAliasingLevel
    {
        get
        {
            return Instance.m_AntiAliasingLevel;
        }
        set
        {
            Instance.m_AntiAliasingLevel = value;
        }
    }

    /// <summary>
    /// Get the currently stored Texture2D.
    /// (If you did not take any screenshot before, this will return a null)
    /// </summary>
    public static Texture2D CurrentTexture
	{
		get{
			return Instance.GetCurrentTexture();
		}
	}

	/// <summary>
	/// Return the sprite that converts from the current texture2D.
	/// (If you did not take any screenshot before, this will return a null)
	/// </summary>
	public static Sprite CurrentSprite
	{
		get{
			return Instance.GetCurrentSprite();
		}
	}

	/// <summary>
	/// Get the currently stored RenderTexture.
	/// (If you did not take any screenshot before, this will return a null)
	/// </summary>
	public static RenderTexture CurrentRenderTexture
	{
		get{
			return Instance.GetCurrentRenderTexture();
		}
	}

    /// <summary>
    /// Set the main onCaptured callback to be invoked by all capture methods.
    /// </summary>
    /// <param name="mainOnCaptured">The callback to be fired at each capture.</param>
    public static void iSetMainOnCapturedCallback(Action mainOnCaptured)
	{
		Instance.SetMainOnCapturedCallback(mainOnCaptured);
	}

    /// <summary>
    /// Set the main onCaptured callback for receiving all images from all capture methods. Return a Texture2D in the callback, at each capture.
    /// </summary>
    /// <param name="mainOnCaptured">The callback to be fired at each capture.</param>
    public static void iSetMainOnCapturedCallback(Action<Texture2D> mainOnCaptured)
	{
		Instance.SetMainOnCapturedCallback(mainOnCaptured);
	}

    /// <summary>
    /// Set the main onCaptured callback for receiving all images from all capture methods. Return a Sprite in the callback, at each capture.
    /// </summary>
    /// <param name="mainOnCaptured">The callback to be fired at each capture.</param>
	public static void iSetMainOnCapturedCallback(Action<Sprite> mainOnCaptured)
	{
		Instance.SetMainOnCapturedCallback(mainOnCaptured);
	}

    /// <summary>
    /// Set the main onCaptured callback for receiving all images from all capture methods. Return a RenderTexture in the callback, at each capture.
    /// </summary>
    /// <param name="mainOnCaptured">The callback to be fired at each capture.</param>
	public static void iSetMainOnCapturedCallback(Action<RenderTexture> mainOnCaptured)
	{
		Instance.SetMainOnCapturedCallback(mainOnCaptured);
    }

    /// <summary>
    /// Capture the full screen, return a Texture2D in the callback.
    /// </summary>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    public static void iCaptureScreen(Action<Texture2D> onCapturedCallback = null)
    {
        Instance.CaptureScreen(onCapturedCallback);
    }

    /// <summary>
    /// Capture the full screen, return a Sprite in the callback.
    /// </summary>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    public static void iCaptureScreen_Sprite(Action<Sprite> onCapturedCallback = null)
    {
        Instance.CaptureScreen_Sprite(onCapturedCallback);
    }

    /// <summary>
    /// Capture the full screen, return a RenderTexture in the callback.
    /// </summary>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    public static void iCaptureScreen_RenderTexture(Action<RenderTexture> onCapturedCallback = null)
    {
        Instance.CaptureScreen_RenderTexture(onCapturedCallback);
    }

    /// <summary>
    /// Capture a portion of the screen at specific screen position, return a Texture2D in the callback.
    /// </summary>
    /// <param name="screenPosition">Screen position.</param>
    /// <param name="imageSize">The target image size.</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    public static void iCapture(Vector2 screenPosition, Vector2 imageSize, Action<Texture2D> onCapturedCallback = null)
    {
        Instance.Capture(screenPosition, imageSize, onCapturedCallback);
    }

    /// <summary>
    /// Capture a portion of the screen at specific screen position, return a Sprite in the callback.
    /// </summary>
    /// <param name="screenPosition">Screen position.</param>
    /// <param name="imageSize">The target image size.</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    public static void iCapture_Sprite(Vector2 screenPosition, Vector2 imageSize, Action<Sprite> onCapturedCallback = null)
    {
        Instance.Capture_Sprite(screenPosition, imageSize, onCapturedCallback);
    }

    /// <summary>
    /// Capture a portion of the screen at specific screen position, return a RenderTexture in the callback.
    /// </summary>
    /// <param name="screenPosition">Screen position.</param>
    /// <param name="imageSize">The target image size.</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    public static void iCapture_RenderTexture(Vector2 screenPosition, Vector2 imageSize, Action<RenderTexture> onCapturedCallback = null)
    {
        Instance.Capture_RenderTexture(screenPosition, imageSize, onCapturedCallback);
    }

    /// <summary>
    /// Capture image with the view of the target camera. Return a Texture2D in the callback.
    /// </summary>
    /// <param name="camera">Target Camera.</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    /// <param name="width">The width that forcing the image to fill in.</param>
    /// <param name="height">The height that forcing the image to fill in.</param>
    public static void iCaptureWithCamera(Camera camera, Action<Texture2D> onCapturedCallback = null, int width = 0, int height = 0)
    {
        Instance.CaptureWithCamera(camera, onCapturedCallback, width, height);
    }

    /// <summary>
    /// Capture image with the view of the target camera. Return a Sprite in the callback.
    /// </summary>
    /// <param name="camera">Target Camera.</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    /// <param name="width">The width that forcing the image to fill in.</param>
    /// <param name="height">The height that forcing the image to fill in.</param>
    public static void iCaptureWithCamera_Sprite(Camera camera, Action<Sprite> onCapturedCallback = null, int width = 0, int height = 0)
    {
        Instance.CaptureWithCamera_Sprite(camera, onCapturedCallback, width, height);
    }

    /// <summary>
    /// Capture image with the view of the target camera. Return a RenderTexture in the callback.
    /// </summary>
    /// <param name="camera">Target Camera.</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    /// <param name="width">The width that forcing the image to fill in.</param>
    /// <param name="height">The height that forcing the image to fill in.</param>
    public static void iCaptureWithCamera_RenderTexture(Camera camera, Action<RenderTexture> onCapturedCallback = null, int width = 0, int height = 0)
    {
        Instance.CaptureWithCamera_RenderTexture(camera, onCapturedCallback, width, height);
    }

    /// <summary>
    /// Capture image with the view of the target camera. Return a Texture2D in the callback.
    /// </summary>
    /// <param name="camera">Target Camera.</param>
    /// <param name="scale">The value uses to scale the image.</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    public static void iCaptureWithCamera(Camera camera, float scale, Action<Texture2D> onCapturedCallback = null)
    {
        Instance.CaptureWithCamera(camera, scale, onCapturedCallback);
    }

    /// <summary>
    /// Capture image with the view of the target camera. Return a Sprite in the callback.
    /// </summary>
    /// <param name="camera">Target Camera.</param>
    /// <param name="scale">The value uses to scale the image.</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    public static void iCaptureWithCamera_Sprite(Camera camera, float scale, Action<Sprite> onCapturedCallback = null)
    {
        Instance.CaptureWithCamera_Sprite(camera, scale, onCapturedCallback);
    }

    /// <summary>
    /// Capture image with the view of the target camera. Return a RenderTexture in the callback.
    /// </summary>
    /// <param name="camera">Target Camera.</param>
    /// <param name="scale">The value uses to scale the image.</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    public static void iCaptureWithCamera_RenderTexture(Camera camera, float scale, Action<RenderTexture> onCapturedCallback = null)
    {
        Instance.CaptureWithCamera_RenderTexture(camera, scale, onCapturedCallback);
    }

    /// <summary>
    /// Capture image with the view of the target camera. Return a RenderTexture in the callback.
    /// </summary>
    /// <param name="camera">Target Camera.</param>
    /// <param name="onCapturedCallback">On Captured Callback.</param>
    /// <param name="width">The width that forcing the image to fill in.</param>
    /// <param name="height">The height that forcing the image to fill in.</param>
    /// <param name="blitToNewTexture">Create and return a new RenderTexture, so that will not be removed by the Clear method.</param>
    public static void iCaptureRenderTextureWithCamera(Camera camera, Action<RenderTexture> onCapturedCallback = null, int width = 0, int height = 0, bool blitToNewTexture = true)
    {
        Instance.CaptureRenderTextureWithCamera(camera, onCapturedCallback, width, height, blitToNewTexture);
    }

    /// <summary>
    /// Capture image with the view of the target camera. Return a RenderTexture in the callback.
    /// </summary>
    /// <param name="camera">Target Camera.</param>
    /// <param name="scale">The value uses to scale the image. (Scale the image size down to the minimum of 0.1X and up to 4X)</param>
    /// <param name="onCapturedCallback">On captured callback, return the captured RenderTexture.</param>
    /// <param name="blitToNewTexture">Create and return a new RenderTexture, so that will not be removed by the Clear method.</param>
    public static void iCaptureRenderTextureWithCamera(Camera camera, float scale, Action<RenderTexture> onCapturedCallback = null, bool blitToNewTexture = true)
    {
        Instance.CaptureRenderTextureWithCamera(camera, scale, onCapturedCallback, blitToNewTexture);
    }

    /// <summary>
    /// Attach a camera render script on the camera to capture image from camera.
    /// </summary>
    /// <param name="camera">Target Camera.</param>
    public static void iRegisterRenderCamera(Camera camera)
	{
		Instance.RegisterRenderCamera(camera);
	}

	/// <summary>
	/// Clear the instance of camera render and remove the script.
	/// </summary>
	/// <param name="camera">Target Camera.</param>
	public static void iUnRegisterRenderCamera(Camera camera)
	{
		Instance.UnRegisterRenderCamera(camera);
	}

	/// <summary>
	/// Clear the instance of camera render on all cameras, and remove the script.
	/// </summary>
	public static void iUnRegisterAllRenderCameras()
	{
		Instance.UnRegisterAllRenderCameras();
	}

    /// <summary>
    /// Clear the instance of ScreenshotHelper:
    /// Destroy the stored textures, remove callbacks, remove script from camera, and unload all assets that are not used. 
    /// </summary>
	public static void iClear(bool clearCallback = true, bool clearTextures = true)
	{
        Instance.Clear(clearCallback, clearTextures);
	}
	#endregion


	#region ----- Others -----
	public void UpdateDebugText(string text)
	{
		if(m_DebugText != null)
		{
			Debug.Log(text);
			m_DebugText.text = text;
		}
	}

    /// <summary>
    /// Create a Sprite with the provided Texture2D.
    /// </summary>
	public static Sprite ToSprite(Texture2D texture)
	{
		if(texture == null) return null;

		Vector2 pivot = new Vector2(0.5f, 0.5f);
		float pixelPerUnit = 100;
		return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot, pixelPerUnit);
	}

    /// <summary>
    /// Destroy the target Texture2D. 
    /// Optional to create a 1x1 pixel texture to replace it (for preventing the warnings in some cases, Metal: Fragment shader missing texture...).
    /// </summary>
    public static void ClearTexture2D(ref Texture2D texture, bool replaceWithMinimumTexture = true)
    {
        Destroy(texture);
        if(replaceWithMinimumTexture) texture = new Texture2D(1, 1);
    }

    /// <summary>
    /// Destroy the target RenderTexture. 
    /// Optional to create a 1x1 pixel texture to replace it (for preventing the warnings in some cases, Metal: Fragment shader missing texture...).
    /// </summary>
    public static void ClearRenderTexture(ref RenderTexture texture, bool replaceWithMinimumTexture = true)
    {
        Destroy(texture);
        if(replaceWithMinimumTexture) texture = new RenderTexture(1, 1, 24);
    }
    #endregion

}
