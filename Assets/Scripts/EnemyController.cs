using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AiState
{
  idle,
  following,
  shooting,
  melee,
  die
};

public class EnemyController : MonoBehaviour
{
  public float hpPool = 100;
  public float movingSpeed = 1.0f;
  public float accelerationSpeed = 1.0f;
  public float currentMovementSpeed = 0.0f;
  public float turningSpeed = 2.0f;
  public bool moving = false;
  public float meleeDamage = 2;

  public AiState state;

  public GameObject ammo;
  public ParticleSystem bloodParticles;
  public ParticleSystem BigBloodParticles;
  /*
  public float ammoPerSecond = 2;
  float ammoTimer = 0;
  
  */


  float reactionTimer = 0.0f;


  Vector3 targetPosition;
  Rigidbody body;

  float targetRotation = 0;
  float currentRotation = 0;

  PlayerController player;

  float shockingTimer = 0;


  // Start is called before the first frame update
  void Start()
  {
    body = GetComponent<Rigidbody>();
    targetPosition = body.position;
    player = GameObject.Find("Player").GetComponent<PlayerController>();
    state = AiState.idle;
  }

  void OnDrawGizmosSelected()
  {

    Gizmos.color = Color.red;
    Gizmos.DrawSphere(targetPosition, 0.5f);

  }

  void HandleMovement()
  {

    Vector3 velo = body.velocity;
    float oldY = velo.y;
    if (moving == true)
    {
      currentMovementSpeed += accelerationSpeed * Time.deltaTime;
    }
    else
    {
      currentMovementSpeed -= accelerationSpeed * Time.deltaTime;
    }

    currentMovementSpeed = Mathf.Clamp(currentMovementSpeed, 0, movingSpeed);

    velo = transform.forward * 1;
    velo *= Vector3.SqrMagnitude(velo);
    velo *= currentMovementSpeed;

    currentRotation = Mathf.LerpAngle(currentRotation, targetRotation, turningSpeed * Time.deltaTime);
    body.rotation = Quaternion.Euler(0, currentRotation, 0);
    velo.y = oldY;

    if (shockingTimer <= 0.0f)
    {
      body.velocity = velo;
    }

  }

  public void takeDamage(float dmg, Vector3 hitpoint)
  {

    hpPool -= dmg;
    GameObject gob = Instantiate(bloodParticles.gameObject, hitpoint, Quaternion.identity);
    gob.transform.parent = null;
    if (hpPool < 0)
    {
      die();
    }
    Vector3 direction = body.position - hitpoint;
    direction.Normalize();

    body.AddForce(direction, ForceMode.Impulse);
    shockingTimer = 0.5f;
    moving = false;
    reactionTimer = 0.5f;

    state = AiState.following;
    following();
  }

  bool canSeePlayer()
  {
    RaycastHit hitInfo;
    Vector3 direction = player.GetPosition() - body.position;
    direction.Normalize();
    float maxDistance = 40;

    if (Physics.Raycast(body.position, direction, out hitInfo, maxDistance))
    {
      if (hitInfo.collider.tag == "Player")
      {
        return true;
      }
    }


    return false;
  }

  void HandleDistances()
  {
    float distance = Vector3.Distance(body.position, player.GetPosition());

    if (distance <= 1.0f)
    {
      state = AiState.melee;
    }
    else
    {
      if (Random.Range(0, 100) > 30)
      {
        state = AiState.shooting;
      }
      else
      {
        state = AiState.following;
      }
    }
  }

  void Idle()
  {
    //Debug.Log("idle");
    reactionTimer = 1.0f;
    moving = false;

    //if can see player, or hear it, decice what to do
    //if its near, try melee, shooting or following

    if (canSeePlayer() == true)
    {
      HandleDistances();
    }
  }

  void shoot()
  {
    //Debug.Log("shoot");

    GameObject gob = Instantiate(ammo, body.position + transform.forward, Quaternion.identity);
    gob.GetComponentInChildren<missile>().shoot(transform.forward, gameObject);
    reactionTimer = 0.5f;
    state = AiState.idle;
    Idle();

  }

  void following()
  {
    //Debug.Log("following");
    reactionTimer = 1.0f;

    if (canSeePlayer() == false)
    {
      state = AiState.idle;
    }
    else
    {
      targetPosition = player.GetPosition();
      targetRotation = Mathf.Atan2(targetPosition.x - body.position.x, targetPosition.z - body.position.z) / 3.1415f * 180.0f;
      moving = true;
      HandleDistances();
    }

  }

  void melee()
  {
    //Debug.Log("melee");
    reactionTimer = 2.0f;
    state = AiState.idle;
    moving = false;

    float distance = Vector3.Distance(body.position, player.GetPosition());
    if (distance <= 1.0f)
    {
      player.TakeDamage(meleeDamage, transform.forward);
    }
  }

  void die()
  {
    //Debug.Log("die");
    hpPool = 0;
    GameObject gob = Instantiate(BigBloodParticles.gameObject, transform.position, Quaternion.identity);
    gob.transform.parent = null;
    Object.Destroy(gameObject);
  }

  // Update is called once per frame
  void Update()
  {
    reactionTimer -= Time.deltaTime;
    shockingTimer -= Time.deltaTime;

    if (reactionTimer <= 0)
    {
      switch (state)
      {
        case AiState.following:
          following();
          break;
        case AiState.die:
          die();
          break;
        case AiState.melee:
          melee();
          break;
        case AiState.shooting:
          shoot();
          break;
        case AiState.idle:
        default:
          Idle();
          break;
      }
    }
  }

  void FixedUpdate()
  {
    HandleMovement();
  }
}
