using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;

    [SerializeField] int particleObjectCount;
    [SerializeField] int particleEmitCount;

    [SerializedDictionary("ObjectPrefab", "Name")]
    public SerializedDictionary<GameObject, string> particleNameDictionary;

    private Dictionary<string, ParticleSetup> particles;

    private Object particleLock;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            GenerateParticles();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void GenerateParticles()
    {
        particles = new();
        foreach (var particle in particleNameDictionary)
        {
            ParticleSetup particleSetup = new ParticleSetup();
            particleSetup.list = new();
            particleSetup.currentIterator = new();

            for (int i = 0; i < particleObjectCount; i++)
            {
                var newParticle = Instantiate(particle.Key, transform);
                particleSetup.list.Add(newParticle.GetComponent<ParticleSystem>());
            }

            particles[particle.Value] = particleSetup;
        }
    }

    private struct ParticleSetup{
        public List<ParticleSystem> list;
        public int currentIterator;
    }

    public void DoParticles(string particleName, Vector3 position)
    {
        DoParticles(particleName, position, Vector3.one, particleEmitCount);   
    }
    public void DoParticles(string particleName, Vector3 position, Vector3 scale)
    {
        DoParticles(particleName, position, scale, particleEmitCount);
    }
    public void ClearParticles()
    {
        foreach (var particle in particles)
        {
            foreach(var go in particle.Value.list)
            {
                go.gameObject.SetActive(false);
            }
        }
    }
    public void DoParticles(string particleName, Vector3 position, Vector3 scale, int count)
    {
        lock (particles)
        {
            if (particles.ContainsKey(particleName))
            {
                var particleItem = particles[particleName];

                int iterator = particleItem.currentIterator;

                var particle = particleItem.list[iterator];

                particle.gameObject.SetActive(true);
                particle.transform.position = position;
                particle.Emit(count);

                iterator++;

                if (iterator >= particleItem.list.Count)
                {
                    iterator = 0;
                }

                particleItem.currentIterator = iterator;

                particles[particleName] = particleItem;
            }
        }
    }
}
