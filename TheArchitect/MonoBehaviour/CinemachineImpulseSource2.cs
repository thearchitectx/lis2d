using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineImpulseSource2 : CinemachineImpulseSource
{
    public void GenerateImpulseAnimationEvent(float force)
    {
        GenerateImpulse(force);
    }
}
