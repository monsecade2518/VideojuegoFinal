
using UnityEngine;

public class ActivadorPlant : MonoBehaviour
{
    [SerializeField] private PlantAnimation PlantAsociar;

    private bool jugadorCerca = false;

    private void Update()
    {
        if (jugadorCerca && InputHelper.PlantsPressed())
        {
            if (PlantAsociar != null)
            {
                PlantAsociar.ActivarPlanta();
                Debug.Log("Planta activada");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            Debug.Log("Jugador cerca de la planta");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            Debug.Log("Jugador no cerca de la planta");
        }
    }
}
