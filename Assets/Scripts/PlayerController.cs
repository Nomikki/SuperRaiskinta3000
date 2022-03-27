using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PlayerController : MonoBehaviour
{
  public bool alive = true;
  public float hpPool = 100;
  public float shockTime = 0.0f;
  public float movingSpeed = 1.5f;
  public float jumpForce = 1.5f;
  public float activationDistance = 0.5f;
  public float mouseSensitivity = 1.0f;
  Camera cam;
  Rigidbody body;
  public GameObject flashlight;
  public GameObject weapon;
  public float lightTurningSpeed = 20.0f;
  public float lightMovingSpeed = 15.0f;

  public bool isGrounded = false;

  const float firerate = 1.0f / 4.0f;
  public float firetimer = 0.0f;

  public GameObject bulletDecal;

  public float recoilTime = 0.0f;
  public Vector2 recoildDirection;
  public float gunHeat = 0.0f;

  public Vector3 camStartPosition;
  public Vector3 weaponStartPosition;

  public TextMeshProUGUI hpText;
  public GameObject hpTextController;

  public AudioSource[] weaponAudioSource;
  int currentAudioSource = 0;


  /*
  Vector2[] recoilPattern = new Vector2[7] {
    new Vector2(0.1f, 0.2f),
    new Vector2(0, 0.3f),
    new Vector2(0, 0),
    new Vector2(0, 0),
    new Vector2(0, 0),
    new Vector2(0, 0),
    new Vector2(0, 0),
  };
  */

  Vector2 lookAt;

  // Start is called before the first frame update
  void Start()
  {
    body = GetComponent<Rigidbody>();
    cam = Camera.main;
    camStartPosition = cam.transform.localPosition;
    weaponStartPosition = weapon.transform.localPosition;
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

    if (alive)
    {
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
    }

    if (shockTime <= 0.0f)
    {
      body.velocity = velocity;
    }
  }



  void HandleRotations()
  {
    if (alive == false)
      return;

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
    
  }

  void FixedUpdate() {
    CheckIfTouchingGround();
    HandleMovement();
    HandleRotations();
    HandleFlashlight();
    if (alive)
    {
      HandleUse();
      HandleShooting();
      HandleRecoil();
    }
    hpText.text = "HP " + hpPool.ToString();

    shockTime -= Time.deltaTime;
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

  void HandleRecoil()
  {
    if (recoilTime > 0.0f)
    {
      float sprayArea = 0.25f;
      recoilTime -= Time.deltaTime;

      //lookAt.x += (recoildDirection.x*sprayArea) * Time.deltaTime;
      recoilTime = Mathf.Clamp(recoilTime, 0, 0.25f);

      lookAt.y = Mathf.Lerp(lookAt.y, (lookAt.y - (recoildDirection.y * sprayArea)), recoilTime * 4);
      lookAt.x = Mathf.Lerp(lookAt.x, (lookAt.x - (recoildDirection.x * sprayArea)), recoilTime * 4);

      cam.transform.localPosition = camStartPosition + new Vector3(0, 0, Mathf.Lerp(recoilTime * 0.2f, 0, recoilTime));
      weapon.transform.localPosition = weaponStartPosition + new Vector3(0, 0, Mathf.Lerp(recoilTime, 0, recoilTime * 4));
    }

    if (recoilTime <= 0.0f)
    {
      if (gunHeat > 0)
        gunHeat -= Time.deltaTime * 1.5f;
      else
        gunHeat = 0;

    }


  }

  void HandleShooting()
  {
    RaycastHit hitInfo;
    firetimer -= Time.deltaTime;

    if (Input.GetAxis("Fire1") > 0)
    {
      if (firetimer <= 0.0f)
      {
        firetimer = firerate;


        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 100.0f))
        {

          weaponAudioSource[currentAudioSource++].Play();
          currentAudioSource %= weaponAudioSource.Length;
          //hitscan
          //Debug.Log("Hit " + hitInfo.collider.name);
          GameObject gob = Instantiate(bulletDecal, hitInfo.point + hitInfo.normal * 0.00001f, Quaternion.LookRotation(hitInfo.normal));
          bulletDecal.transform.up = hitInfo.normal;
          gob.transform.SetParent(hitInfo.collider.transform);
          recoilTime = 0.25f;
          if (gunHeat < 0)
            gunHeat = 1.0f;
          else
            gunHeat += 0.2f;

          if (gunHeat > 2)
            gunHeat = 2;

          recoildDirection.x = Random.Range(-1f, 1f) * gunHeat;
          recoildDirection.y = Random.Range(-1.1f, 1.3f) * gunHeat;

          if (hitInfo.collider.tag == "Enemy")
          {
            float damageSubr = Mathf.Max(gunHeat - 1, 0) * 3.0f;
            float damage = Mathf.Max(3.0f - damageSubr, 0.2f);
            //Debug.Log("Make damage: " + damage);
            hitInfo.collider.GetComponent<EnemyController>().takeDamage(damage, hitInfo.point);
          }
        }
      }
    }

  
  }


  public void SetPosition(Vector3 pos)
  {
    if (!body)
      body = GetComponent<Rigidbody>();

    body.position = pos;
    body.velocity = Vector3.zero;
  }

  public Vector3 GetPosition()
  {
    return body.position;
  }

  public void TakeDamage(float amount, Vector3 direction)
  {
    hpPool -= amount;
    //body.velocity += direction * amount;
    body.AddForce(direction * amount * 0.5f, ForceMode.Impulse);
    shockTime = 0.33f;
    if (hpPool <= 0)
    {
      hpPool = 0;
      alive = false;
    }
  }

  public void createHitPointText(Vector3 pos, Vector3 direction, string text)
  {
    GameObject gob = Instantiate(hpTextController, pos, Quaternion.identity);
    gob.GetComponent<hitpointTextController>().setText(text, GetPosition());
  }
}


