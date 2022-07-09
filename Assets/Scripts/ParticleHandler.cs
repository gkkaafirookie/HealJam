using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
public class ParticleHandler : MonoBehaviour
{
    private ParticleSystem particle;
    private ParticleSystemRenderer particleRenderer;
    private Material particleMaterial;

    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        particleRenderer = GetComponent<ParticleSystemRenderer>();
        particleMaterial = particleRenderer.material;
    }

    public void PlayParticleAtPosition(Vector3 pos)
    {
        var pmain = particle.main;

        transform.position = pos;
        particle.Play();
        particleMaterial.DOFade(1, 0);
        particleMaterial.DOFade(0, pmain.startLifetime.constant).SetEase(Ease.InExpo);
    }
}
