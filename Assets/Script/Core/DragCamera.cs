using UnityEngine;

public class DragCamera : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float zoomStep = 5f;
    [SerializeField] private float minCamSize = 60f;
    [SerializeField] private float maxCamSize = 100f;

    //[SerializeField] private float mapMinX = 0f;
    //[SerializeField] private float mapMaxX = 100f;
    //[SerializeField] private float mapMinY = 0f;
    //[SerializeField] private float mapMaxY = 100f;

    private Vector3 origin;
    private Vector3 diff;
    private Vector3 resetCamera;

    private bool drag = false;

    private void Start()
    {
        resetCamera = Camera.main.transform.position;
    }
    
    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;
            if(drag == false)
            {
                drag = true;
                origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
               drag = false;
        }

        if (drag)
        {
            Camera.main.transform.position = origin - diff;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Camera.main.transform.position = resetCamera;
        }

        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            ZoomIn();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            ZoomOut();
        }
    }
    public void ZoomIn()
    {
        if(cam.orthographicSize <= minCamSize) return;
        float newSize = cam.orthographicSize - zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);

    }

    public void ZoomOut()
    {
        if(cam.orthographicSize >= maxCamSize) return;
        float newSize = cam.orthographicSize + zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);

    }
    
}
