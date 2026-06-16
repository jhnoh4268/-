using System.Collections;
using UnityEngine;
using DG.Tweening;

public class MovingPlatform : MonoBehaviour, IPlatform
{

    public Vector3 CurrentVelocity { get; private set; }
    public bool IsActivated { get; private set; } = true;

    public Vector3 targetPos;
    public float cycleTime;
    public bool useRandomDelay;

    public Vector3 platformMovement;
    private Vector3 lastPosition;
    private Vector3 startPos;

    
    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + targetPos;

        if(useRandomDelay)
        {
            StartCoroutine(RandomDelay());
        }
        else
        {
            Move();
        }
    }

    void Update()
    {
        platformMovement = transform.position - lastPosition;
        lastPosition = transform.position;
    }

    public void Move()
    {
        transform.DOMove(targetPos, cycleTime)
                 .SetLoops(-1, LoopType.Yoyo)
                 .SetEase(Ease.InOutSine)
                 .SetUpdate(UpdateType.Normal);
    }

    public void ResetPlatform()
    {
        
    }

    IEnumerator RandomDelay()
    {
        float randomTime = Random.Range(0, 2);
        yield return new WaitForSeconds(randomTime);
        Move();

        yield break;
    }

}
