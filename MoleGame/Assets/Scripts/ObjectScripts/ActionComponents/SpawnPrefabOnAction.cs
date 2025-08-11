using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefabOnAction : IActionComponent
{
    [SerializeField] List<GameObject> prefabList;

    public override void DoAction()
    {
        foreach (GameObject prefab in prefabList)
        {
            var go = Instantiate(prefab);
            go.transform.position = transform.position;
        }
    }
}
