using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    // 민감도, 각도
    public float Sensitivity = 200f;
    Vector3 angle;

    public GameObject target;
    private Vector3 Distance;
    void Start()
    {
        angle = Camera.main.transform.eulerAngles;
        angle.x *= -1;
        
        // 플레이어와 카메라 사이 간격
        Distance = new Vector3(0f, -2f, 8f);
    }

    void LateUpdate()
    {

        // 마우스 이동량 저장용 변수
        float x = Input.GetAxis("Mouse Y");
        float y = Input.GetAxis("Mouse X");

        // 카메라 각도 계산
        angle.x += x * Sensitivity * Time.deltaTime;
        angle.y += y * Sensitivity * Time.deltaTime;

        // 카메라 각도 제한(카메라 위, 아래로 돌렸을때 90도이상 돌아가는거 방지)
        angle.x = Mathf.Clamp(angle.x, -90f, 0);
        // 카메라 각도 적용
        transform.eulerAngles = new Vector3(-angle.x, angle.y, 0);
        // 카메라 위치 적용
        transform.position = target.transform.position - transform.rotation*Distance;
    }
}
