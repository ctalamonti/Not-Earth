/// <summary>
/// Created by SwanDEV 2019
/// </summary>

using UnityEngine;
using System;

public class CameraRenderBase : MonoBehaviour
{
    /// <summary>
    /// [Camera capture methods Only] The anti-aliasing level for the resulting texture, 
    /// the greater value results in the edges of the image look smoother. Available value: 1(OFF), 2, 4, 8
    /// (The greater value of anti-aliasing and resolution together may cause a wrong output of the texture, please adjust the values as need.)
    /// </summary>
    public int m_AntiAliasingLevel = 4;

    [HideInInspector] public RenderTexture m_RenderTexture;

    [HideInInspector] public bool m_ToCapture = true;
    protected Action<Texture2D> _onCaptureCallback;
    protected Action<RenderTexture> _onCaptureCallbackRTex;

    protected bool _blitToNewTexture = true;

    protected int _targetWidth = 0;
    protected int _targetHeight = 0;

    /// <summary>
    /// The scale.
    /// </summary>
    protected float _scale = 1f;

    private void Start()
    {
        m_RenderTexture = new RenderTexture(1, 1, 24);
        m_RenderTexture.antiAliasing = m_AntiAliasingLevel;
    }

    private void _Init()
    {
        if (m_RenderTexture == null && !(_targetWidth > 0 && _targetHeight > 0)) m_RenderTexture = new RenderTexture(1, 1, 24);
        m_ToCapture = true;
    }

    public void SetOnCaptureCallback(Action<Texture2D> onCaptured, float scale)
    {
        _onCaptureCallback = onCaptured;
        _scale = Mathf.Clamp(scale, 0.1f, 4f);
        _targetWidth = 0;
        _targetHeight = 0;
        _Init();
    }

    public void SetOnCaptureCallback(Action<Texture2D> onCaptured, int width = 0, int height = 0)
    {
        _onCaptureCallback = onCaptured;
        _scale = 1f;
        _targetWidth = width;
        _targetHeight = height;
        _Init();

    }

    public void SetOnCaptureCallback(Action<RenderTexture> onCaptured, float scale, bool blitToNewTexture = true)
    {
        _onCaptureCallbackRTex = onCaptured;
        _blitToNewTexture = blitToNewTexture;
        _scale = Mathf.Clamp(scale, 0.1f, 4f);
        _targetWidth = 0;
        _targetHeight = 0;
        _Init();
    }

    public void SetOnCaptureCallback(Action<RenderTexture> onCaptured, int width = 0, int height = 0, bool blitToNewTexture = true)
    {
        _onCaptureCallbackRTex = onCaptured;
        _blitToNewTexture = blitToNewTexture;
        _scale = 1f;
        _targetWidth = width;
        _targetHeight = height;
        _Init();
    }

    public Texture2D GetLastTexture2D()
    {
        return _RenderTextureToTexture2D(m_RenderTexture);
    }

    public RenderTexture GetLastRenderTexture()
    {
        return m_RenderTexture;
    }

    protected Texture2D _RenderTextureToTexture2D(RenderTexture source)
    {
        RenderTexture.active = source;
        Texture2D tex = new Texture2D(source.width, source.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        return tex;
    }

    protected RenderTexture _CutOutRenderTextureWithCameraViewport(RenderTexture source, Camera cam, Vector2 targetSize)
    {
        Rect rect = new Rect(Mathf.CeilToInt(cam.pixelRect.x), Mathf.CeilToInt(cam.pixelRect.y), cam.pixelRect.width, cam.pixelRect.height);

        RenderTexture.active = source;
        Texture2D tex = null;
        if (rect.width >= 1f && rect.height >= 1f && rect.x <= 0f && rect.y <= 0)
        {
            tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            tex.ReadPixels(rect, 0, 0);
            tex.Apply();
        }
        else
        {
            Texture2D texTemp = new Texture2D(source.width, source.height, TextureFormat.RGB24, false);
            texTemp.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
            //texTemp.Apply();
            tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            tex.SetPixels(texTemp.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height));
            tex.Apply();
        }
        RenderTexture.active = null;

        RenderTexture rt = new RenderTexture((int)targetSize.x, (int)targetSize.y, 24);
        Graphics.Blit(tex, rt);
        return rt;
    }

    public void Clear()
    {
        _onCaptureCallback = null;
        if (m_RenderTexture != null)
        {
            Destroy(m_RenderTexture);
        }
        Destroy(this);
    }
}
