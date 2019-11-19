using UnityEngine;
using System.Collections;

public class Reward : MonoBehaviour
{
    Transform trnsfrm;
    MeshRenderer mRenderer;
    Collider col;
    ParticleSystem  prtcSyst;

    public void Initialize ()
    {
        trnsfrm = transform;
        mRenderer = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
        prtcSyst = GetComponentInChildren<ParticleSystem>();
    }

    public void DestroyReward()
    {
        prtcSyst.enableEmission = false;
        mRenderer.enabled = false;
        col.enabled = false;
    }
}
