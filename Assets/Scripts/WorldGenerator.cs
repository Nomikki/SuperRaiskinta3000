using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{

  public GameObject chunkObject;
  List<MapTile> tiles = new List<MapTile>();
  public GameObject playerObject;
  public GameObject door;
  Vector3 startPos;

  public Material material;
  public GameObject roomLight;

  List<Vector2> templateDoors = new List<Vector2>();

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
    for (int y = 0; y < VoxelData.dungeonSize; y++)
    {
      for (int x = 0; x < VoxelData.dungeonSize; x++)
      {
        tiles.Add(new MapTile());
      }
    }

    for (int y = 0; y < VoxelData.dungeonSize; y++)
    {
      for (int x = 0; x < VoxelData.dungeonSize; x++)
      {
        tiles[x + y * VoxelData.dungeonSize].canWalk = false;
      }
    }
  }

  void dig(int x1, int y1, int x2, int y2)
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

    bool lastWasWalkable = false;
    for (int tilex = x1; tilex <= x2; tilex++)
    {
      for (int tiley = y1; tiley <= y2; tiley++)
      {
        if (tilex > 0 && tiley > 0 && tilex < VoxelData.dungeonSize && tiley < VoxelData.dungeonSize)
        {
          int index = tilex + tiley * VoxelData.dungeonSize;

          if (tiles[index].canWalk == false && lastWasWalkable == true &&
          (x1 == x2 || y1 == y2)
        )
          {
            this.templateDoors.Add(new Vector2(tilex, tiley));
            Debug.Log("door added to " + tilex + ", " + tiley);
          }

          lastWasWalkable = tiles[tilex + tiley * VoxelData.dungeonSize].canWalk;

          tiles[tilex + tiley * VoxelData.dungeonSize].canWalk = true;
          tiles[tilex + tiley * VoxelData.dungeonSize].createMe = true;

          tiles[(tilex - 1) + (tiley) * VoxelData.dungeonSize].createMe = true;
          tiles[(tilex + 1) + (tiley) * VoxelData.dungeonSize].createMe = true;
          tiles[(tilex) + (tiley - 1) * VoxelData.dungeonSize].createMe = true;
          tiles[(tilex) + (tiley + 1) * VoxelData.dungeonSize].createMe = true;

          tiles[(tilex - 1) + (tiley - 1) * VoxelData.dungeonSize].createMe = true;
          tiles[(tilex + 1) + (tiley - 1) * VoxelData.dungeonSize].createMe = true;
          tiles[(tilex - 1) + (tiley + 1) * VoxelData.dungeonSize].createMe = true;
          tiles[(tilex + 1) + (tiley + 1) * VoxelData.dungeonSize].createMe = true;
        }
      }
    }

  }


  void createRoom(bool first, int x1, int y1, int x2, int y2)
  {
    dig(x1, y1, x2, y2);

    if (first)
    {
      startPos = new Vector3((x1 + x2) / 2, 0, (y1 + y2) / 2);
      playerObject.GetComponent<PlayerController>().SetPosition(startPos);
    }

    Vector3 lampPos = new Vector3((x1 + x2) / 2, 0.75f, (y1 + y2) / 2);
    Instantiate(roomLight, lampPos, Quaternion.identity);

  }



  void generateBSP()
  {

    int _sprMapX = VoxelData.dungeonSize - 10;
    int _sprMapY = VoxelData.dungeonSize - 10;

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
        createRoom(firstRoom, l.room.x, l.room.y , l.room.x + l.room.w - 1, l.room.y + l.room.h - 1);
        firstRoom = false;
      }
    }


    for (int i = 0; i < _leafs.Count; i++)
    {
      Leaf l = _leafs[i];
      //vielä käytävät
      for (int k = 0; k < l.halls.Count; k++)
      {
        dig(l.halls[k].x, l.halls[k].y, l.halls[k].x + l.halls[k].w, l.halls[k].y + l.halls[k].h);
      }
    }


    for (int i = 0; i < templateDoors.Count; i++)
    {
      AddDoor(templateDoors[i]);
    }

  }

  void AddDoor(Vector2 p)
  {
    Instantiate(door, new Vector3(p.x, 0, p.y), Quaternion.identity);
  }

  public bool CanWalk(int x, int y)
  {
    if (x >= 0 && y >= 0 && x < VoxelData.dungeonSize && y < VoxelData.dungeonSize)
      return tiles[x + y * VoxelData.dungeonSize].canWalk;

    return false;
  }

  public bool CreateMe(int x, int y)
  {
    if (x >= 0 && y >= 0 && x < VoxelData.dungeonSize && y < VoxelData.dungeonSize)
      return tiles[x + y * VoxelData.dungeonSize].createMe;

    return false;
  }

  void building()
  {
    //create some chunks
    int numOfChunks = (int)Mathf.Ceil((float)(VoxelData.dungeonSize / (float)VoxelData.chunkWidth));


    for (int z = 0; z < numOfChunks; z++)
    {
      for (int x = 0; x < numOfChunks; x++)
      {
        GameObject gob = Instantiate(chunkObject, Vector3.zero, Quaternion.identity);
        gob.name = "chunk " + x + ", " + z;
        Chunk chunk = gob.GetComponent<Chunk>();
        chunk.worldGenerator = this;
        chunk.Populate(new Vector2(x * VoxelData.chunkWidth, z * VoxelData.chunkWidth));
      }
    }
  }

}


