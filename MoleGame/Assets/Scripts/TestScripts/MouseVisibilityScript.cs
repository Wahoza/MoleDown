using UnityEngine;

public class MouseVisibilityScript : MonoBehaviour
{
    [SerializeField] bool visible;
    void Start()
    {
        Cursor.visible = visible;
    }
}
