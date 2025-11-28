using UnityEngine;

public class gearRotation : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public bool rightDirection = false;
    public bool leftDirection = false;

    void Update()
    {
        if (rightDirection)
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
        if (leftDirection)
        {
            transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);
        }
    }
}
