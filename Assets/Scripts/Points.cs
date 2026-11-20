using UnityEngine;
public class Points : MonoBehaviour
{
    private float contadorTiempo;
    private float tiempoEspera = 0.1f;

    private void Update()
    {
        if (contadorTiempo < tiempoEspera)
            contadorTiempo += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        int valorPuntosfrag = 10;
        int valorPuntosGEMA = 330;
        if (contadorTiempo >= tiempoEspera && other.CompareTag("Player"))
        {
            if (XPpointsManager.Instance != null)
            {
                if (gameObject.CompareTag("Gema"))
                {
                    XPpointsManager.Instance.SumarPuntos(valorPuntosGEMA);
                }
                else if (gameObject.CompareTag("Fragment"))
                {
                    XPpointsManager.Instance.SumarPuntos(valorPuntosfrag);
                }
            }
            Destroy(gameObject);
        }
    }
}