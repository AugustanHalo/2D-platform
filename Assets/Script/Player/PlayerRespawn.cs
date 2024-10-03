using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private AudioClip checkpointSound;
    private UIManager uIManager;
    private Transform currentCheckpoint;
    private Health playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        uIManager = FindObjectOfType<UIManager>();
    }

    public void CheckRespawn()
    {
        if(currentCheckpoint == null) //If there is no checkpoint
        {
            //show screem game over
            uIManager.GameOver();
            return;
        }

        transform.position = currentCheckpoint.position; //Move player to the checkpoint position
        playerHealth.Respawn(); // restore player health and enable all the components
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            currentCheckpoint = collision.transform;
            AudioManager.instance.PlayAudioClip(checkpointSound);
            collision.GetComponent<Collider2D>().enabled = false;
            collision.GetComponent<Animator>().SetTrigger("appear");
        }
    }
}
