using System.Collections;
using UnityEngine;

public class FreezeUpVector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(UpRoutine());
    }

    private IEnumerator UpRoutine()
    {
        Vector3 initUP = transform.up;

        while (true)
        {
            transform.up = initUP;
            yield return null;
        }
    }
}
