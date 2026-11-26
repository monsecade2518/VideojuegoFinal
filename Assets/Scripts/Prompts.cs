using System;
using System.Collections.Generic;
using UnityEngine;

public class Prompts : MonoBehaviour
{
    private enum Modo
    {
        Panel,
        AlEntrar
    }

    private const float Margen = 20f;
    private const float Separacion = 8f;
    private const float DuracionEntrada = 0.32f;
    private const float IntervaloEntrada = 0.16f;

    [SerializeField] private Modo modo = Modo.Panel;
    [SerializeField, Tooltip("Asigna el componente Prompts en modo Panel de esta sala.")]
    private Prompts panelDestino = null;
    [SerializeField, TextArea(2, 5)] private string texto = string.Empty;

    private readonly List<Instruccion> instrucciones = new List<Instruccion>();
    private GUIStyle estiloTexto;
    private Vector2 desplazamiento;
    private bool bajarAlUltimo;
    private bool mostrado;
    private Func<bool> puedeMostrar;

    private void Start()
    {
        if (modo != Modo.Panel || string.IsNullOrWhiteSpace(texto)) return;

        foreach (string linea in texto.Split('\n'))
            Mostrar(linea);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (modo != Modo.AlEntrar || mostrado || !isActiveAndEnabled || !other.CompareTag("Player")) return;
        if (panelDestino == null || panelDestino.modo != Modo.Panel || string.IsNullOrWhiteSpace(texto)) return;
        if (puedeMostrar != null && !puedeMostrar()) return;

        panelDestino.Mostrar(texto);
        mostrado = true;
    }

    public void ConfigurarAvisoAlEntrar(Prompts panel, string mensaje, Func<bool> condicion)
    {
        modo = Modo.AlEntrar;
        panelDestino = panel;
        texto = mensaje;
        puedeMostrar = condicion;
    }

    public void Mostrar(string mensaje)
    {
        if (modo != Modo.Panel || string.IsNullOrWhiteSpace(mensaje)) return;

        float inicio = Time.unscaledTime;
        if (instrucciones.Count > 0)
            inicio = Mathf.Max(inicio, instrucciones[^1].Inicio + IntervaloEntrada);

        instrucciones.Add(new Instruccion(mensaje.Trim(), inicio));
        bajarAlUltimo = true;
    }

    private void OnGUI()
    {
        if (instrucciones.Count == 0) return;

        float anchoPanel = Mathf.Min(420f, Screen.width - Margen * 2f);
        float altoMaximo = Screen.height * 0.45f;
        if (anchoPanel <= 60f || altoMaximo <= 0f) return;

        estiloTexto ??= new GUIStyle(GUI.skin.label)
        {
            fontSize = 17,
            alignment = TextAnchor.MiddleLeft,
            wordWrap = true,
            richText = false,
            padding = new RectOffset(18, 14, 10, 10),
            normal = { textColor = new Color(0.95f, 0.97f, 0.98f, 1f) }
        };

        float anchoTarjeta = anchoPanel;
        float altoContenido = MedirContenido(anchoTarjeta);
        if (altoContenido > altoMaximo)
        {
            anchoTarjeta -= GUI.skin.verticalScrollbar.fixedWidth + 4f;
            altoContenido = MedirContenido(anchoTarjeta);
        }

        float altoVisible = Mathf.Min(altoContenido, altoMaximo);
        if (bajarAlUltimo)
        {
            desplazamiento.y = altoContenido - altoVisible;
            bajarAlUltimo = false;
        }

        Rect panel = new Rect(Margen, Screen.height - Margen - altoVisible, anchoPanel, altoVisible);
        Rect contenido = new Rect(0f, 0f, anchoTarjeta, altoContenido);
        desplazamiento = GUI.BeginScrollView(panel, desplazamiento, contenido, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);

        if (Event.current.type == EventType.Repaint)
        {
            float y = 0f;
            foreach (Instruccion instruccion in instrucciones)
            {
                Dibujar(instruccion, new Rect(0f, y, anchoTarjeta, instruccion.Alto));
                y += instruccion.Alto + Separacion;
            }
        }

        GUI.EndScrollView();
    }

    private float MedirContenido(float ancho)
    {
        float alto = -Separacion;
        foreach (Instruccion instruccion in instrucciones)
        {
            instruccion.Alto = Mathf.Max(42f, estiloTexto.CalcHeight(instruccion.Contenido, ancho));
            alto += instruccion.Alto + Separacion;
        }

        return alto;
    }

    private void Dibujar(Instruccion instruccion, Rect tarjeta)
    {
        float avance = Mathf.Clamp01((Time.unscaledTime - instruccion.Inicio) / DuracionEntrada);
        if (avance <= 0f) return;

        float entrada = 1f - Mathf.Pow(1f - avance, 3f);
        tarjeta.x = Mathf.Lerp(-24f, 0f, entrada);

        Color colorAnterior = GUI.color;
        GUI.color = new Color(0.14f, 0.20f, 0.27f, 0.5f * entrada);
        GUI.DrawTexture(tarjeta, Texture2D.whiteTexture);
        GUI.color = new Color(0.40f, 0.76f, 0.88f, 1f * entrada);
        GUI.DrawTexture(new Rect(tarjeta.x, tarjeta.y, 4f, tarjeta.height), Texture2D.whiteTexture);
        GUI.color = new Color(1f, 1f, 1f, entrada);
        GUI.Label(tarjeta, instruccion.Contenido, estiloTexto);
        GUI.color = colorAnterior;
    }

    private sealed class Instruccion
    {
        public GUIContent Contenido { get; }
        public float Inicio { get; }
        public float Alto { get; set; }

        public Instruccion(string mensaje, float inicio)
        {
            Contenido = new GUIContent(mensaje);
            Inicio = inicio;
        }
    }
}
