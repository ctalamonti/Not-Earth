/// <summary>
/// Created by SwanDEV 2017
/// </summary>

using UnityEngine;

public class CameraOnRender : CameraRenderBase
{
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination);
        if (!m_ToCapture)
        {
            return;
        }
        m_ToCapture = false;

        if (_targetWidth > 0 && _targetHeight > 0)
        {
            m_RenderTexture = new RenderTexture(_targetWidth, _targetHeight, 24);
        }
        else if (m_RenderTexture.width != (int)(source.width * _scale) || m_RenderTexture.height != (int)(source.height * _scale))
        {
            m_RenderTexture = new RenderTexture((int)(source.width * _scale), (int)(source.height * _scale), 24);
        }
        Graphics.Blit(source, m_RenderTexture);

        if (_onCaptureCallback != null)
        {
            _onCaptureCallback(GetLastTexture2D());
        }

        if (_onCaptureCallbackRTex != null)
        {
            if (_blitToNewTexture)
            {
                RenderTexture rTex = new RenderTexture(m_RenderTexture.width, m_RenderTexture.height, 24);
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
