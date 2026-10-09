using UnityEngine;

public class DamageDealedText : MonoBehaviour
{
    [SerializeField] private GameObject damageDealedText;
    private Health health;
    void Start()
    {
        health = GetComponent<Health>();
        health.DamageAplied += CreateDamageText;
    }

    void CreateDamageText(float damage)
    {
        GameObject text = Instantiate(damageDealedText);
        text.transform.position = transform.position;
        var textController = text.GetComponent<DamageDealedTextController>();
        textController.Setup(damage);
    }

    void OnDestroy()
    {
        if (health != null)
        {
            health.DamageAplied -= CreateDamageText;
        }
    }
}
