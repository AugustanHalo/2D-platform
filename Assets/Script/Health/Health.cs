using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header ("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool isDead;

    [Header("Invulnerability")]
    [SerializeField] private float invulnerabilityTime;
    [SerializeField] private int numOfFlash;
    private SpriteRenderer spriteRenderer;
    public bool isInvulnerable { get; private set; }

    [Header("Component")]
    [SerializeField] private Behaviour[] DisbledComponents;

    [Header("Sounds")]
    [SerializeField] private AudioClip takeDamageSound;
    [SerializeField] private AudioClip deathSound;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if(currentHealth > 0) //If the player is still alive
        {
            anim.SetTrigger("TakeDamage");
            if(takeDamageSound)
            {
                AudioManager.instance.PlayAudioClip(takeDamageSound);
            }
            StartCoroutine(Invulnerabity());
        }
        else //If the player is dead
        {
            if(!isDead)
            {
                isDead = true;
                
                //Disable all the components
                foreach(Behaviour component in DisbledComponents)
                {
                    component.enabled = false;
                }
                anim.SetBool("IsGrounded", true);
                anim.SetTrigger("Die");

                if (deathSound)
                {
                    AudioManager.instance.PlayAudioClip(deathSound);
                }
            }
            
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(1);
        }
    }

    public void Heal(float _healAmount)
    {
        currentHealth = Mathf.Clamp(currentHealth + _healAmount, 0, startingHealth);
    }

    public void Respawn()
    {
        Heal(startingHealth);
        isDead = false;
        anim.ResetTrigger("Die");
        //Active all the components
        foreach (Behaviour component in DisbledComponents)
        {
            component.enabled = true;
        }
        anim.Play("Idle"); 
        StartCoroutine(Invulnerabity());

    }

    private IEnumerator Invulnerabity()
    {
        isInvulnerable = true;
        if(CompareTag("Player"))
        {
            Physics2D.IgnoreLayerCollision(8, 9, true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(9,10, true);
        }
        for (int i = 0; i < numOfFlash; i++)
        {
            spriteRenderer.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(invulnerabilityTime / (numOfFlash *2));
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(invulnerabilityTime / (numOfFlash * 2));
        }
        if(CompareTag("Player"))
        Physics2D.IgnoreLayerCollision(8, 9, false);
        else
        Physics2D.IgnoreLayerCollision(9, 10, false);
        isInvulnerable = false;
    }
}
