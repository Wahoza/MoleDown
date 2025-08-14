using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPetalEffect : MonoBehaviour
{
    [SerializeField] List<Transform> petals;
    [SerializeField] Vector3 rotation;

    private IEnumerator Start()
    {
        while (true)
        {
            foreach (Transform t in petals)
            {
                t.Rotate(rotation * Time.deltaTime);
            }
            yield return null;
        }
    }
}
