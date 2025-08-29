using System.Collections.Generic;
using UnityEngine;

public class CubeDecorating : MonoBehaviour
{
    [SerializeField] List<GameObject> decor;
    [SerializeField] float decorAmount;

    private GameObject[,,] placedObjects = new GameObject[16, 16, 16];
    private bool[,,] placedBools = new bool[16, 16, 16];
    private Vector3 worldPos;
    private Queue<Vector3Int> addedPositions = new Queue<Vector3Int>();
    private Queue<Vector3Int> removedPositions = new Queue<Vector3Int>();
    private Queue<GameObject> removedObjects = new Queue<GameObject>();
    public void SetChunkSize(int width, int height, int length)
    {
        Debug.Log($"Set New Chunk Size to w:{width} h:{height} l:{length}");
        placedObjects = new GameObject[width + 1, height + 1, length + 1];
        placedBools = new bool[width + 1, height + 1, length + 1];
    }
    public void SetWorldPosition(Vector3 position) {
        worldPos = position;
    }
    public void UpdateDecoration() {

        while (addedPositions.Count > 0)
        {
            if (transform == null)
                return;

            var pos = addedPositions.Dequeue();

            if(Random.value > decorAmount)
            {
                placedBools[pos.y, pos.x, pos.z] = true;
                continue;
            }

            GameObject obj;

            if(removedObjects.Count > 0)
            {
                obj = removedObjects.Dequeue();

                while (obj == null && removedObjects.Count > 0)
                {
                    obj = removedObjects.Dequeue();
                }

                if(obj == null)
                {
                    obj = Instantiate(decor[Random.Range(0, decor.Count - 1)], transform);
                }

                obj.SetActive(true);

                Debug.Log("Sucessfull Queue");
            }
            else
            {


                obj = Instantiate(decor[Random.Range(0, decor.Count - 1)], transform);
            }

            obj.transform.position = pos + worldPos;
            obj.transform.rotation = Quaternion.Euler(0, Random.value * 90, 0);
            placedObjects[pos.x, pos.y, pos.z] = obj;
            placedBools[pos.y, pos.x, pos.z] = true;
        }

        while (removedPositions.Count > 0)
        {
            var pos = removedPositions.Dequeue();

            if (placedObjects[pos.x, pos.y, pos.z] != null)
            {
                var obj = placedObjects[pos.x, pos.y, pos.z];
                obj.SetActive(false);
                removedObjects.Enqueue(obj);

                placedBools[pos.y, pos.x, pos.z] = false;
                placedObjects[pos.x, pos.y, pos.z] = null;
            }
        }
    }
    public void PlaceObject(Vector3Int localPosition)
    {
        if (!addedPositions.Contains(localPosition))
        {
            if (placedObjects.GetLength(0) > localPosition.x && placedObjects.GetLength(1) > localPosition.y && placedObjects.GetLength(2) > localPosition.z)
            {
                if (!placedBools[localPosition.x, localPosition.y, localPosition.z])
                {
                    addedPositions.Enqueue(localPosition);
                    placedBools[localPosition.x, localPosition.y, localPosition.z] = true;
                }
            }
        }
    }
    
    public void RemoveObject(Vector3Int localPosition) 
    {
        if (!removedPositions.Contains(localPosition))
        {
            if (placedObjects.GetLength(0) > localPosition.x && placedObjects.GetLength(1) > localPosition.y && placedObjects.GetLength(2) > localPosition.z)
            {
                if (placedBools[localPosition.x, localPosition.y, localPosition.z])
                {
                    removedPositions.Enqueue(localPosition);
                    placedBools[localPosition.x, localPosition.y, localPosition.z] = false;
                }
            }
        }
    }

    public void ClearAllObjects()
    {
        foreach (var obj in placedObjects)
        {
            if(obj != null)
            {
                Destroy(obj);
            }
        }
    }
}
