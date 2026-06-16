using UnityEngine;

public class RotatePlatform : MonoBehaviour
{
    public int rotationDir = 1;
    public float rotationSpeed = 2;
    public Quaternion rotationDelta;
    private Quaternion lastRotation;

    public Vector3 axis;
    private Vector3 rotateVector;
    private void Start()
    {
        lastRotation = transform.rotation;

        rotateVector = axis * rotationSpeed * rotationDir;
    }

    void Update()
    {
        // 회전
        transform.Rotate(rotateVector);

        // 이번 프레임의 회전 변화량 계산
        rotationDelta = transform.rotation * Quaternion.Inverse(lastRotation);
        lastRotation = transform.rotation;
    }
}
