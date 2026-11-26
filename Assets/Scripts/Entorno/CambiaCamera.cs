using UnityEngine;

public class CambiaCamera : MonoBehaviour
{
    [SerializeField] private CameraFollow2D cameraFollow;
    [SerializeField] private float nuevoOffsetY = 2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Se detectó player");
            if (cameraFollow == null)
                cameraFollow = Camera.main.GetComponent<CameraFollow2D>();

            cameraFollow.SetOffsetY(nuevoOffsetY);
        }
    }
}