using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;

    [Header("Nombres de los estados en el Animator")]
    public string animIdleDer = "IdleDer";
    public string animIdleIzq = "IdleIzq";
    public string animCaminarDer = "caminadoDer";
    public string animCaminarIzq = "caminadoIzq";

    public string animSaltoIzq = "saltoIzq";

    public string animSaltoDer = "SaltoDer";

    private Animator animator;
    private PlayerSidescroller jugador;
    private int ultimaDireccion = 1; // 1 = derecha, -1 = izquierda
    private string animActual = "";

    void Awake()
    {
        animator = GetComponent<Animator>();
        jugador = GetComponentInParent<PlayerSidescroller>();
    }

    void LateUpdate()
    {
        float moverX = InputHelper.Horizontal();

        if (moverX != 0f)
            ultimaDireccion = moverX > 0f ? 1 : -1;

        if (jugador != null && jugador.IsJumping)
        {
            ReproducirAnimacion(ultimaDireccion < 0 ? animSaltoIzq : animSaltoDer);
            return;
        }

        if (moverX != 0f)
            ReproducirAnimacion(ultimaDireccion < 0 ? animCaminarIzq : animCaminarDer);
        else
            ReproducirAnimacion(ultimaDireccion < 0 ? animIdleIzq : animIdleDer);
    }

    void ReproducirAnimacion(string nuevaAnim)
    {
        if (animActual == nuevaAnim) return;

        animActual = nuevaAnim;
        animator.Play(nuevaAnim, 0, 0f);
    }
}
