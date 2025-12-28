using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private float lookSpeedH = 2f;

    [SerializeField]
    private float lookSpeedV = 2f;

    [SerializeField]
    private float zoomSpeed = 2f;

    [SerializeField]
    private float dragSpeed = 3f;

    private static float defPosZ = -25f;

    private float yaw = 0f;
    private float pitch = 0f;

    private Camera currentCamera;


    public void SetDefaultRoomPosition()
    {
        float xxx = SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridX() * RoomData.defaultRoomWidth + SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomWidth() / 2 - 0.5f;
        float yyy = SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridY() * RoomData.defaultRoomHeight + SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomHeight() / 2 - 0.5f;

        transform.position = new Vector3(xxx, yyy, defPosZ);
    }

    private void Start()
    {
        currentCamera = GetComponent<Camera>();

        // Initialize the correct initial rotation
        this.yaw = this.transform.eulerAngles.y;
        this.pitch = this.transform.eulerAngles.x;

        InitTopViewCamera();
    }


    Vector3 vecPerspectiveAngles = new Vector3(-36, 45, -60);
    Vector3 vecPerspectivePos = new Vector3(-21, -22, 0);

    public void InitTopViewCamera()
    {
        currentCamera.orthographic = true;
        transform.eulerAngles = Vector3.zero;
        //transform.position = new Vector3(Grid.gridWidth / 2, Grid.gridHeight / 2, defPosZ);
        currentCamera.orthographicSize = 4.2f;
    }

    public void InitIsometricCamera()
    {
        currentCamera.orthographic = false;
        transform.eulerAngles = vecPerspectiveAngles;

        Vector3 pos = new Vector3(Grid.gridWidth / 4, Grid.gridHeight / 4, defPosZ);
        pos += vecPerspectivePos;

        transform.position = pos;
        currentCamera.fieldOfView = 10.0f;
    }

    public void ChangeCamera()
    {
        if (currentCamera.orthographic)
        {
            InitIsometricCamera();
        }
        else
        {
            InitTopViewCamera();
        }
    }

    private void Update()
    {
        /*
        if (orthoCamera.orthographic)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else
        {
            transform.eulerAngles = vecPerspectiveAngles;
            orthoCamera.fieldOfView = 10.0f;
        }
        */

        // Only work with the Left Alt pressed
        //if (Input.GetKey(KeyCode.LeftAlt))
        {
            //Look around with Left Mouse
            /*
            if (Input.GetMouseButton(0))
            {
                this.yaw += this.lookSpeedH * Input.GetAxis("Mouse X");
                this.pitch -= this.lookSpeedV * Input.GetAxis("Mouse Y");

                this.transform.eulerAngles = new Vector3(this.pitch, this.yaw, 0f);
            }
            */
 
            //drag camera around with Middle Mouse
            if (Input.GetMouseButton(2))
            {
                transform.Translate(-Input.GetAxisRaw("Mouse X") * Time.deltaTime * dragSpeed, -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * dragSpeed, 0);
            }


            if (Input.GetMouseButton(1))
            {
                //Zoom in and out with Right Mouse
                if (currentCamera.orthographic)
                {
                    currentCamera.orthographicSize -= (Input.GetAxisRaw("Mouse X") * this.zoomSpeed);
                }
                else
                {
                    this.transform.Translate(0, 0, Input.GetAxisRaw("Mouse X") * this.zoomSpeed * .07f, Space.Self);
                }
            }

            //Zoom in and out with Mouse Wheel
            if (currentCamera.orthographic)
            {
                currentCamera.orthographicSize -= (Input.GetAxis("Mouse ScrollWheel") * this.zoomSpeed);
            }
            else
            {
                this.transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * this.zoomSpeed, Space.Self);
            }

            if (currentCamera.orthographicSize < 1)
            {
                currentCamera.orthographicSize = 1;
            }
        }
    }
}