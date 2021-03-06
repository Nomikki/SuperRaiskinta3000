using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missile : MonoBehaviour
{
  public float movementSpeed;
  public float damage;
  public Vector3 direction;
  GameObject shooter;
  Rigidbody body;

  // Start is called before the first frame update
  void Start()
  {
    getBody();
  }

  // Update is called once per frame
  void Update()
  {
    RaycastHit hitInfo;
    if (Physics.Raycast(body.position, transform.forward, out hitInfo, 0.25f))
    {
      if (hitInfo.collider.tag == "Player")
      {
        //Debug.Log("Player takes damage!");
        hitInfo.collider.GetComponent<PlayerController>().TakeDamage(damage, transform.forward);
      }
      else if (hitInfo.collider.tag == "Enemy")
      {
        if (hitInfo.collider.gameObject != shooter)
        {
          hitInfo.collider.GetComponent<EnemyController>().takeDamage(damage, hitInfo.point);
        }
      }
      else
      {
        //?
      }

      Object.Destroy(gameObject);
    }
  }

  void getBody()
  {
    if (body == null)
      body = GetComponent<Rigidbody>();
  }

  public void shoot(Vector3 targetDirection, GameObject shooter)
  {
    transform.forward = targetDirection;
    getBody();
    body.velocity = transform.forward * movementSpeed;
  }


}
