using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Save : MonoBehaviour
{
     private Rigidbody2D rb;

     private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate(){
        
    }

    // Detecta el choque físico con el jugador
    private void OnCollisionEnter2D(Collision2D collision)
    {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (collision.gameObject.CompareTag("Player"))
            {
                LivesManager.Instance.SumarVidas(1);
                gameObject.SetActive(false);
                Debug.Log("Se detectó jugador");
            }
               
    }
        
}
