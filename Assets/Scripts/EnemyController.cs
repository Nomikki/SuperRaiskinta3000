using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
  public float hpPool = 100;
  public float movingSpeed = 1.0f;
  public float turningSpeed = 2.0f;
  public ParticleSystem bloodParticles;
  public ParticleSystem BigBloodParticles;
  float reactionTimer = 0.0f;



  bool moving = false;

  Vector3 targetPosition;
  Rigidbody body;

  float targetRotation = 0;
  float currentRotation = 0;

  PlayerController player;


  // Start is called before the first frame update
  void Start()
  {
    body = GetComponent<Rigidbody>();
    targetPosition = body.position;
    player = GameObject.Find("Player").GetComponent<PlayerController>();
  }

  void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawSphere(targetPosition, 0.5f);
  }

  void HandleMovement()
  {
    Vector3 velo = body.velocity;
    if (moving == true)
    {
      velo = transform.forward * 1;
      velo *= Vector3.SqrMagnitude(velo);
      velo *= movingSpeed;
    }
    else
    {
      velo *= 0;
    }

    currentRotation = Mathf.LerpAngle(currentRotation, targetRotation, turningSpeed * Time.deltaTime);
    body.rotation = Quaternion.Euler(0, currentRotation, 0);
    body.velocity = velo;
  }

  public void takeDamage(float dmg, Vector3 hitpoint)
  {
    hpPool -= dmg;
    //bloodParticles.transform.position = hitpoint;
    /*
    bloodParticles.Stop();
    bloodParticles.Play();
    */
    GameObject gob = Instantiate(bloodParticles.gameObject, hitpoint, Quaternion.identity);
    gob.transform.parent = null;


    if (hpPool < 0)
    {
      gob = Instantiate(BigBloodParticles.gameObject, hitpoint, Quaternion.identity);
      gob.transform.parent = null;
      Object.Destroy(gameObject);
    }
  }

  void Idle()
  {
    if (reactionTimer < 0)
    {

      /*
      targetPosition.x = body.position.x + Random.Range(-5, 5);
      targetPosition.z = body.position.z + Random.Range(-5, 5);
      */
      targetPosition = player.GetPosition();
      if (canSeePlayer())
      {
        moving = true;
        reactionTimer = 0.5f;
      }
      else
      {

        if (Random.Range(0, 10) > 3)
        {
          moving = true;
        }
        else
        {
          moving = false;
        }
      }
      /*
      Transform tr = transform;
      tr.LookAt(targetPosition);
      body.rotation = tr.rotation;
      */
      targetRotation = Mathf.Atan2(targetPosition.x - body.position.x, targetPosition.z - body.position.z) / 3.1415f * 180.0f;


      reactionTimer = 1.0f;
    }
  }

  // Update is called once per frame
  void Update()
  {
    HandleMovement();
    Idle();
    reactionTimer -= Time.deltaTime;
  }

  bool canSeePlayer()
  {
    Vector3 direction = body.position - player.GetPosition();
    direction.Normalize();

    RaycastHit hitInfo;
    if (Physics.Raycast(body.position, direction, out hitInfo, 50))
    {
      if (hitInfo.collider.name == "Player")
      {
        Debug.DrawRay(body.position, direction, Color.green, 3.0f);
        return true;
      }
    }

    return false;
  }

  bool isInTargetPosition()
  {
    if (Vector3.Distance(body.position, targetPosition) < 1.0f)
    {
      return true;
    }
    return false;
  }
}