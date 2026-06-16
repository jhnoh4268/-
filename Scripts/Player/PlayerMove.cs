using System.Dynamic;
//using Unity.Android.Gradle.Manifest;
using UnityEngine;
using FMODUnity;

public class PlayerMove : MonoBehaviour
{
    private PlayerController pc;
    private PlayerInput pi;
    private PlayerAnim pa;

    public Vector3 verticalVelocity { get; private set; }
    public Vector3 horizontalVelocity { get; private set; }
    public float moveSpeed { get; private set; } = 5f;

    private float radius;
    // 충돌제외레이어 저장변수
    int layerMask;

    private float wallJumpLockDuraction = 0.3f;

    private bool isGrounded;
    private float gravity = -20f;

    private int jumpCount;
    private float jumpForce = 8f;

    [Header("wallJump")] // 벽점프 (horizontal)
    public Vector3 wallJumplForce { get; private set; }
    private float wallJumpForceDecay = 5f;

    [Header("Dash")] // 대시 (horizontal)
    public Vector3 dashForce { get; private set; }
    private float dashForceDecay = 8f;

    [Header("springJump")] // 스프링 (horizontal)
    public Vector3 springJumpForce { get; private set; }
    private float springJumpForceDecay = 8f;

    public Vector3 hitForce { get; private set; }
    private float hitForceDecay = 5f;

    private bool canBreak = true;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 코인박스
        if(hit.collider.CompareTag("CoinInBox"))
        {   
            // 밑에서 충돌했을때만
            if(hit.normal.y > -1 || !canBreak) return;
            canBreak = false;
            hit.collider.GetComponent<CoinInBox>().OnCoinInBoxHit();
        }

        // 벽돌
        if(hit.collider.CompareTag("Brick"))
        {
            // 밑에서 충돌했을때만
            if(hit.normal.y > -1 || !canBreak) return;
            canBreak = false;
            hit.collider.GetComponent<Brick>().OnBrickHit();
        }
    }
    private void Start()
    {
        pc = GetComponent<PlayerController>();
        pi = GetComponent<PlayerInput>();
        pa = GetComponent<PlayerAnim>();

        radius = 0.25f;
        // player는 충돌제외
        layerMask = ~LayerMask.GetMask("Player");
    }

    public void MoveUpdate()
    {
        CheckGround();
        ApplyGravity();

        FollowPlatform();
        FollowRotatePlatform();
        SpringJump();

        UpdateExternalForce();
    }

    public void ExcuteMove()
    {
        Vector3 FinalMove;
        //          움직임 방향,         움직임 속도,    수직힘(중력, 점프)
        FinalMove = (pi.HorizontalDir * moveSpeed + verticalVelocity);

        FinalMove += wallJumplForce;
        FinalMove += springJumpForce;
        FinalMove += horizontalVelocity;
        FinalMove += hitForce;

        // 최종 움직임 실행
        pc.controller.Move(FinalMove * Time.deltaTime);
    }

    public void CheckGround()
    {
        // 바닥에 닿았는지 확인후 저장
        if(pc.controller.isGrounded)
        {
            canBreak = true;
            isGrounded = true;
        } 
        else
        {
            isGrounded = false;
        }
    }

    public void ApplyGravity()
    {

        if(!isGrounded)
        {
            // (animation) isFall false로 전환
            pa.TriggerFallAni(false);
            // 시간에 따라 중력에의해 낙하속도 증가
            verticalVelocity += new Vector3(0, gravity * Time.deltaTime, 0);
        } else
        {
            // 바닥에 닿을시 대시속도 삭제
            dashForce = Vector3.zero;
            // 바닥에 닿을시 벽점프힘 삭제
            wallJumplForce = Vector3.zero;

            // 바닥에 붙이기
            verticalVelocity = new Vector3(0, -2f, 0);

            jumpCount = 0;
            // 대시후 땅에 접촉시 조작가능
            pc.inputLockerTimer = 0;
        }
    }

    public void Jump()
    {
        jumpCount++;
        verticalVelocity = new Vector3(0, jumpForce, 0);
    }

    public void DoubleJump()
    {
        if(jumpCount >= 2)
            return;
        jumpCount++;

        verticalVelocity = new Vector3(0, jumpForce * 0.8f, 0);
    }

    public void WallJump(RaycastHit wallInfo)
    {
        jumpCount++;
        Vector3 wallNormal = wallInfo.normal;
        Vector3 horizontalDir = new Vector3(wallNormal.x, 0, wallNormal.z).normalized;

        // 벽점프시 필요한 수평힘값 저장
        float wallJumpHorizontalForce = 10f;
        // WallJumpForce에 할당하여 점진적 감소 유도
        wallJumplForce = horizontalDir * wallJumpHorizontalForce;

        // 수직 점프
        verticalVelocity = new Vector3(0, jumpForce * 0.8f, 0);

        // 벽차기후 반대 방향 바라보기
        transform.forward = horizontalDir;
        // 일정시간 입력 잠금
        pc.inputLockerTimer = wallJumpLockDuraction;
    }

    void UpdateExternalForce()
    {
        // wallJumpForce값 점차 감소
        wallJumplForce = Vector3.Lerp(wallJumplForce, Vector3.zero, wallJumpForceDecay * Time.deltaTime);
        // dashforce 값 점차 감소
        dashForce = Vector3.Lerp(dashForce, Vector3.zero, dashForceDecay * Time.deltaTime * 0.6f);
        // springforce 값 점차 감소
        springJumpForce = Vector3.Lerp(springJumpForce, Vector3.zero, springJumpForceDecay * Time.deltaTime);

        hitForce = Vector3.Lerp(hitForce, Vector3.zero, hitForceDecay * Time.deltaTime);
    }

    void FollowPlatform()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.2f))
        {
            MovingPlatform dt = hit.collider.GetComponent<MovingPlatform>();
            if(dt == null)
                return;
            if(!isGrounded)
                return;

            Vector3 platformMoveDistance = dt.platformMovement / Time.deltaTime;
            horizontalVelocity = new Vector3(platformMoveDistance.x, 0, platformMoveDistance.z);

            if(platformMoveDistance.y != 0)
            {
                // 수직플랫폼에서 땅에 강하게 붙이기
                verticalVelocity = new Vector3(0, platformMoveDistance.y - 3f, 0);
            }
        } else
        {
            horizontalVelocity = Vector3.zero;
        }
    }

    void FollowRotatePlatform()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.2f))
        {
            RotatePlatform rp = hit.collider.GetComponent<RotatePlatform>();
            if(rp == null)
            {

                return;
            }
            if(!isGrounded)
                return;

            // 캐릭터의 위치 보정
            Vector3 relativePos = transform.position - rp.transform.position;
            Vector3 nextPos = rp.rotationDelta * relativePos;

            // 실제 이동해야할 거리 계산
            Vector3 moveDistance = (rp.transform.position + nextPos) - transform.position;

            // 캐릭터의 회전보정
            transform.rotation = rp.rotationDelta * transform.rotation;

            horizontalVelocity = moveDistance / Time.deltaTime;
        } else
        {
            horizontalVelocity = Vector3.zero;
        }
    }

    void SpringJump()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.5f))
        {
            if(!isGrounded)
                return;

            if(hit.collider.CompareTag("Spring"))
            {
                pa.TriggerJumpAni();

                Vector3 springNormal = hit.normal;
                Vector3 horizontalDir = new Vector3(springNormal.x, 0, springNormal.z).normalized;

                jumpCount = 2;

                verticalVelocity = new Vector3(0, jumpForce * 2.5f, 0);
                springJumpForce = horizontalDir * jumpForce * 2.5f;
            }
        }
    }

    public void HitPlayer(Vector3 dir, float force)
    {
        pc.inputLockerTimer = 0.5f;
        hitForce = dir * force;
    }
}

