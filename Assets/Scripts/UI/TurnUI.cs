using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnNameTMP;
    [SerializeField] private TextMeshProUGUI actionValueTMP;
    [SerializeField] private Image iconImg;

    [SerializeField] private float lerpSpeed = 0.5f;

    private Transform anchor;

    public Transform Anchor => anchor;

    public void SetIcon(Sprite icon) => iconImg.sprite = icon;
    public void SetActionValue(int value) => actionValueTMP.text = value.ToString();
    public void SetTurnName(string name) => turnNameTMP.text = name;
    
    public void SetAnchor(Transform anchor)
    {
        this.anchor = anchor;
        transform.position = anchor.position;
    }

    private void Update()
    {
        if (anchor != null)
        {
            transform.position = Vector3.Lerp(transform.position, anchor.position, lerpSpeed * Time.deltaTime);
        }
    }
}
