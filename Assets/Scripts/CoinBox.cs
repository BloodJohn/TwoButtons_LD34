using System;
using DG.Tweening;
using UnityEngine;

public class CoinBox : MonoBehaviour
{
    [SerializeField] private Transform root;
    [SerializeField] private Transform coin;
    [SerializeField] private int coinAmount = 6;

    private Vector3 coinInitialScale;

    private bool _isActive;

    private void Start()
    {
        coinInitialScale = coin.transform.localScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isActive || !other.CompareTag("Player"))
            return;

        _isActive = true;
        coinAmount += -1;

        root.transform.DOMove(root.transform.position + new Vector3(0, 0.1f, 0), 0.1f)
            .SetLoops(2, LoopType.Yoyo)
            .OnKill(OnComplete);

        coin.DOMove(coin.transform.position + new Vector3(0, 1f, 0), 0.4f)
            .SetLoops(2, LoopType.Yoyo);

        coin.localScale = coinInitialScale;
        coin.DOScaleX(-coinInitialScale.x, 0.4f).SetLoops(2, LoopType.Yoyo);
    }

    private void OnComplete()
    {
        _isActive = false;
        if(coinAmount == 0)
            root.gameObject.SetActive(false);
    }
}
