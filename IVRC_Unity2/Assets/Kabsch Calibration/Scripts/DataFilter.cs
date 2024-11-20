using UnityEngine;

public class DataFilter
{
    private float positionLowPassFactor = 0.1f;
    private float rotationLowPassFactor = 0.1f;

    private Vector3 filteredPosition;
    private Quaternion filteredRotation;

    public Vector3 FilterPosition(Vector3 newPosition)
    {
        filteredPosition = Vector3.Lerp(filteredPosition, newPosition, positionLowPassFactor);
        return filteredPosition;
    }

    public Quaternion FilterRotation(Quaternion newRotation)
    {
        filteredRotation = Quaternion.Slerp(filteredRotation, newRotation, rotationLowPassFactor);
        return filteredRotation;
    }
}
