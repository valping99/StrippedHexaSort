using System;
using DG.Tweening;
using UnityEngine;

public class HexGidRotate : MonoBehaviour
{
    private float _lastMouseX;                           
    private float _currentRotationY;
    [SerializeField] private float _rotationSpeed = 0.5f;
    [SerializeField] private Transform _baseTransform;
    [SerializeField] private AxisBasement _axisBasement;

    private void Start()
    {
        _axisBasement.AxisOffset = 0;
    }

    private void OnMouseDown()
    {
        _lastMouseX = Input.mousePosition.x;

        _currentRotationY = transform.eulerAngles.y;
        AudioManager.Instance.PlaySfxSource(EAudioSfxTracking.Sfx_MapSlide);
    }

    private void OnMouseDrag()
    {
        float deltaX = Input.mousePosition.x - _lastMouseX;
        float newRotationY = _currentRotationY + deltaX * _rotationSpeed;

        transform.rotation = Quaternion.Euler(0f, newRotationY, 0f);
    }
    
    private void OnMouseUp()
    {
        float snappedRotationY = Mathf.Round(transform.eulerAngles.y / 60f) * 60f;

        transform.DORotate(new Vector3(0f, snappedRotationY, 0f), 0.2f, RotateMode.FastBeyond360);
        _axisBasement.AxisOffset = snappedRotationY;
    }
}