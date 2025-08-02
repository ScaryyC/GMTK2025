using Unity.VisualScripting;
using UnityEngine;

public class LightRotator : MonoBehaviour
{
    public bool isLightRotating = true;
    public float rotationRate = 0.001f;

    void Update()
    {
        if (isLightRotating)
        {
            RotateLight();
        }
    }

    void RotateLight()
    {
        transform.Rotate(Vector3.up, rotationRate);
    }
}
