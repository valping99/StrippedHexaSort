using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class HexTileBehaviour : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private float _jumpHeight = 1.0f;
    [SerializeField] private float _jumpDuration = 0.1f;
    [SerializeField] private int _milisecondsToDelay = 80;
    [field: SerializeField] public float Height = 0.2f;
    public Color ColorMaterial { get; private set; }
    public Vector3 OriginalPosition { get; private set; }
    public ETileColor Color { get; private set; }
    private bool _isAnimating = false;

    public void Configuration(TileGroupSO group)
    {
        _meshRenderer.material = group.TileMaterial;
        Color = group.TileColor;
        ColorMaterial = group.TileMaterial.color;
    }

    public void SetOriginalPosition(Vector3 position)
    {
        OriginalPosition = position;
    }

    public async UniTask JumpAndFlipToTarget(HexTileBehaviour hexTileBehaviour, Vector3 higherObject)
    {
        if (_isAnimating) return;
        _isAnimating = true;
        JumpToPosition(hexTileBehaviour.OriginalPosition, higherObject, _jumpDuration).Forget();
        transform.localEulerAngles = Vector3.zero;
        await UniTask.Delay(_milisecondsToDelay);
        _isAnimating = false;
    }

    private async UniTask JumpToPosition(Vector3 targetPosition, Vector3 higherObject, float duration)
    {
        Vector3 moveTarget = new Vector3(targetPosition.x, targetPosition.y + Height, targetPosition.z);

        SetOriginalPosition(moveTarget);

        Vector3 target = new Vector3(0, targetPosition.y + Height, 0);
        Sequence jumpSequence = DOTween.Sequence();

        Vector3 midPoint = new Vector3(
            (moveTarget.x + transform.localPosition.x) / 2,
            higherObject.y + _jumpHeight, 
            (moveTarget.z + transform.localPosition.z) / 2
        );

        jumpSequence
           .Append(transform.DOLocalPath(new Vector3[] { transform.localPosition, midPoint, target }, duration,
                PathType.CatmullRom).SetEase(Ease.OutQuad))
           .Join(transform.DOLocalRotate(new Vector3(180, 0, 0), duration,
                RotateMode.LocalAxisAdd).SetEase(Ease.OutQuad));

        await jumpSequence.ToUniTask();
    }

    public async UniTask ClearHexTile()
    {
        transform.DOScale(Vector3.zero, .5f).SetEase(Ease.InBack).ToUniTask();
        await UniTask.Delay(100);
    }
}