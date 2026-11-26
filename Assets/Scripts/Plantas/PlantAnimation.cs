
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlantAnimation : MonoBehaviour
{
    private static readonly int ActivarAnimacion = Animator.StringToHash("Activar");

    [SerializeField] private float velocidad = 3f;
    [SerializeField] private string tagInvisibleWall = "InvisibleWall";
    [SerializeField] private Animator visual;

    private bool move = false;
    private int dir = 1;
    private int choques = 0;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Este método será llamado desde ActivadorPlant
    public void ActivarPlanta()
    {
        if (!move && visual != null)
            visual.SetTrigger(ActivarAnimacion);

        move = true;
        choques = 0;
        dir = 1;
    }

    private void FixedUpdate()
    {
        if (move)
        {
            rb.linearVelocityX = velocidad * dir;
        }
        else
        {
            rb.linearVelocityX = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagInvisibleWall) && move)
        {
            dir = -dir;
            choques++;

            if (choques >= 2)
            {
                move = false;
            }
        }
    }
}
