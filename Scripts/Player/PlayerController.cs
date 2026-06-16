using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller { get; set; }
    private PlayerInput pi;
    private PlayerMove pm;
    private PlayerAnim pa;

    // (벽점프)
    RaycastHit hit;
    // 상태
    public enum PlayerState
    {
        Idle,
        Run,
        Jump,
        DoubleJump,
        WallJump
    };
    public PlayerState currentState;

    [HideInInspector] public float inputLockerTimer { get; set; }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        pi = GetComponent<PlayerInput>();
        pm = GetComponent<PlayerMove>();
        pa = GetComponent<PlayerAnim>();
    }
    private void Update()
    {
        if(inputLockerTimer > 0)
            inputLockerTimer -= Time.deltaTime;

        // player animController에 현재 속도, 현재 바닥접지여부 전달
        pa.SetMoveParams(new Vector2(pi.h, pi.v).magnitude, controller.isGrounded);
        // player 움직임(물리상호작용) 실제 적용전(계산만)
        pm.MoveUpdate();
        // FSM 상태핸들링 및 상태 전이
        HandleState();
        // player 움직임계산 실제적용
        pm.ExcuteMove();
    }
    private void HandleState()
    {
        bool isMoving = (inputLockerTimer <= 0) && (pi.HorizontalDir.magnitude > 0.1f);
        bool canJump = (inputLockerTimer <= 0) && (pi.isJump || pi.coyoteTimer > 0) && controller.isGrounded;

        float radius = 0.25f;
        bool canWallJump =
            (inputLockerTimer <= 0) && pi.isJump &&
            Physics.SphereCast(transform.position + new Vector3(0, 0.5f, 0),
            radius, transform.forward, out hit, 0.06f, ~LayerMask.GetMask("Player"));

        switch(currentState)
        {
            // 대기상태
            case PlayerState.Idle:
                if(isMoving)
                    ChangeState(PlayerState.Run);
                if(canJump)
                    ChangeState(PlayerState.Jump);
            break;
            // 달리기 상태
            case PlayerState.Run:
                if(!isMoving)
                    ChangeState(PlayerState.Idle);
                if(canJump)
                    ChangeState(PlayerState.Jump);
            break;
            // 점프 상태
            case PlayerState.Jump:
                if(controller.isGrounded)
                {
                    ChangeState(isMoving ? PlayerState.Run : PlayerState.Idle);
                }
                if(pi.isJump)
                    ChangeState(PlayerState.DoubleJump);
                if(canWallJump)
                    ChangeState(PlayerState.WallJump);
            break;
            // 이단 점프
            case PlayerState.DoubleJump:
                if(controller.isGrounded)
                {
                    ChangeState(isMoving ? PlayerState.Run : PlayerState.Idle);
                }
            break;
            // 벽점프
            case PlayerState.WallJump:

                if(canWallJump)
                {
                    pm.WallJump(hit);
                    pa.TriggerJumpAni();
                }
                if(controller.isGrounded)
                {
                    ChangeState(isMoving ? PlayerState.Run : PlayerState.Idle);
                }
            break;
        }
    }
    private void ChangeState(PlayerState newState)
    {
        if(currentState == newState)
            return;

        currentState = newState;

        // 상태 진입시 1회성 연산처리
        switch(currentState)
        {
            case PlayerState.Idle:
            break;
            case PlayerState.Run:
            break;
            case PlayerState.Jump:
            pm.Jump();
            pa.TriggerJumpAni();
            break;
            case PlayerState.DoubleJump:
            pm.DoubleJump();
            pa.TriggerDoubleJumpAni();
            break;
            case PlayerState.WallJump:
            pm.WallJump(hit);
            pa.TriggerJumpAni();
            break;
        }
    }
}
