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

    [SerializeField] private string animDanoDer = "DanoDer";
    [SerializeField] private string animDanoIzq = "DanoIzq";
    [SerializeField] private string animMuerteDer = "MuerteDer";
    [SerializeField] private string animMuerteIzq = "MuerteIzq";

    private Animator animator;
    private PlayerSidescroller jugador;
    private int ultimaDireccion = 1; // 1 = derecha, -1 = izquierda
    private string animActual = "";
    private int estadoReaccion;
    private int fotogramaInicioReaccion;
    private bool muerto;
    private bool muerteTerminada;

    void Awake()
    {
        animator = GetComponent<Animator>();
        jugador = GetComponentInParent<PlayerSidescroller>();
    }

    void LateUpdate()
    {
        if (muerto)
        {
            // LateUpdate lee el estado después de que el Animator haya evaluado el fotograma.
            if (!muerteTerminada && Time.frameCount != fotogramaInicioReaccion)
                muerteTerminada = !ReaccionEnCurso;

            return;
        }

        if (ReaccionEnCurso)
            return;

        estadoReaccion = 0;
        float moverX = InputHelper.Horizontal();

        if (jugador != null)
            ultimaDireccion = jugador.FacingDirection;
        else if (moverX != 0f)
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

    public void ReproducirDano()
    {
        if (!muerto)
            IniciarReaccion(animDanoDer, animDanoIzq);
    }

    public bool ReproducirMuerte()
    {
        if (!IniciarReaccion(animMuerteDer, animMuerteIzq))
            return false;

        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        muerto = true;
        return true;
    }

    public bool MuerteEnCurso => muerto && !muerteTerminada && isActiveAndEnabled &&
        animator != null && animator.isActiveAndEnabled;

    private bool ReaccionEnCurso
    {
        get
        {
            if (estadoReaccion == 0 || !isActiveAndEnabled || animator == null || !animator.isActiveAndEnabled ||
                animator.runtimeAnimatorController == null)
                return false;

            // Animator.Play puede tardar hasta la siguiente evaluación del Animator.
            if (Time.frameCount == fotogramaInicioReaccion)
                return true;

            AnimatorStateInfo estado = animator.GetCurrentAnimatorStateInfo(0);
            return estado.fullPathHash == estadoReaccion && estado.normalizedTime < 1f;
        }
    }

    private bool IniciarReaccion(string derecha, string izquierda)
    {
        if (!isActiveAndEnabled || animator == null || !animator.isActiveAndEnabled ||
            animator.runtimeAnimatorController == null || animator.layerCount == 0)
            return false;

        int direccion = jugador != null ? jugador.FacingDirection : ultimaDireccion;
        string nombre = direccion < 0 ? izquierda : derecha;
        int estado = Animator.StringToHash(animator.GetLayerName(0) + "." + nombre);
        if (!animator.HasState(0, estado))
            return false;

        estadoReaccion = estado;
        fotogramaInicioReaccion = Time.frameCount;
        animActual = nombre;
        animator.Play(estado, 0, 0f);
        return true;
    }

    void ReproducirAnimacion(string nuevaAnim)
    {
        if (animActual == nuevaAnim) return;

        animActual = nuevaAnim;
        animator.Play(nuevaAnim, 0, 0f);
    }
}
