using UnityEngine;

public class SpikeHead : Enemy_Damage
{
    [Header ("Spike Head Attribute")]
    [SerializeField] private float range;
    [SerializeField] private float speed;
    [SerializeField] private float delayTime;
    [SerializeField] private LayerMask playerLayer;

    [Header("SFX")]
    [SerializeField] private AudioClip spikeHeadSound;
    private float delayCheckTimer;
    private Vector3 destination;
    private bool isAttacking;
    private Vector3[] directions = new Vector3[4];



    private void OnEnable()
    {
        Stop();
    }
    private void Update()
    {
        if(isAttacking)
        {
            transform.Translate(destination * speed * Time.deltaTime);
        }
        else
        {
            delayCheckTimer += Time.deltaTime;
            if (delayCheckTimer > delayTime)
            {
                CheckPlayerInRange();             
            }
        }

    }

    private void CheckPlayerInRange()
    {
        CalculateDirection();

        //Check if player is in range
        for(int i = 0; i < directions.Length; i++)
        {
            Debug.DrawRay(transform.position, directions[i], Color.red);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i], range, playerLayer);
            if (hit.collider != null && !isAttacking)
            {
                destination = directions[i];
                isAttacking = true;
                delayCheckTimer = 0;
                break;
            }
        }
    }

    private void CalculateDirection()
    {
        directions[0] = transform.right * range; //Right direction
        directions[1] = -transform.right * range; //Left direction
        directions[2] = transform.up * range; //Up direction
        directions[3] = -transform.up * range; //Down direction
    }

    private void Stop()
    {
        destination = transform.position; //Set destination to current position so that not moving
        isAttacking = false;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if(spikeHeadSound)
        {
            AudioManager.instance.PlayAudioClip(spikeHeadSound);
        }
        //Stop moving when hit something
        Stop();
    }
}
