using System.Collections;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    [SerializeField] private float damage;

    [Header ("Fire Trap Timer")]
    [SerializeField] private float fireTrapTimer;
    [SerializeField] private float activeTime;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Health player;

    [Header("SFX")]
    [SerializeField] private AudioClip fireTrapSound;

    private bool isActive;
    private bool isTriggered;
    private bool dealDamage = false;

    private void Awake()
    {
        isActive = false;
        isTriggered = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if(!isTriggered)
            {
                //trigger the fire trap
                StartCoroutine(ActiveFireTrap());

                
            }
            player = collision.GetComponent<Health>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        player = null;
    }

    private void Update()
    {
        if (isActive && player != null && dealDamage)
        {
            player.TakeDamage(damage);
            dealDamage = false;
        }

    }

    private IEnumerator ActiveFireTrap()
    {
        //turn on sprite to red to alert player
        isTriggered = true;
        spriteRenderer.color = Color.red;

        //wait for the fire trap to be active
        yield return new WaitForSeconds(fireTrapTimer);
        animator.SetBool("active", true);

        //activate the fire trap
        isActive = true;
        dealDamage = true;
        spriteRenderer.color = Color.white;
        if(fireTrapSound)
        {
            AudioManager.instance.PlayAudioClip(fireTrapSound);
        }
        yield return new WaitForSeconds(activeTime);


        //turn off the fire trap
        animator.SetBool("active", false);
        isActive = false;
        isTriggered = false;
    }

}
