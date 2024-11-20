using UnityEngine;

public class DataAcquisition : MonoBehaviour
{
    public Transform tracker;
    public Transform HMD;

    public Vector3 PositionA { get; private set; }
    public Quaternion RotationA { get; private set; }

    public Vector3 PositionB { get; private set; }
    public Quaternion RotationB { get; private set; }

    void Update()
    {        
        PositionA = tracker.position;
        RotationA = tracker.rotation;

        PositionB = HMD.position;
        RotationB = HMD.rotation;
    }
}
