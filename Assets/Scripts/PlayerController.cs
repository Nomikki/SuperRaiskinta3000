using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

  public float movingSpeed = 1.5f;
  public float mouseSensitivity = 1.0f;
  Camera cam;
  Rigidbody body;

  Vector2 lookAt;

  // Start is called before the first frame update
  void Start()
  {
    body = GetComponent<Rigidbody>();
    cam = Camera.main;
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  void HandleMovement()
  {
    float horizontal = Input.GetAxis("Horizontal");
    float vertical = Input.GetAxis("Vertical");
    Vector3 velocity = body.velocity;
    float oldY = velocity.y;
    velocity.y = 0;
    //velocity += (transform.forward * vertical + transform.right * horizontal);
    velocity = transform.right * horizontal + transform.forward * vertical;
    velocity = Vector3.ClampMagnitude(velocity, 1);
    velocity *= movingSpeed;
    velocity.y = oldY;
    body.velocity = velocity;
  }

  void HandleRotations()
  {
    float mouseX = Input.GetAxis("Mouse X");
    float mouseY = -Input.GetAxis("Mouse Y");

    lookAt.x += mouseX * mouseSensitivity;
    lookAt.y += mouseY * mouseSensitivity;

    lookAt.x = Mathf.Repeat(lookAt.x, 360);
    lookAt.y = Mathf.Clamp(lookAt.y,  -80, 80);

    transform.rotation = Quaternion.Euler(0, lookAt.x, 0);

    cam.transform.rotation = Quaternion.Euler(lookAt.y, lookAt.x, 0);
  }

  // Update is called once per frame
  void Update()
  {
    HandleMovement();
    HandleRotations();

  }



  public void SetPosition(Vector3 pos)
  {
    if (!body)
      body = GetComponent<Rigidbody>();

    body.position = pos;
    body.velocity = Vector3.zero;
  }
}

