using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class MobsAnimation : MonoBehaviour
{

    [Header("Movimiento")] 
    [SerializeField] private float velocidad = 3f;

    [Header("Combate")]
    [SerializeField] private float fuerzaRebote = 8f;
    private bool move = false;

     private Rigidbody2D rb;

     private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate(){
        if (move)
        {
            rb.linearVelocityX = velocidad;
        }
        else
        {
            rb.linearVelocityX = -velocidad;
        }
    }

    //Detecta plantas y paredes invisibles
    private void OnTriggerEnter2D(Collider2D other){
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
            
        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 normal = collision.GetContact(0).normal;

            if (gameObject.CompareTag("Mob") && normal.y < -0.5f)
            {
                if (playerRb != null)
                {
                    playerRb.linearVelocityY = fuerzaRebote;
                }
                GetComponent<MobXP>()?.SoltarXP();
                gameObject.SetActive(false);
                Debug.Log("El jugador mató a mob");
            }
            else
            {
                if (gameObject.CompareTag("Mob")){

                    LivesManager.Instance.RestarVidas(1);
                    Debug.Log("El jugador recibió daño MENOS 1 VIDA.");
                }

                else if (gameObject.CompareTag("Mob2")){

                    LivesManager.Instance.RestarVidas(2);
                    Debug.Log("El jugador recibió daño MENOS 2 VIDAS.");
                }

                collision.gameObject.GetComponent<PlayerSidescroller>().knockbackTimer = 0.2f;
                // Si el jugador está a la izquierda del mob, empuja a la izquierda (-1), si no, a la derecha (1)
                float direccion = collision.transform.position.x < transform.position.x ? -1f : 1f;

                // Empuje hacia atrás y un poco hacia arriba para evitar la fricción del suelo
                playerRb.linearVelocity = new Vector2(direccion * fuerzaRebote, fuerzaRebote * 0.6f);
            }
        }
    }
}
