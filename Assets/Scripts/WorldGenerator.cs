using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
  public const int mapx = 80 * 2;
  public const int mapy = 80 * 2;

  List<MapTile> tiles = new List<MapTile>();
  /*
  public GameObject[] walls;
  public GameObject[] floors;
  public GameObject[] lamps;
  */
  public GameObject playerObject;
  const int MAX_LEAF_SIZE = 20 * 3;
  List<Leaf> _leafs = new List<Leaf>();
  Leaf root;

  // Use this for initialization
  void Start()
  {
    initAll();
    generateBSP();
    building();
  }

  void initAll()
  {
    for (int y = 0; y < mapy; y++)
    {
      for (int x = 0; x < mapx; x++)
      {
        tiles.Add(new MapTile());
      }
    }

    for (int y = 0; y < mapy; y++)
    {
      for (int x = 0; x < mapx; x++)
      {
        tiles[x + y * mapx].canWalk = false;
      }
    }
  }

  void dig(int x1, int y1, int x2, int y2, bool roomDigging)
  {
    //swapataan
    if (x2 < x1)
    {
      int tmp = x2;
      x2 = x1;
      x1 = tmp;
    }

    if (y2 < y1)
    {
      int tmp = y2;
      y2 = y1;
      y1 = tmp;
    }

    //bool lastWasWalkable = false;
    for (int tilex = x1; tilex <= x2; tilex++)
    {
      for (int tiley = y1; tiley <= y2; tiley++)
      {
        if (tilex > 0 && tiley > 0 && tilex < mapx && tiley < mapy)
        {
          tiles[tilex + tiley * mapx].canWalk = true;
          tiles[tilex + tiley * mapx].createMe = true;

          tiles[(tilex - 1) + (tiley) * mapx].createMe = true;
          tiles[(tilex + 1) + (tiley) * mapx].createMe = true;
          tiles[(tilex) + (tiley - 1) * mapx].createMe = true;
          tiles[(tilex) + (tiley + 1) * mapx].createMe = true;

          tiles[(tilex - 1) + (tiley - 1) * mapx].createMe = true;
          tiles[(tilex + 1) + (tiley - 1) * mapx].createMe = true;
          tiles[(tilex - 1) + (tiley + 1) * mapx].createMe = true;
          tiles[(tilex + 1) + (tiley + 1) * mapx].createMe = true;
        }
      }
    }

  }


  void createRoom(bool first, int x1, int y1, int x2, int y2)
  {
    dig(x1, y1, x2, y2, true);

    if (first)
    {
      Vector3 p = new Vector3((x1 + x2) / 2, 0, (y1 + y2) / 2);
      //playerObject.transform.position = p;
    }
  }



  void generateBSP()
  {

    int _sprMapX = mapx - 10;
    int _sprMapY = mapy - 10;

    root = new Leaf(10, 10, _sprMapX, _sprMapY);

    _leafs.Clear();
    _leafs.Add(root);

    bool did_split = true;
    // we loop through every Leaf in our Vector over and over again, until no more Leafs can be split.
    while (did_split)
    {
      did_split = false;

      for (int i = 0; i < _leafs.Count; i++)
      {
        Leaf l = _leafs[i];

        if (l.leftChild == null && l.rightChild == null) // if this Leaf is not already split...
        {
          // if this Leaf is too big, or 75% chance...
          if (l.width > MAX_LEAF_SIZE || l.height > MAX_LEAF_SIZE || Random.Range(0, 100) > 25)
          {
            if (l.split()) // split the Leaf!
            {
              // if we did split, push the child leafs to the Vector so we can loop into them next
              _leafs.Add(l.leftChild);
              _leafs.Add(l.rightChild);
              did_split = true;
            }
          }
        }
      }
    }
    
    _leafs[0].createRooms();

    //-------------
    bool firstRoom = true;
    for (int i = 0; i < _leafs.Count; i++)
    {
      Leaf l = _leafs[i];

      if (l.leftChild == null || l.rightChild == null)
      {
        createRoom(firstRoom, l.room.x + 1, l.room.y + 1, l.room.x + l.room.w - 1, l.room.y + l.room.h - 1);
        firstRoom = false;
      }
    }


    for (int i = 0; i < _leafs.Count; i++)
    {
      Leaf l = _leafs[i];
      //vielä käytävät
      for (int k = 0; k < l.halls.Count; k++)
        dig(l.halls[k].x, l.halls[k].y, l.halls[k].x + l.halls[k].w, l.halls[k].y + l.halls[k].h, false);
    }


  }

  void building()
  {
    for (int y = 0; y < mapy; y++)
    {
      for (int x = 0; x < mapx; x++)
      {

        if (tiles[x + y * mapx].createMe == true)
        {

          if (tiles[x + y * mapx].canWalk == false)
          {
            //Instantiate(walls[0], new Vector3(x, 0.5f, y), Quaternion.identity);
          }
          else
          {
            //Instantiate(floors[floorID], new Vector3(x, -0.5f, y), Quaternion.identity);
          }
        }
      }

    }
  }

}
