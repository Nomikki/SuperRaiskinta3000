using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
  public Image img;
  public WorldGenerator worldGenerator;
  const float refreshTime = 1.0f / 4.0f; //4 times per second
  PlayerController player;

  float timer = 0;
  Texture2D texture;
  void Start()
  {
    img.gameObject.SetActive(false);
    player = GameObject.Find("Player").GetComponent<PlayerController>();
    texture = new Texture2D(VoxelData.dungeonSize, VoxelData.dungeonSize);
    texture.filterMode = FilterMode.Point;
    GetComponent<Renderer>().material.mainTexture = texture;
  }

  public void GenerateMinimap()
  {
    Color color = Color.black;

    for (int y = 0; y < texture.height; y++)
    {
      for (int x = 0; x < texture.width; x++)
      {
        color = Color.black;
        color.a = 0;

        if (worldGenerator.CanWalk(x, y))
        {
          color = Color.white;
          color.a = 0.5f;
        }

        texture.SetPixel(x, y, color);
      }
    }

    color = Color.red;
    color.a = 1.0f;
    int px = (int)Mathf.Clamp(player.transform.position.x, 0, VoxelData.dungeonSize);
    int py = (int)Mathf.Clamp(player.transform.position.z, 0, VoxelData.dungeonSize);

    texture.SetPixel(px, py, color);

    texture.Apply();
    img.material.mainTexture = texture;

  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.M))
    {
      if (img.gameObject.activeInHierarchy)
        img.gameObject.SetActive(false);
      else
        img.gameObject.SetActive(true);
    }

    if (img.gameObject.activeInHierarchy)
    {
      timer += Time.deltaTime;
      if (timer > refreshTime)
      {
        timer = 0;
        GenerateMinimap();
      }
    }

  }
}
