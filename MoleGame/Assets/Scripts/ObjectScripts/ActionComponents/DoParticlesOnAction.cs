using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
public class DoParticlesOnAction : IActionComponent
{
    [SerializedDictionary("Particlename", "Count")]
    public SerializedDictionary<string, int> particles;


    public override void DoAction()
    {
        foreach (var particle in particles)
        {
            ParticleManager.Instance.DoParticles(particle.Key, transform.position, Vector3.one, particle.Value);
        }
    }
}
