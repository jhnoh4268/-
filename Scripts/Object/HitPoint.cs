using UnityEngine;
using static ithappy.Platformer_9_Heaven.RotationScript;

public class HitPoint : MonoBehaviour
{
    public float force = 8f;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            // 수학적수식은 AI활용
            // 봉의 중심에서 플레이어를 바라보는 방향 (반경 방향)
            Vector3 radiusVec = other.transform.position - transform.position;

            // 회전축과 반경 벡터를 외적하여 '접선(회전) 방향'을 구함
            // 외적 순서에 따라 시계/반시계 방향이 결정됩니다.
            Vector3 tangentDir = Vector3.Cross(Vector3.up, radiusVec).normalized;

            // 밖으로 밀려나는 힘
            Vector3 pushOutDir = radiusVec.normalized;
            Vector3 finalDir = (tangentDir + pushOutDir * 0.5f).normalized;

            PlayerMove pm = other.transform.GetComponent<PlayerMove>();
            pm.HitPlayer(finalDir.normalized, force);
        }
    }
}
