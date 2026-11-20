using UnityEngine;

public class ActivadorLiana : MonoBehaviour
{
    [SerializeField] private AnimationVine lianaAAsociar;
    private bool jugadorCerca = false;

    private void Update()
    {
        if (jugadorCerca && InputHelper.VinesPressed())
        {
            if (lianaAAsociar != null)
            {
                lianaAAsociar.Activar();
                jugadorCerca = false;
                Debug.Log("Jugador no cerca de la liana");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            Debug.Log("Jugador cerca de la liana");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            Debug.Log("Jugador no cerca de la liana");
        }
    }
}