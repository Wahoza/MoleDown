using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationEffect : MonoBehaviour
{
    [SerializeField] List<Transform> transforms;
    [SerializeField] public Vector3 rotation;

    private IEnumerator Start()
    {
        while (true)
        {
            foreach (Transform t in transforms)
            {
                t.Rotate(rotation * Time.deltaTime);
            }
            yield return null;
        }
    }
}
