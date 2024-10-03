using System.Collections;
using UnityEngine;

public class Kitchen : MonoBehaviour
{
    [SerializeField] private float radius = 140f;
    [SerializeField] private float timeToCook = 5f;
    [SerializeField] private float damagePerHit = 5f;
    [SerializeField] private int hitPerSecond = 5;
    [SerializeField] private AudioClip cookingSound;
    [SerializeField] private AudioClip cutitngSound;

    private SpriteRenderer spriteRenderer;

    public static void Create(Vector3 position)
    {
        Transform kitchenTransform = Instantiate(GameAssets.i.pfKitchen, position, Quaternion.identity);
        Kitchen kitchen = kitchenTransform.GetComponent<Kitchen>();
    }
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        AudioManager.instance.PlayAudioClip(cookingSound);
        StartCoroutine(Rise());
    }

    private IEnumerator Rise()
    {

        float elapsedTime = 0;
        float riseTime = 5f;
        Color color = new Color(1,1,1,0);
        yield return new WaitForSeconds(5f);
        spriteRenderer.enabled = true;
        while (elapsedTime < riseTime)
        {
            color.a = Mathf.Lerp(0, 1, elapsedTime / riseTime);
            spriteRenderer.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(StartCooking());
    }
    private IEnumerator StartCooking()
    {
        float elapsedTime = 0;
        float intervalBetweenHits = 1f / hitPerSecond;
        AudioManager.instance.PlaySlashes(cutitngSound);
        while (elapsedTime < timeToCook)
        {
            CutAndChop();
            yield return new WaitForSeconds(intervalBetweenHits);
            elapsedTime += intervalBetweenHits;
        }
        Collider2D[] entities = GetEntitiesInRadius();
        foreach (Collider2D entity in entities)
        {
            IDamageable damageable = entity.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.InKitchen(false);
            }
        }
        gameObject.SetActive(false);
        AudioManager.instance.StopSlashes();
    }

    private void CutAndChop()
    {
        Collider2D[] entities = GetEntitiesInRadius();
        foreach (Collider2D entity in entities)
        {
            IDamageable damageable = entity.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(Random.Range(damagePerHit - 2, damagePerHit + 5));
                damageable.InKitchen(true);
            }
        }
    }

    private Collider2D[] GetEntitiesInRadius()
    {
        return Physics2D.OverlapCircleAll(transform.position, radius);
    }
}
