using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private Animator anim;
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void SetMoveParams(float moveSpeed, bool isGrounded)
    {
        // 애니메이터에 속도 전달
        anim.SetFloat("speed", moveSpeed);
        // 바닥 체크 전달
        anim.SetBool("isGrounded", isGrounded);
    }

    public void TriggerJumpAni()
    {
        anim.SetTrigger("doJump");
    }

    public void TriggerFallAni(bool isFall)
    {
        anim.SetBool("isFalling", isFall);
    }

    public void TriggerDoubleJumpAni()
    {
        anim.SetTrigger("doDoubleJump");
    }
}
