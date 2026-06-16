using UnityEngine;
using FMODUnity;

public class Coin : MonoBehaviour
{
    public int value = 1;
    public float rotationSpeed = 50f;

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // FMOD
            RuntimeManager.PlayOneShot("event:/getCoin", transform.position);

            // PlayerDataManager 체크
            if (PlayerDataManager.Instance != null)
            {
                PlayerDataManager.Instance.AddCoin(value);
            }
            else
            {
                Debug.LogError("PlayerDataManager 없음");
            }


            CoinEffectPool.instance.PlayEffect(transform.position);
            Destroy(gameObject);
        }
    }
}