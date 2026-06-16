using UnityEngine;

public class Hammer : MonoBehaviour
{
    // 속도, 최대각
    public float swingSpeed = 2.0f;
    public float maxAngle = 60.0f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime * swingSpeed;

        float swingFactor = Mathf.Sin(timer);

        float newAngle = swingFactor * maxAngle;

        Vector3 angles = gameObject.transform.localEulerAngles;
        // 에셋이 45도 틀어져있어서 빼줬음
        angles.z = newAngle - 45f;

        gameObject.transform.localEulerAngles = angles;
    }
}
