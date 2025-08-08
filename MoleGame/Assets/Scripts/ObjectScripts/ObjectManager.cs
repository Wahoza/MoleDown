using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance { get; private set; }
    private List<GameObject> objects = new();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void RegisterObject(GameObject obj)
    {
        objects.Add(obj);
    }

    public void DestroyAllObjects()
    {
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }

        objects.Clear();
    }
}
