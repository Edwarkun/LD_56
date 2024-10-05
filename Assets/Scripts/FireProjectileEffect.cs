using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectileEffect : MonoBehaviour
{
    public ParticleSystem fireParticles;
    private void OnDestroy()
    {
        var value = fireParticles.main;
        value.loop = false;
        fireParticles.Stop();
        fireParticles.transform.parent = null;
    }
}
