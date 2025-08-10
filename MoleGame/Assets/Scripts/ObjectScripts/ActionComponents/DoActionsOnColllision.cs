using UnityEngine;
using AYellowpaper.SerializedCollections;
public class DoActionsOnColllision : MonoBehaviour
{
    [SerializedDictionary("IAction", "LayerMask")]
    public SerializedDictionary<IActionComponent, LayerMask> actionLayerMasks;
    private void OnCollisionEnter(Collision collision)
    {
        if (GetComponent<Rigidbody>() is Rigidbody rb && rb != null)
        {
            if (rb.isKinematic)
            {
                return;
            }
        }

        foreach(var kvp in actionLayerMasks)
        {
            if (kvp.Value == (kvp.Value | (1 << collision.gameObject.layer))) {

                if (kvp.Key != null)
                {
                    kvp.Key.DoAction();
                }
            }
        }
        
    }
}
