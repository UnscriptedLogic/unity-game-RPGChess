using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnscriptedLogic;

public class EffectUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI amountTMP;
    [SerializeField] private TextMeshProUGUI effectTMP;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float verticalOffset;
    [SerializeField] private float animDuration;
    [SerializeField] private Ease ease;

    private void Start()
    {
        canvasGroup.DOFade(0f, animDuration).SetEase(ease);
        transform.DOMoveY(transform.position.y + verticalOffset, animDuration).SetEase(ease).OnComplete(() => Destroy(gameObject));
    }

    public void SetText(int amount, string desc = null)
    {
        bool isStringNotEmpty = !string.IsNullOrEmpty(desc);
        effectTMP.gameObject.SetActive(isStringNotEmpty);
        
        if (isStringNotEmpty)
        {
            effectTMP.text = desc;
        }

        amountTMP.text = amount.ToString();
    }
}
