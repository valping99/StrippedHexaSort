using System;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField] private SelectedEventSO _selectedEvent;
    [SerializeField] private SelectedHexTileGroupSO _selectedHexTileGroup;
    [SerializeField] private HexBase _defaultHexBase;
    [SerializeField] private HexTileGroup _hexTileGroup;
    [SerializeField] private float _dragHeight;
    [SerializeField] private float _radius;
    private Vector3 _offset;
    private float _distance;
    private bool _isDragging = false;
    private Vector3 _originalPosition;

    void OnMouseDown()
    {
        _originalPosition = transform.position;
        _distance = Camera.main.WorldToScreenPoint(transform.position).z;
        _offset = transform.position - GetTouchWorldPosition();
        _isDragging = _hexTileGroup.IsDropped ? false : true;
    }

    void OnMouseDrag()
    {
        if (_isDragging)
        {
            Vector3 newPosition = GetTouchWorldPosition() + _offset;
            newPosition.y = _dragHeight;
            transform.position = newPosition;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, new Vector3(0,-100,0), out hit))
            {
                if (hit.collider.CompareTag("HexBase"))
                {
                    HexBase hex = hit.collider.gameObject.GetComponent<HexBase>();
                    _selectedEvent.RaiseEvent(hex);
                }
            }
            else
            {
                _selectedEvent.RaiseEvent(_defaultHexBase);
            }
        }
    }

    void OnMouseUp()
    {
        if (_isDragging)
        {
            RaycastHit hit;

            if (Physics.SphereCast(transform.position, _radius, Vector3.down, out hit, 1000))
            {
                Debug.DrawLine(transform.position, hit.point, Color.white);

                if (hit.collider.CompareTag("HexBase"))
                {
                    HexBase hex = hit.collider.gameObject.GetComponent<HexBase>();

                    if (hex.IsDropped())
                    {
                        transform.position = _originalPosition;
                        return;
                    }

                    hex.SetDropped(_hexTileGroup, true);
                    _hexTileGroup.DropHexTile(hex, true);
                    _hexTileGroup.transform.SetParent(hex.transform);
                    transform.localRotation = Quaternion.Euler(0,90,90);
                    transform.position = hex.transform.position;
                    _selectedHexTileGroup.RaiseEvent(_hexTileGroup);
                    return;
                }
            }

            _isDragging = false;
            transform.position = _originalPosition;
            AudioManager.Instance.PlaySfxSource(EAudioSfxTracking.Sfx_CanNotPutDown);
        }
    }

    Vector3 GetTouchWorldPosition()
    {
        Vector3 screenPosition = Input.mousePosition;
        screenPosition.z = _distance;
        return Camera.main.ScreenToWorldPoint(screenPosition);
    }
}