using UnityEngine;
using System.Collections;
public class BillboardObject : MonoBehaviour
{
    IEnumerator Start()
    {
        while (true)
        {
            transform.forward = Camera.main.transform.position - transform.position;
            yield return null;
        }
    }
}
