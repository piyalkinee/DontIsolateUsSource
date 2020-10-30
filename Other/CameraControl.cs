using UnityEngine;

public class CameraControl : MonoBehaviour
{
    #region Fields
    //Scripts
    private World World;
    //Preference
    private float CameraSpeed = 0.08f;
    private int Frame = 3;
    //Permissions
    private bool UseCamera = false;
    private bool PlayerControlsCamera = false;
    //Tech
    private int ScreenHeight;
    private int ScreenWidth;
    #endregion
    #region Methods
    public void StartCamera()
    {
        World = GameObject.Find("-World-").GetComponent<World>();

        ScreenHeight = Screen.height;
        ScreenWidth = Screen.width;

        transform.position = new Vector3(World.CurrentLevel.Cells.GetLength(0) / 2 - 8, transform.position.y, World.CurrentLevel.Cells.GetLength(1) / 2 - 8);

        UseCamera = true;
        PlayerControlsCamera = true;
    }
    private void Update()
    {
        if (UseCamera)
        {
            Vector3 cameraTransform = transform.position;

            if (Input.mousePosition.x <= Frame)
            {
                cameraTransform.x -= CameraSpeed;
                cameraTransform.z += CameraSpeed;
            }
            else if (Input.mousePosition.x >= ScreenWidth - Frame)
            {
                cameraTransform.x += CameraSpeed;
                cameraTransform.z -= CameraSpeed;
            }
            else if (Input.mousePosition.y <= Frame)
            {
                cameraTransform.x -= CameraSpeed;
                cameraTransform.z -= CameraSpeed;
            }
            else if (Input.mousePosition.y >= ScreenHeight - Frame)
            {
                cameraTransform.x += CameraSpeed;
                cameraTransform.z += CameraSpeed;
            }

            if (PlayerControlsCamera)
                transform.position = new Vector3(Mathf.Clamp(cameraTransform.x, -2, World.CurrentLevel.Cells.GetLength(0) - 8), cameraTransform.y, Mathf.Clamp(cameraTransform.z, -2, World.CurrentLevel.Cells.GetLength(1) - 8));
        }
    }
    #endregion
}
