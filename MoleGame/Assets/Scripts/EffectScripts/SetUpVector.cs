using System.Collections;
using UnityEngine;

public class SetUpVector : MonoBehaviour
{
    [SerializeField] Vector3 upVector;
    [SerializeField] float waitDuration;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(waitDuration);
        transform.up = upVector;
    }
}
