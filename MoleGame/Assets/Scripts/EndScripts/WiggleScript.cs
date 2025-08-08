using UnityEngine;

public class WiggleScript : MonoBehaviour
{
    public float wiggleAmount = 5f; 
    public float wiggleSpeed = 5f;

    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.rotation;
    }

    void Update()
    {
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        Quaternion targetRotation = initialRotation * Quaternion.Euler(
            -mouseY * wiggleAmount,
            mouseX * wiggleAmount,
            0f
        );

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * wiggleSpeed);
    }

}
