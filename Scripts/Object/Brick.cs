using UnityEngine;

public class Brick : MonoBehaviour
{
    private bool isActivited = true;
    public GameObject effect;

    public GameObject ActivitedBlock;

    //벽돌 충돌시 호출
    public void OnBrickHit()
    {
        if(!isActivited) return;
        isActivited = false;

        effect = Instantiate(effect);
        effect.transform.position = transform.position;

        ActivitedBlock.SetActive(false);
        Invoke("DestroyEffect", 1f);
    }

    private void DestroyEffect()
    {
        Destroy(effect);
        Destroy(gameObject);
    }
}
