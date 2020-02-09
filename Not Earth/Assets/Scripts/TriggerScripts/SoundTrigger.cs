using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger : TriggerBase
{
    private bool sound = true;
    protected override void DoAction()
    {
        if (sound)
        {
            sm.SetClip(soundIndex);
            sm.PlaySound(location.position);
            sound = false; 

        }
       
    }
    public int soundIndex;
    public SoundManager sm;
    public Transform location;
}
