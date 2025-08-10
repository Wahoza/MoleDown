using System.Collections;
using UnityEngine;

public class DestroyAtYLevelComponent : MonoBehaviour
{
    [SerializeField] float minYLevel;
    IEnumerator Start()
    {
        while(transform.position.y > minYLevel)
        {
            yield return null;
        }

        Destroy(gameObject);
    }
}
