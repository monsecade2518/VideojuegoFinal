using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SiguienteRoom : MonoBehaviour
{
    [SerializeField] private int puntosNecesarios = 120;
    [SerializeField] private float opacidadBloqueado = 0.35f; // Qué tan transparente se ve cerrado

    private Collider2D portalCollider;
    private SpriteRenderer spriteRenderer;
    private bool portalActivo = false;

    private void Awake()
    {
        portalCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Apagamos el collider al iniciar el room para que el player no pueda activarlo aún
        portalCollider.enabled = false;
        CambiarOpacidad(opacidadBloqueado);
    }

    private void Update()
    {
        // Si ya se abrió, no hace falta seguir comprobando
        if (portalActivo) return;

        if (XPpointsManager.Instance != null && XPpointsManager.Instance.AlcanzoMeta(puntosNecesarios))
        {
            ActivarPortal();
        }
    }

    private void ActivarPortal()
    {
        portalActivo = true;
        portalCollider.enabled = true; // Ahora el Player ya puede tocarlo y cambiar de escena
        CambiarOpacidad(1f); // Opacidad al 100%
        Debug.Log("¡Portal activado!");
    }

    private void CambiarOpacidad(float alpha)
    {
        if (spriteRenderer == null) return;

        Color colorActual = spriteRenderer.color;
        colorActual.a = alpha;
        spriteRenderer.color = colorActual;
    }
}