using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class DCanvasScalerHandler : MonoBehaviour
{
    public Vector2 m_ReferenceResolution = new Vector2(1080, 1920);

	void Start ()
    {
        CanvasScaler cs = GetComponent<CanvasScaler>();
        if(cs)
        {
            if(Screen.width > Screen.height)
            {
                if(m_ReferenceResolution.x > m_ReferenceResolution.y) cs.referenceResolution = m_ReferenceResolution;
                else cs.referenceResolution = new Vector2(m_ReferenceResolution.y, m_ReferenceResolution.x);
            }
            else
            {
                if (m_ReferenceResolution.x > m_ReferenceResolution.y) cs.referenceResolution = new Vector2(m_ReferenceResolution.y, m_ReferenceResolution.x);
                else cs.referenceResolution = m_ReferenceResolution;
            }
        }
	}
}
