using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

  public float movingSpeed = 1.5f;
  public float jumpForce = 1.5f;
  public float activationDistance = 0.5f;
  public float mouseSensitivity = 1.0f;
  Camera cam;
  Rigidbody body;
  public GameObject flashlight;
  public float lightTurningSpeed = 20.0f;
  public float lightMovingSpeed = 15.0f;

  public bool isGrounded = false;

  const float firerate = 1.0f / 4.0f;
  public float firetimer = 0.0f;

  public GameObject bulletDecal;


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
    float tryJump = Input.GetAxis("Jump");
    Vector3 velocity = body.velocity;
    float oldY = velocity.y;
    velocity.y = 0;
    //velocity += (transform.forward * vertical + transform.right * horizontal);
    velocity = transform.right * horizontal + transform.forward * vertical;
    velocity = Vector3.ClampMagnitude(velocity, 1);
    velocity *= movingSpeed;
    velocity.y = oldY;

    if (tryJump > 0)
    {
      if (isGrounded)
      {
        velocity.y = jumpForce;
        isGrounded = false;
      }
    }


    body.velocity = velocity;
  }

  public Vector3 GetPosition()
  {
    return transform.position;
  }

  void HandleRotations()
  {
    float mouseX = Input.GetAxis("Mouse X");
    float mouseY = -Input.GetAxis("Mouse Y");

    lookAt.x += mouseX * mouseSensitivity;
    lookAt.y += mouseY * mouseSensitivity;

    lookAt.x = Mathf.Repeat(lookAt.x, 360);
    lookAt.y = Mathf.Clamp(lookAt.y, -80, 80);

    transform.rotation = Quaternion.Euler(0, lookAt.x, 0);

    cam.transform.rotation = Quaternion.Euler(lookAt.y, lookAt.x, 0);
  }

  void HandleFlashlight()
  {

    Vector3 lpos = flashlight.transform.position;
    Vector3 lrot = flashlight.transform.rotation.eulerAngles;

    lpos += (body.position - lpos) * Time.deltaTime * lightMovingSpeed;
    lrot.x = Mathf.LerpAngle(lrot.x, lookAt.y, lightTurningSpeed * Time.deltaTime);
    lrot.y = Mathf.LerpAngle(lrot.y, lookAt.x, lightTurningSpeed * Time.deltaTime);

    flashlight.transform.SetPositionAndRotation(lpos, Quaternion.Euler(lrot));
  }

  void CheckIfTouchingGround()
  {
    isGrounded = false;
    RaycastHit hit;
    if (Physics.Raycast(body.position, Vector3.down, out hit, 0.3f))
    {
      isGrounded = true;
    }
  }

  // Update is called once per frame
  void Update()
  {
    CheckIfTouchingGround();
    HandleMovement();
    HandleRotations();
    HandleFlashlight();
    HandleUse();
    HandleShooting();
  }

  void HandleUse()
  {
    RaycastHit hitInfo;

    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, activationDistance))
    {

      if (Input.GetAxis("Use") > 0)
      {
        if (hitInfo.collider.tag == "door")
        {
          if (hitInfo.collider.transform.parent && hitInfo.collider.transform.parent.parent)
          {
            if (hitInfo.collider.transform.parent.parent.GetComponent<doorController>())
              hitInfo.collider.transform.parent.parent.GetComponent<doorController>().Open();
          }
        }
      }
    }

  }

  void HandleShooting()
  {
    RaycastHit hitInfo;

    if (Input.GetAxis("Fire1") > 0)
    {
      if (firetimer <= 0.0f)
      {
        firetimer = firerate;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 100.0f))
        {
          //Debug.Log("Hit " + hitInfo.collider.name);
          GameObject gob = Instantiate(bulletDecal, hitInfo.point + hitInfo.normal * 0.00001f, Quaternion.LookRotation(hitInfo.normal));
          bulletDecal.transform.up = hitInfo.normal;
          gob.transform.SetParent(hitInfo.collider.transform);
        }
      }
    }

    firetimer -= Time.deltaTime;
  }


  public void SetPosition(Vector3 pos)
  {
    if (!body)
      body = GetComponent<Rigidbody>();

    body.position = pos;
    body.velocity = Vector3.zero;
  }
}


