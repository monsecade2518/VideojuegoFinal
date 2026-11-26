using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class MobsAnimation : MonoBehaviour
{
    private static readonly int CaminadoDer = Animator.StringToHash("Base Layer.caminadoDer");
    private static readonly int CaminadoIzq = Animator.StringToHash("Base Layer.caminadoIzq");
    private static readonly int MuerteDer = Animator.StringToHash("Base Layer.muerteDer");
    private static readonly int MuerteIzq = Animator.StringToHash("Base Layer.muerteIzq");

    [SerializeField] private Animator visual;

    [Header("Movimiento")] 
    [SerializeField] private float velocidad = 3f;

    [Header("Combate")]
    [SerializeField] private float fuerzaRebote = 8f;
    private bool move = false;
    private bool mirandoDerecha;
    private int estadoCaminado;
    private int estadoMuerte;

     private Rigidbody2D rb;

     private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!CompareTag("Mob"))
            visual = null;
    }

    private void OnEnable()
    {
        if (estadoMuerte != 0)
            rb.simulated = true;

        estadoMuerte = 0;
        estadoCaminado = 0;
    }

    private void FixedUpdate()
    {
        if (estadoMuerte != 0)
            return;

        float velocidadX = move ? velocidad : -velocidad;
        rb.linearVelocityX = velocidadX;
        if (visual == null)
            return;

        if (velocidadX != 0f)
            mirandoDerecha = velocidadX > 0f;

        int estado = mirandoDerecha ? CaminadoDer : CaminadoIzq;
        if (estado == estadoCaminado)
            return;

        estadoCaminado = estado;
        visual.Play(estado, 0, 0f);
    }

    private void LateUpdate()
    {
        if (estadoMuerte == 0)
            return;

        if (visual == null || !visual.isActiveAndEnabled || visual.runtimeAnimatorController == null)
        {
            gameObject.SetActive(false);
            return;
        }

        AnimatorStateInfo estado = visual.GetCurrentAnimatorStateInfo(0);
        if (estado.fullPathHash == estadoMuerte && estado.normalizedTime >= 1f)
            gameObject.SetActive(false);
    }

    private void MorirPorPisoton()
    {
        int estado = mirandoDerecha ? MuerteDer : MuerteIzq;
        if (visual == null || !visual.isActiveAndEnabled || visual.runtimeAnimatorController == null ||
            !visual.HasState(0, estado))
        {
            GetComponent<MobXP>()?.SoltarXP();
            gameObject.SetActive(false);
            return;
        }

        estadoMuerte = estado;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.simulated = false;
        GetComponent<MobXP>()?.SoltarXP();
        visual.Play(estado, 0, 0f);
    }

    //Detecta plantas y paredes invisibles
    private void OnTriggerEnter2D(Collider2D other){
            if (estadoMuerte != 0)
                return;

            if (other.CompareTag("InvisibleWall")){
                move = !move;
            }

            if (gameObject.CompareTag("Mob2")){
                if (other.CompareTag("Plant")){
                    move = !move;
                    GetComponent<MobXP>()?.SoltarXP();
                    gameObject.SetActive(false);
                }
            }

        }

    // Detecta el choque físico con el jugador
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (estadoMuerte != 0)
            return;

        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

        if (collision.gameObject.CompareTag("Player"))
        {
            LivesManager vidas = LivesManager.Instance;
            if (vidas?.ReiniciandoEscena == true)
                return;

            Vector2 normal = collision.GetContact(0).normal;

            if ((gameObject.CompareTag("Mob") ||gameObject.CompareTag("BossFinal")) && normal.y < -0.5f)
            {
                if (playerRb != null)
                {
                    playerRb.linearVelocityY = fuerzaRebote;
                }
                MorirPorPisoton();
                Debug.Log("El jugador mató a mob");
            }
            else
            {
                if (gameObject.CompareTag("Mob")){

                    vidas.RestarVidas(1);
                    Debug.Log("El jugador recibió daño MENOS 1 VIDA.");
                }

                if (gameObject.CompareTag("BossFinal")){

                    vidas.RestarVidas(2);
                    Debug.Log("El jugador recibió daño MENOS 2 VIDAS.");
                }


                else if (gameObject.CompareTag("Mob2")){

                    vidas.RestarVidas(2);
                    Debug.Log("El jugador recibió daño MENOS 2 VIDAS.");
                }

                if (vidas?.ReiniciandoEscena == true)
                    return;

                collision.gameObject.GetComponent<PlayerSidescroller>().knockbackTimer = 0.2f;
                // Si el jugador está a la izquierda del mob, empuja a la izquierda (-1), si no, a la derecha (1)
                float direccion = collision.transform.position.x < transform.position.x ? -1f : 1f;

                // Empuje hacia atrás y un poco hacia arriba para evitar la fricción del suelo
                playerRb.linearVelocity = new Vector2(direccion * fuerzaRebote, fuerzaRebote * 0.6f);
            }
        }
    }
}
