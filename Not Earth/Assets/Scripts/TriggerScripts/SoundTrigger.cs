using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger : TriggerBase
{
    protected override void DoAction()
    {
        sm.SetClip(soundIndex);
        sm.PlaySound(location.position);
    }
    public int soundIndex;
    public SoundManager sm;
    public Transform location;
}
