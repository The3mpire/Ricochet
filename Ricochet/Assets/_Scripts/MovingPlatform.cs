using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("Uses a random wait time between Min Wait Time and Wait Time")]
    [SerializeField] private bool useRandomWaitTime;
    [Tooltip("The minimum wait time when Use Random Wait Time is true")]
    [SerializeField] private float minWaitTime;
    [Tooltip("The speed in units/second the platform moves")]
    [SerializeField] private float moveSpeed;
    [Tooltip("The time the platform waits till moving to the next position")]
    [SerializeField] private float waitTime;
    [Tooltip("The List of positions the platform will move to")]
    [SerializeField] private List<Vector3> positions;
    #endregion

    #region Private Variables
    private int nextPosInd;
    private GameManager gameManager;
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        nextPosInd = 1;
    }

    private void Start()
    {
        StartCoroutine(WaitHelper());
    }
    #endregion

    #region Public Methods
    public void Move()
    {
        transform.DOKill();

        Vector3 dest = positions[nextPosInd++];
        float xDiff = transform.localPosition.x - dest.x;
        float yDiff = transform.localPosition.y - dest.y;
        float dist = Mathf.Sqrt(xDiff * xDiff + yDiff * yDiff);

        transform.DOLocalMove(dest, dist / moveSpeed).OnComplete(() => StartCoroutine(WaitHelper())).SetEase(Ease.InOutSine);
        if (nextPosInd == positions.Count)
        {
            nextPosInd = 0;
        }
    }
    #endregion

    #if UNITY_EDITOR
    public void UpdatePosition()
    {
        transform.localPosition = positions[0];
    }
    #endif

    #region Private Methods
    private IEnumerator WaitHelper()
    {
        if (useRandomWaitTime)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, waitTime));
        }
        else
        {
            yield return new WaitForSeconds(waitTime);
        }
        
        Move();
    }
    #endregion

    #region Getters
    public List<Vector3> GetPositions()
    {
        return positions;
    }
    #endregion
}
