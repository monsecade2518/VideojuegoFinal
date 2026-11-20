using UnityEngine;

public class MobXP : MonoBehaviour
{
    [SerializeField] private GameObject prefabCosita;
    [SerializeField] private GameObject mob;

    public void SoltarXP()
    {

        float impulsoVertical = 5f;
        float dispersionHorizontal = 5f;

        if (prefabCosita == null) return;

        int cantidad = 0;
        if (mob.CompareTag("Mob") || mob.CompareTag("BossFinal"))
        {
            cantidad = 1;
        }

        else if (mob.CompareTag("Mob2"))
        {
            cantidad = 5;
        }

        for (int i = 0; i < cantidad; i++)
        {
            GameObject cosita = Instantiate(prefabCosita, transform.position, Quaternion.identity);
            
            Rigidbody2D rb = cosita.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float fuerzaX = Random.Range(-dispersionHorizontal, dispersionHorizontal);
                float fuerzaY = Random.Range(impulsoVertical * 0.8f, impulsoVertical * 1.2f);
                rb.AddForce(new Vector2(fuerzaX, fuerzaY), ForceMode2D.Impulse);
            }
        }
    }
}