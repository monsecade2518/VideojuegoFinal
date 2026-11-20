using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    private bool cargandoEscena = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (cargandoEscena) return;

        if (other.CompareTag("Portal"))
        {
            cargandoEscena = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (cargandoEscena) return;

        if (other.CompareTag("LevelBound"))
        {
            LivesManager.Instance.RestarVidas(3);
            cargandoEscena = true;
            LivesManager.Instance.MatarPersonaje();
        }
    }
}