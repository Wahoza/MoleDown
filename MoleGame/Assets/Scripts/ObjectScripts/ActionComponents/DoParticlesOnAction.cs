using System.Collections.Generic;
using UnityEngine;

public class DoParticlesOnAction : IActionComponent
{
    [SerializeField] List<string> particles;


    public override void DoAction()
    {
        foreach (var particle in particles)
        {
            ParticleManager.Instance.DoParticles(particle, transform.position);
        }
    }
}
