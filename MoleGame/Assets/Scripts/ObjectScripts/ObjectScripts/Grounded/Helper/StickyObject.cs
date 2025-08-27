using UnityEngine;
using System.Collections;
[RequireComponent(typeof(ConfigurableJoint))]
public class StickyObject : MonoBehaviour
{
    private ConfigurableJoint m_Joint;

    private Rigidbody stickBody;
    [SerializeField] float otherRbControll;
    public void DoStick(Rigidbody targetStickBody, Rigidbody originRigidbody)
    {
        m_Joint = GetComponent<ConfigurableJoint>();

        transform.position = targetStickBody.position;

        transform.SetParent(originRigidbody.transform);

        m_Joint.connectedBody = targetStickBody;

        stickBody = targetStickBody;
        stickBody.freezeRotation = false;
    }

    public void UnStick()
    {
        if(stickBody == null)
            return;

        stickBody.freezeRotation = true;
        stickBody.transform.rotation = Quaternion.identity;
    }
    private void OnDestroy()
    {
        UnStick();
    }
}
