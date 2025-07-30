using UnityEngine;

public class DestructionOrbTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] DestructionObject destruction;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        var parent = collision.transform.parent;

        if (parent != null)
        {
            var world = parent.gameObject.GetComponent<MarchingWorld>();
            if (world != null)
            {
                destruction.Deform(world, transform.position);
            }
        }
    }
}
