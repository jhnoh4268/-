using UnityEngine;
using DG.Tweening;
using System.Collections;

public class TimedPlatform : MonoBehaviour, IPlatform
{

    private Vector3 originPosition;
    private Vector3 originScale;
    private Collider platformCollider;
    private BoxCollider TriggerCollider;
    private MeshRenderer platformRender;

    public float fallDelay = 1f;
    public float respawnTime = 3f;

    public bool IsActivated { get; private set; } = false;

    public Vector3 CurrentVelocity { get; private set; }

    
    private void Start()
    {
        originPosition = transform.position;
        originScale = transform.localScale;
        platformCollider = GetComponent<Collider>();
        TriggerCollider = GetComponent<BoxCollider>();
        platformRender = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Player") && !IsActivated)
        {
            Move();
        }
    }

    IEnumerator Disappear()
    {
        IsActivated = true;

        // 플랫폼 흔들림
        transform.DOShakePosition(fallDelay, 0.25f, 15, 90, false, true);
        yield return new WaitForSeconds(fallDelay);

        // 작아지면서 사라짐
        transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
        // collider 비활성화
        platformCollider.enabled = false;
        TriggerCollider.enabled = false;

        // 사라진후 render 비활성화
        yield return new WaitForSeconds(0.2f);
        platformRender.enabled = false;

        // 리스폰시간동안 대기
        yield return new WaitForSeconds(respawnTime);

        // 남아있을수있는 움직임 삭제
        transform.DOKill();
        // 안정성을 위해 한프레임 기다림
        yield return new WaitForEndOfFrame();

        ResetPlatform();
    }

    public void Move()
    {
        StartCoroutine("Disappear");
    }

    public void ResetPlatform()
    {
        // 플랫폼 리스폰을 위해 원래설정값으로 되돌리기
        transform.position = originPosition;
        transform.localScale = Vector3.zero;
        platformRender.enabled = true;

        // 완전히 생긴후에 collider키기
        transform.DOScale(originScale, 0.5f).SetEase(Ease.OutBack).OnComplete(() => {
            IsActivated = false;
            platformCollider.enabled = true;
            TriggerCollider.enabled = true;
        });
    }
}
