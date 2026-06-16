using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class CoinEffectPool : MonoBehaviour
{
    //싱글턴
    public static CoinEffectPool instance;
    
    //오브젝트풀
    [SerializeField] private GameObject coinEffect;
    private IObjectPool<GameObject> coinEffectPool;

    private void Awake()
    {
        instance = this;

        coinEffectPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject effect = Instantiate(coinEffect);
                return effect;
            },
            actionOnGet: effect =>
            {
                effect.SetActive(true);
            },
            actionOnRelease: effect =>
            {
                effect.SetActive(false);
            },
            actionOnDestroy: effect =>
            {
                Destroy(effect);
            },
            defaultCapacity: 5,
            maxSize: 15
        );
    }
    //이펙트 호출
    public void PlayEffect(Vector3 position)
    {
        GameObject effect = coinEffectPool.Get();
        effect.transform.position = position;
        StartCoroutine(ReleaseEffectAfterTime(effect));
    }
    //이펙트 비활성화
    public void ReleaseEffect(GameObject effect)
    {
        coinEffectPool.Release(effect);
    }
    //1초후 비활성화
    IEnumerator ReleaseEffectAfterTime(GameObject effect)
    {
        yield return new WaitForSeconds(1f);
        ReleaseEffect(effect);
    }
}
