using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorController : MonoBehaviour
{
  Animator anim;
  PlayerController player;
  bool doorOpen = false;

  public AudioSource openDoor;
  public AudioSource closeDoor;

  // Start is called before the first frame update
  void Start()
  {
    anim = GetComponentInChildren<Animator>();
    player = GameObject.Find("Player").GetComponent<PlayerController>();
  }

  // Update is called once per frame
  void Update()
  {

    Vector3 playerPos = player.GetPosition();

    if (doorOpen == true)
    {
      if (Vector3.Distance(playerPos, transform.position) > 2.0f)
      {
        Close();
      }
    }


  }

  public void Open()
  {
    if (doorOpen == false)
      openDoor.Play();
    doorOpen = true;
    anim.SetBool("open", true);
  }

  public void Close()
  {
    if (doorOpen == true)
      closeDoor.Play();
    doorOpen = false;
    anim.SetBool("open", false);
  }
}
