using UnityEngine;
using UnityEngine.XR;

//Written by Abdul Galeel Ali

public class CameraController : MonoBehaviour
{
    public Camera MainCamera;

    public float MovementSpeed;
    public float ZoomSpeed;
    public float RotationSpeed;

    public float MaxY;
    public float MinY;

    Vector3 OldMousePos;

    private void Start()
    {
        transform.rotation = Quaternion.Euler(60, 0, 0);
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            if (transform.position.y > MinY)
            {
                transform.position = Vector3.Slerp(transform.position, transform.position + (transform.forward * (transform.position.y / 10)), MovementSpeed * 10 * Time.deltaTime);
            }
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            if (transform.position.y < MaxY)
            {
                transform.position = Vector3.Slerp(transform.position, transform.position - (transform.forward * (transform.position.y / 10)), MovementSpeed * 10 * Time.deltaTime);
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position = Vector3.Slerp(transform.position, transform.position + (transform.right * (transform.position.y / 10)), MovementSpeed * Time.deltaTime * (transform.position.y / 15));
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position = Vector3.Slerp(transform.position, transform.position - (transform.right * (transform.position.y / 10)), MovementSpeed * Time.deltaTime * (transform.position.y / 15));
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.position = Vector3.Slerp(transform.position, new Vector3(transform.position.x + (transform.forward.x * transform.position.y / 10), transform.position.y, transform.position.z + (transform.forward.z * transform.position.y / 10)), MovementSpeed * Time.deltaTime * (transform.position.y / 15));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position = Vector3.Slerp(transform.position, new Vector3(transform.position.x - (transform.forward.x * transform.position.y / 10), transform.position.y, transform.position.z - (transform.forward.z * (transform.position.y / 10))), MovementSpeed * Time.deltaTime * (transform.position.y / 15));
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 MousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);

            if (Input.GetMouseButtonDown(2))
            {
                OldMousePos = MousePosition;
            }

            Vector3 NewPos = transform.position;
            if (MousePosition.x != OldMousePos.x)
            {
                NewPos += Mathf.Sign(OldMousePos.x - MousePosition.x) * transform.right * MovementSpeed * (transform.position.y / 20);
            }

            if (MousePosition.y != OldMousePos.y)
            {
                NewPos += Mathf.Sign(OldMousePos.y - MousePosition.y) * transform.forward * MovementSpeed * (transform.position.y / 20);
            }
            NewPos.y = transform.position.y;

            transform.position = Vector3.Lerp(transform.position, NewPos, MovementSpeed * Time.deltaTime);
            OldMousePos = Input.mousePosition;
        }

        if (Input.GetKey(KeyCode.Z))
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
            {
                transform.RotateAround(hit.point, Vector3.up, Time.deltaTime * 20);
            }
        }
        else if (Input.GetKey(KeyCode.C))
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
            {
                transform.RotateAround(hit.point, -Vector3.up, Time.deltaTime * 20);
            }
        }
    }
}