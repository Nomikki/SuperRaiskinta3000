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

  List<byte> minimapData;

  float timer = 0;
  Texture2D texture;
  void Start()
  {
    img.gameObject.SetActive(false);
    player = GameObject.Find("Player").GetComponent<PlayerController>();
    texture = new Texture2D(VoxelData.dungeonSize, VoxelData.dungeonSize);
    texture.filterMode = FilterMode.Point;
    GetComponent<Renderer>().material.mainTexture = texture;

    ClearMinimap();
  }


  void calcLos()
  {

    for (int i = 0; i < VoxelData.dungeonSize * VoxelData.dungeonSize; i++)
    {
      if (minimapData[i] == 1)
        minimapData[i] = 2;
    }

    for (int a = 0; a < 360; a++)
    {
      float px = player.GetPosition().x;
      float py = player.GetPosition().z;
      float dx = Mathf.Sin((float)a / 180.0f * 3.141f);
      float dy = Mathf.Cos((float)a / 180.0f * 3.141f);


      for (int l = 0; l < 10; l++)
      {
        px += dx;
        py += dy;
        int tx = (int)px;
        int ty = (int)py;
        if (tx > 0 && ty > 0 && tx < VoxelData.dungeonSize && ty < VoxelData.dungeonSize)
        {
          if (worldGenerator.CanWalk(tx, ty) == true)
          {
            int index = tx + ty * VoxelData.dungeonSize;
            if (index > 0 && index < minimapData.Count)
            {
              minimapData[index] = 1;
            }
          }
          else
          {
            break;
          }
        }
      }
    }
  }

  public void ClearMinimap()
  {
    minimapData = new List<byte>();
    minimapData.Clear();
    for (int i = 0; i < VoxelData.dungeonSize * VoxelData.dungeonSize; i++)
      minimapData.Add(0);
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

        int index = x + y * VoxelData.dungeonSize;
        if (worldGenerator.CanWalk(x, y) && minimapData[index] != 0)
        {
          if (minimapData[index] == 1)
            color = Color.white;
          else
            color = Color.gray;
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


    timer += Time.deltaTime;
    if (timer > refreshTime)
    {
      timer = 0;
      calcLos();
      if (img.gameObject.activeInHierarchy)
      {
        GenerateMinimap();
      }
    }


  }
}
