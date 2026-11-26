using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SiguienteRoom : MonoBehaviour
{
    [SerializeField] private int puntosNecesarios = 120;
    [SerializeField] private Prompts panelPrompts;

    private Collider2D portalCollider;
    private GameObject avisoPortal;
    private bool portalActivo = false;

    private void Awake()
    {
        portalCollider = GetComponent<Collider2D>();

        // Apagamos el collider al iniciar el room para que el player no pueda activarlo aún
        portalCollider.enabled = false;

        if (portalCollider is BoxCollider2D cajaPortal)
            CrearAvisoPortal(cajaPortal);
    }

    private void Update()
    {
        // Si ya se abrió, no hace falta seguir comprobando
        if (portalActivo) return;

        if (TienePuntosSuficientes())
        {
            ActivarPortal();
        }
    }

    private bool TienePuntosSuficientes()
    {
        return XPpointsManager.Instance != null && XPpointsManager.Instance.AlcanzoMeta(puntosNecesarios);
    }

    private void ActivarPortal()
    {
        portalActivo = true;
        if (avisoPortal != null)
            avisoPortal.SetActive(false);

        portalCollider.enabled = true; // Ahora el Player ya puede tocarlo y cambiar de escena
        Debug.Log("¡Portal activado!");
    }

    private void CrearAvisoPortal(BoxCollider2D cajaPortal)
    {
        Prompts panel = panelPrompts != null ? panelPrompts : gameObject.AddComponent<Prompts>();
        avisoPortal = new GameObject("Aviso de portal bloqueado");
        avisoPortal.layer = gameObject.layer;
        avisoPortal.transform.SetParent(transform, false);

        BoxCollider2D sensor = avisoPortal.AddComponent<BoxCollider2D>();
        sensor.isTrigger = true;
        sensor.offset = cajaPortal.offset;
        sensor.size = cajaPortal.size;
        sensor.edgeRadius = cajaPortal.edgeRadius;

        Prompts aviso = avisoPortal.AddComponent<Prompts>();
        aviso.ConfigurarAvisoAlEntrar(
            panel, "You must kill all the enemies to go through the portal!", () => isActiveAndEnabled && !TienePuntosSuficientes());
    }
}
