using Unity.VisualScripting;
using UnityEngine;

public class Patroling : MonoBehaviour
{
    [Header("Patroling")]
    [SerializeField] private float speed;
    [SerializeField] private float leftBound;
    [SerializeField] private float rightBound;
    private bool movingRight = false;

    private Animator animator;
   

    [Header ("Idle Behaviour")]
    [SerializeField] private float idleTime;
    private float idleTimer;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        leftBound = transform.position.x - leftBound;
        rightBound = transform.position.x + rightBound;
    }

    private void OnDisable()
    {
        animator.SetBool("run", false);
        
    }

    private void Update()
    {
        if (!movingRight)
        {
            if (transform.position.x >= leftBound)
            {
                MoveInDirection(-1);
            }
            else
            {
                DirectionChange();
            }
        }
        else
        {
            if (transform.position.x <= rightBound)
            {
                MoveInDirection(1);
            }
            else
            {
                DirectionChange();
            }
        }
    }

    private void DirectionChange()
    {
        animator.SetBool("run", false);
        
        idleTimer += Time.deltaTime;

        if (idleTimer > idleTime)
        {
            movingRight = !movingRight;
        }
    }

    private void MoveInDirection(int _direction)
    {
        idleTimer = 0;
        animator.SetBool("run", true);

        //Make enemy face direction
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * _direction,
            transform.localScale.y, transform.localScale.z);

        //Move in that direction
        transform.position = new Vector3(transform.position.x + Time.deltaTime * _direction * speed,
            transform.position.y, transform.position.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position,new Vector3(rightBound, transform.position.y, transform.position.z));
        Gizmos.DrawLine(transform.position,new Vector3(leftBound, transform.position.y, transform.position.z));
    }
}
