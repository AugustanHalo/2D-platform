using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform preRoom;
    [SerializeField] private Transform nextRoom;
    [SerializeField] private CameraController cameraController;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.transform.position.x < transform.position.x)
            {
                cameraController.MoveToNewRoom(nextRoom);
                nextRoom.GetComponent<Room>().ActivateRoom(true);
                preRoom.GetComponent<Room>().ActivateRoom(false);

            }
            else
            {
                cameraController.MoveToNewRoom(preRoom);
                nextRoom.GetComponent<Room>().ActivateRoom(false);
                preRoom.GetComponent<Room>().ActivateRoom(true);
            }
        }
    }
}
