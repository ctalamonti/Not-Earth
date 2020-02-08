/// <summary>
/// Created by SwanDEV 2019
/// </summary>

using UnityEngine;

public class CameraOnUpdateRender : CameraRenderBase
{
    private Camera rCamera = null;

    private void Update()
    {
        OnUpdateRender();
    }

    private void OnUpdateRender()
    {
        if (!m_ToCapture)
        {
            return;
        }
        m_ToCapture = false;

        if (rCamera == null) rCamera = GetComponent<Camera>();

        m_RenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        m_RenderTexture.antiAliasing = m_AntiAliasingLevel;
        rCamera.targetTexture = m_RenderTexture;
        rCamera.Render();
        rCamera.targetTexture = null;

        Vector2 targetSize = (_targetWidth > 0 && _targetHeight > 0) ? new Vector2(_targetWidth, _targetHeight) : new Vector2((int)(rCamera.pixelWidth * _scale), (int)(rCamera.pixelHeight * _scale));
        m_RenderTexture = _CutOutRenderTextureWithCameraViewport(m_RenderTexture, rCamera, targetSize);

        if (_onCaptureCallback != null)
        {
            _onCaptureCallback(GetLastTexture2D());
        }

        if (_onCaptureCallbackRTex != null)
        {
            if (_blitToNewTexture)
            {
                RenderTexture rTex = new RenderTexture(m_RenderTexture.width, m_RenderTexture.height, 24);
                rTex.antiAliasing = m_AntiAliasingLevel;
                Graphics.Blit(GetLastRenderTexture(), rTex);
                _onCaptureCallbackRTex(rTex);
            }
            else
            {
                _onCaptureCallbackRTex(GetLastRenderTexture());
            }
            _onCaptureCallbackRTex = null;
        }

        if (_blitToNewTexture || _onCaptureCallback != null)
        {
            _onCaptureCallback = null;
            if (m_RenderTexture) Destroy(m_RenderTexture);
        }
    }
}
