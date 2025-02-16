using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private float healthAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Health>().Heal(healthAmount);
            gameObject.SetActive(false);
        }
    }
    
}
