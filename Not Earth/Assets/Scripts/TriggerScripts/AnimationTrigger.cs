using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrigger : TriggerBase
{
    public string animName;
    Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    protected override void DoAction()
    {   
        anim.Play(animName);
    }
}
