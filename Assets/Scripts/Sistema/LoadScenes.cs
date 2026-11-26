using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    private bool cargandoEscena = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (cargandoEscena || LivesManager.Instance?.ReiniciandoEscena == true) return;

        if (other.CompareTag("Portal"))
        {
            cargandoEscena = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        LivesManager vidas = LivesManager.Instance;
        if (cargandoEscena || vidas == null || vidas.ReiniciandoEscena) return;

        if (other.CompareTag("LevelBound"))
        {
            cargandoEscena = true;
            vidas.ReiniciarPorCaida();
        }
    }
}
