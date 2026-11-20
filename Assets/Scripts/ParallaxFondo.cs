using UnityEngine;

public class ParallaxFondo : MonoBehaviour
{
    public Transform camara;

    [Tooltip("1 = Pegado a la cámara | 0 = Quieto en el mundo")]
    [Range(0f, 1f)]
    public float velocidadHorizontal = 0.9f;

    [Tooltip("Más bajo que la horizontal para que no se corte al saltar alto")]
    [Range(0f, 1f)]
    public float velocidadVertical = 0.5f;

    private Vector3 ultimaPosicionCamara;
    private bool sincronizado = false;

    void Start()
    {
        if (camara == null && Camera.main != null)
            camara = Camera.main.transform;

        if (camara != null)
            ultimaPosicionCamara = camara.position;
    }

    void LateUpdate()
    {
        if (camara == null) return;

        if (!sincronizado)
        {
            ultimaPosicionCamara = camara.position;
            sincronizado = true;
            return;
        }

        Vector3 movimientoCamara = camara.position - ultimaPosicionCamara;

        transform.position += new Vector3(
            movimientoCamara.x * velocidadHorizontal,
            movimientoCamara.y * velocidadVertical,
            0
        );

        ultimaPosicionCamara = camara.position;
    }

    // Permite sincronizar la cámara justo antes de encender el parallax
    public void ReiniciarPosicionCamara()
    {
        if (camara == null && Camera.main != null)
            camara = Camera.main.transform;

        sincronizado = false;
    }
}