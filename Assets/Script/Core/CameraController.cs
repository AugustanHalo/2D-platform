using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float speed;
    private float currentPosX;
    private Vector3 velocity = Vector3.zero;

    //Follow Camera
    [SerializeField] private Transform player;
    [SerializeField] private float aHeadDistance;
    [SerializeField] private PlayerMovememt playerMovememt;
    private float lookAhead;

    private void Update()
    {
        //Room Camera
        //transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPosX, transform.position.y, transform.position.z), ref velocity, speed );

        //Follow Camera
       
        if (playerMovememt.IsOnWall())
        {
            lookAhead = 0;
        }
        else
        {
            transform.position = new Vector3(player.position.x + lookAhead, transform.position.y, transform.position.z);
            lookAhead = Mathf.Lerp(lookAhead, aHeadDistance * player.localScale.x, Time.deltaTime * speed);
        }
    }

    public void MoveToNewRoom(Transform _newRoom)
    {
        currentPosX = _newRoom.position.x;
    }
}
