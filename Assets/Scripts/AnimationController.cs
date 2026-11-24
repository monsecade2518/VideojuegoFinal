using UnityEngine;
using UnityEngine.InputSystem;

public class ControlPersonaje2D : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;

    [Header("Nombres de los estados en el Animator")]
    public string animIdleDer = "IdleDer";
    public string animIdleIzq = "IdleIzq";
    public string animCaminarDer = "caminadoDer";
    public string animCaminarIzq = "caminadoIzq";

    private Animator animator;
    private int ultimaDireccion = 1; // 1 = derecha, -1 = izquierda
    private string animActual = "";

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float moverX = 0f;

        if (Keyboard.current != null)
        {
            bool derecha = Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed;
            bool izquierda = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed;

            if (derecha) moverX += 1f;
            if (izquierda) moverX -= 1f;
        }

        // Mueve el objeto en X
        transform.position += new Vector3(moverX * velocidad * Time.deltaTime, 0f, 0f);

        // Decide qué animación toca
        if (moverX > 0f)
        {
            ultimaDireccion = 1;
            ReproducirAnimacion(animCaminarDer);
        }
        else if (moverX < 0f)
        {
            ultimaDireccion = -1;
            ReproducirAnimacion(animCaminarIzq);
        }
        else
        {
            ReproducirAnimacion(ultimaDireccion < 0 ? animIdleIzq : animIdleDer);
        }
    }

    void ReproducirAnimacion(string nuevaAnim)
    {
        if (animActual == nuevaAnim) return;

        animActual = nuevaAnim;
        animator.Play(nuevaAnim);
    }
}
