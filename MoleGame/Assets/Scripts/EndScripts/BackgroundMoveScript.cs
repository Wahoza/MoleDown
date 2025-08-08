using UnityEngine;
using UnityEngine.UI;

public class BackgroundMoveScript : MonoBehaviour
{
    [SerializeField] float offsetChangeSpeed;
    [SerializeField] Image bgImage;
    private float currentOffset;

    private void Start()
    {
        currentOffset = 1;
    }
    private void Update()
    {
        currentOffset += offsetChangeSpeed * Time.deltaTime;
        bgImage.material.SetFloat("_CellOffset", currentOffset);
    }
}
