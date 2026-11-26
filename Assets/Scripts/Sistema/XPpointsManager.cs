using UnityEngine;
using UnityEngine.SceneManagement;

public class XPpointsManager : MonoBehaviour
{
    public static XPpointsManager Instance { get; private set; }

    public int puntosActuales = 0;
    private int puntosInicioRoom = 0;
    private string nombreRoomActual = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
    }

    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (escena.name != nombreRoomActual)
        {
            // Entraste a un room nuevo: guardamos el checkpoint de puntos
            nombreRoomActual = escena.name;
            puntosInicioRoom = puntosActuales;
            Debug.Log("Nuevo room: " + nombreRoomActual + ". Puntos guardados: " + puntosInicioRoom);
        }
        else
        {
            // Se recargó el mismo room (el jugador murió): restauramos los puntos
            puntosActuales = puntosInicioRoom;
            Debug.Log("Room reiniciado. Puntos restaurados a: " + puntosActuales);
        }
    }

    public void SumarPuntos(int cantidad)
    {
        puntosActuales += cantidad;
        Debug.Log("Puntos acumulados: " + puntosActuales);
    }

    public bool AlcanzoMeta(int puntosNecesarios)
    {
        //Debug.Log("Puntos actuales: " + puntosActuales + ", Puntos necesarios: " + puntosNecesarios);
        return puntosActuales >= puntosNecesarios;
    }

    // Llama a este método solo al empezar una partida nueva desde el Menú Principal
    public void ReiniciarPartidaCompleta()
    {
        puntosActuales = 0;
        puntosInicioRoom = 0;
        nombreRoomActual = "";
    }

      private void OnGUI()
{
    GUIStyle estilo = new GUIStyle(GUI.skin.label);
    estilo.fontSize = 28;
    estilo.normal.textColor = Color.white;

    // Dibuja el texto en la esquina superior izquierda (x: 20, y: 20)
    GUI.Label(new Rect(20, 60, 300, 50), "Puntos: " + puntosActuales, estilo);
}

}