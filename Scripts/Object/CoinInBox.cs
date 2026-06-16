using UnityEngine;

public class CoinInBox : MonoBehaviour
{
    private bool isActivited = true;
    public GameObject effect;

    public GameObject ActivitedBlock;
    public GameObject unActivitedBlock;
    public int value = 1;

    //코인박스 충돌시 호출
    public void OnCoinInBoxHit()
    {
        if(!isActivited) return;
        isActivited = false;

        PlayerDataManager.Instance.AddCoin(value);

        effect = Instantiate(effect);
        effect.transform.position = gameObject.transform.position;

        ActivitedBlock.SetActive(false);
        unActivitedBlock.SetActive(true);

        Invoke("DestroyEffect", 1f);
    }

    private void DestroyEffect()
    {
        Destroy(effect);
    }
}
