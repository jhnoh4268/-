using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerController pc;
    // 방향 설정
    public float h { get; private set; }
    public float v { get; private set; }
    public Vector3 HorizontalDir { get; private set; }
    // 점프입력
    public bool isJump { get; private set; }

    // 점프입력 유예시간
    [HideInInspector] public float coyoteTimer;
    [HideInInspector] public float coyoteDuration = 0.1f;
    void Start()
    {
        pc = GetComponent<PlayerController>();
    }

    private void Update()
    {
        SetHorzitalDir();
        SetIsJump();
    }

    private void SetHorzitalDir()
    {
        // 입력이 잠겨있을경우 입력불가능
        if(pc.inputLockerTimer > 0)
        {
            HorizontalDir = Vector3.zero;
            return;
        }

        // 입력 값 저장
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        // 카메라 기준 방향
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        forward.Normalize();
        right.y = 0;
        right.Normalize();

        // 방향설정
        HorizontalDir = forward * v + right * h;

        //대각선 이동 속도 보정
        if(HorizontalDir.magnitude > 1f)
        {
            HorizontalDir = HorizontalDir.normalized;
        }
        // player바라보는 방향 현재 이동방향으로 설정(조작키 입력시)
        if(HorizontalDir.magnitude > 0)
        {
            transform.forward = HorizontalDir.normalized;
        }
    }

    private void SetIsJump()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            isJump = true;
            coyoteTimer = coyoteDuration;
        }
        else
        {
            isJump = false;
            coyoteTimer -= Time.deltaTime;
        }
    }
}
