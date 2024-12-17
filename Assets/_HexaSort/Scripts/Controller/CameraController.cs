using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private BoxCollider _boxCollider;

    void Update()
    {
        AdjustCameraSizeToFitObject();
    }

    private void AdjustCameraSizeToFitObject()
    {
        Camera camera = Camera.main;
        Bounds bounds = _boxCollider.bounds;

        float requiredSize = Mathf.Max(bounds.size.x, bounds.size.y) / 2;
        camera.orthographicSize = requiredSize;
    }
}