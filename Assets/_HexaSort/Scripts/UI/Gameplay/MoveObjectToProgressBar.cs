using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MoveObjectToProgressBar : MonoBehaviour
{
    [SerializeField] private RectTransform _targetUI;
    [SerializeField] private Image _prefab;
    [SerializeField] private float _timeDuration = .7f;

    public void MoveToProgress(HexTileBehaviour hexTile)
    {
        if (hexTile == null) return;
        Image icon = Instantiate(_prefab, hexTile.transform);
        RectTransform rectTransform = icon.GetComponent<RectTransform>();
        icon.color = hexTile.ColorMaterial;
        SetupRectTransform(rectTransform);
        
        rectTransform.DOLocalMove(Vector3.zero, _timeDuration).SetEase(Ease.OutQuint)
                     .OnComplete(() => DestroyObject(icon.gameObject));
        rectTransform.DORotate(Vector3.zero, _timeDuration).SetEase(Ease.Linear)
                     .OnComplete(() => DestroyObject(icon.gameObject));
    }

    private void SetupRectTransform(RectTransform rectTransform)
    {
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.SetParent(_targetUI);
        rectTransform.localScale = Vector3.one;
        rectTransform.rotation = Quaternion.Euler(60,0,0);
    }

    private void DestroyObject(GameObject objectToDestroy)
    {
        Destroy(objectToDestroy);
    }
}