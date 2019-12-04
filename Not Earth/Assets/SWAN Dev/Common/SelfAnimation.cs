﻿//Created by SwanDEV 2017

using UnityEngine;
using UnityEngine.Events;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SelfAnimation : MonoBehaviour
{
	public SDemoControl m_Control = null;

	public enum SelfAnimType{
		None = 0,
		Move,
		Rotate,
		Scale,
		Move_RelativePosition,
	}

	public SelfAnimType m_SelfAnimType = SelfAnimType.None;
	public SDemoAnimation.LoopType loop = SDemoAnimation.LoopType.None;

	//public Vector3 initValue;
	public Vector3 fromValue;
	public Vector3 toValue;

	public float time = 0.5f;
	public float delay = 0f;
	public float delay_Revert = 0f;

	public bool executeAtStart = true;
	public bool enableInitValue = false;

	public bool destroyOnComplete = false;

	public UnityEvent onComplete;

	//private Vector3 _originRotation;
	private Vector3 _originPosition;

	void Awake()
	{
		_originPosition = transform.localPosition;
        //_originRotation = transform.localEulerAngles;
        if (executeAtStart) StartAnimation();
	}

	void OnEnable()
	{
        if (m_Control != null) m_Control.m_State = SDemoControl.State.Playing;
	}

	void OnDisable()
	{
		if(m_Control != null) m_Control.m_State = SDemoControl.State.Paused;
	}

	void OnComplete()
	{
		if(onComplete != null) onComplete.Invoke();
		if(destroyOnComplete) Destroy(gameObject);
	}

	void OnDestroy()
	{
		if(m_Control != null) m_Control.m_State = SDemoControl.State.Kill;
	}

	private bool _isOdd = false;
	public void SwitchAnimation()
	{
		_isOdd = !_isOdd;
		if(_isOdd)
		{
			StartAnimation(delay);
		}
		else
		{
			StartAnimationRevert(delay_Revert);
		}
	}
	public void SwitchAnimationRevert()
	{
		_isOdd = !_isOdd;
		if(!_isOdd)
		{
			StartAnimation(delay);
		}
		else
		{
			StartAnimationRevert(delay_Revert);
		}
	}

	public void StartAnimation(float inDelay = 0f)
	{
		if(m_Control != null) m_Control.m_State = SDemoControl.State.Kill; // Kill the current tweening if existed

		switch(m_SelfAnimType)
		{
		case SelfAnimType.Move:
			if(enableInitValue) gameObject.transform.localPosition = fromValue;
			m_Control = SDemoAnimation.Instance.Move(gameObject, fromValue, toValue, time, delay, loop, OnComplete);
			break;

		case SelfAnimType.Rotate:
			if(enableInitValue) gameObject.transform.localEulerAngles = fromValue;
			m_Control = SDemoAnimation.Instance.Rotate(gameObject, fromValue, toValue, time, delay, loop, OnComplete);
			break;

		case SelfAnimType.Scale:
			if(enableInitValue) gameObject.transform.localScale = fromValue;
			m_Control = SDemoAnimation.Instance.Scale(gameObject, fromValue, toValue, time, delay, loop, OnComplete);
			break;

		case SelfAnimType.Move_RelativePosition:
			if(enableInitValue) gameObject.transform.localPosition = _originPosition + fromValue;
			m_Control = SDemoAnimation.Instance.Move(gameObject, _originPosition + fromValue, _originPosition + toValue, time, delay, loop, OnComplete);
			break;
		}
    }

	public void StartAnimationRevert(float inDelay = 0f)
	{
		if(m_Control != null) m_Control.m_State = SDemoControl.State.Kill; // Kill the current tweening if existed

        switch (m_SelfAnimType)
		{
		case SelfAnimType.Move:
			if(enableInitValue) gameObject.transform.localPosition = toValue;
			m_Control = SDemoAnimation.Instance.Move(gameObject, toValue, fromValue, time, inDelay, loop, OnComplete);
			break;

		case SelfAnimType.Rotate:
			if(enableInitValue) gameObject.transform.localEulerAngles = toValue;
			m_Control = SDemoAnimation.Instance.Rotate(gameObject, toValue, fromValue, time, inDelay, loop, OnComplete);
			break;

		case SelfAnimType.Scale:
			if(enableInitValue) gameObject.transform.localScale = toValue;
			m_Control = SDemoAnimation.Instance.Scale(gameObject, toValue, fromValue, time, inDelay, loop, OnComplete);
			break;

		case SelfAnimType.Move_RelativePosition:
			if(enableInitValue) gameObject.transform.localPosition = _originPosition + toValue;
			m_Control = SDemoAnimation.Instance.Move(gameObject, _originPosition + toValue, _originPosition + fromValue, time, inDelay, loop, OnComplete);
			break;
		}
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(SelfAnimation))]
public class SelfAnimationCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SelfAnimation anim = (SelfAnimation)target;

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Play"))
        {
            anim.StartAnimation();
        }
        if (GUILayout.Button("Reverse"))
        {
            anim.StartAnimationRevert();
        }
        if (GUILayout.Button("Pause"))
        {
            anim.m_Control.m_State = SDemoControl.State.Paused;
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
