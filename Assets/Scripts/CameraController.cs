using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform targetOfCameraTransform;

    private void LateUpdate()
    {
        if (!targetOfCameraTransform)
        {
            Debug.LogError("Target of camera transform is null!");
            return;
        }

        var targetPosition = targetOfCameraTransform.position;
        transform.position = new Vector3(
            targetPosition.x,
            targetPosition.y,
            transform.position.z
        );
    }
}