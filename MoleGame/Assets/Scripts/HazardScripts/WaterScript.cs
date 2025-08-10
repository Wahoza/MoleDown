using UnityEngine;
using UnityEngine.EventSystems;

public class WaterScript : MonoBehaviour
{
    [SerializeField] float damping;
    [SerializeField] float dampingSpeed;
    [SerializeField] string splashName;
    [SerializeField] string splashAudioName;

    private void Awake()
    {
        damping = 1/damping;
    }

    private void OnTriggerEnter(Collider other)
    {
        ParticleManager.Instance.DoParticles(splashName, other.transform.position);
        AudioManager.Instance.Play3DAudioAtPosition(splashAudioName, other.transform.position, true);
    }
    //private void OnTriggerStay(Collider other)
    //{
    //    Rigidbody rb = other.attachedRigidbody;
    //    if (rb != null)
    //    {
    //        if (!rb.isKinematic)
    //        {
    //            Vector3 mixedVelocity = Vector3.Lerp(rb.linearVelocity, rb.linearVelocity * damping, dampingSpeed * Time.deltaTime);
    //            rb.linearVelocity = new Vector3(mixedVelocity.x, rb.linearVelocity.y, mixedVelocity.z);
    //        }
    //    }
    //}
}
