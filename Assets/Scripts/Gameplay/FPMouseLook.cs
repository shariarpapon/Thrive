using UnityEngine;

public class FPMouseLook : MonoBehaviour
{
    public static bool RestrictRotation = false;

    public float mouseSensitivity = 100.0f;

    private Transform playerBody;
    private float xRotation = 0.0f;

    private void Awake()
    {
        playerBody = FindObjectOfType<FPController>().transform;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (RestrictRotation == false)
        {
            RotationLogic();
        }
    }

    void RotationLogic()
    {
        float x = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float y = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);
        playerBody.Rotate(Vector3.up * x);
    }

}
