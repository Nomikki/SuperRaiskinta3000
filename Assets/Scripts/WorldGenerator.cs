using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{

  public GameObject chunkObject;
  List<MapTile> tiles = new List<MapTile>();
  public GameObject playerObject;
  public GameObject door;
  public Minimap minimap;
  Vector3 startPos;

  public Material material;
  public GameObject roomLight;
  List<Vector2> templateDoors = new List<Vector2>();

  const int ROOM_MAX_SIZE = 5;
  const int ROOM_MIN_SIZE = 4;

  public BspGenerator root = null;
  // Use this for initialization
  void Start()
  {
    initAll();
    generateBSP();
    minimap.worldGenerator = this;
    minimap.GenerateMinimap();
    building();
    playerObject.GetComponent<PlayerController>().SetPosition(startPos);

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
    //Debug.Log("!");
    //Debug.LogFormat("dig: {0}, {1}, {2}, {3}", x1, y1, x2, y2);

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


  void createRoom(int x1, int y1, int x2, int y2)
  {
    dig(x1, y1, x2, y2);

    Vector3 lampPos = new Vector3((x1 + x2) / 2, 0.75f, (y1 + y2) / 2);
    Instantiate(roomLight, lampPos, Quaternion.identity);

  }

  void makeRoomWithCorridors(Rectangle temproom)
  {
    Rectangle room = temproom;
    if (room.w <= 0) room.w = 1;
    if (room.h <= 0) room.h = 1;
    if (room.x <= 0) room.x = 1;
    if (room.y <= 0) room.y = 1;

    createRoom(
      room.x,
      room.y,
      room.x + room.w - 2,
      room.y + room.h - 2
    );
  }

  void fillWithWalls()
  {
    for (int y = 0; y < VoxelData.dungeonSize; y++)
    {
      for (int x = 0; x < VoxelData.dungeonSize; x++)
      {
        int index = x + y * VoxelData.dungeonSize;
        this.tiles[index].canWalk = false;

      }
    }
  }

  void generateBSP()
  {
    //int levelSeed = 1;
    //Random.InitState(levelSeed);

    int maxSplitLevel = Random.Range(4, 8);
    List<Rectangle> corridors = new List<Rectangle>();

    tiles = new List<MapTile>();
    tiles.Clear();
    for (int y = 0; y < VoxelData.dungeonSize; y++)
      for (int x = 0; x < VoxelData.dungeonSize; x++)
        tiles.Add(new MapTile());


    root = new BspGenerator(0, 0, VoxelData.dungeonSize, VoxelData.dungeonSize, maxSplitLevel);

    int lastX = 0;
    int lastY = 0;

    fillWithWalls();
    //Debug.LogFormat("rooms: {0}", root.rooms.Count);

    int spawnRoomIndex = Random.Range(0, root.rooms.Count - 1);

    for (int i = 0; i < root.rooms.Count; i++)
    {
      Rectangle tempRoom = root.rooms[i];
      tempRoom.w = (int)Random.Range(ROOM_MIN_SIZE, tempRoom.w - 2);
      tempRoom.h = (int)Random.Range(ROOM_MIN_SIZE, tempRoom.h - 2);


      makeRoomWithCorridors(tempRoom);

      if (i > 0)
      {
        corridors.Add(new Rectangle(lastX, lastY, tempRoom.x + tempRoom.w / 2, lastY));
        corridors.Add(new Rectangle(tempRoom.x + tempRoom.w / 2, lastY, tempRoom.x + tempRoom.w / 2, tempRoom.y + tempRoom.h / 2));
      }
      lastX = tempRoom.x + tempRoom.w / 2;
      lastY = tempRoom.y + tempRoom.h / 2;

      if (i == spawnRoomIndex)
      {
        startPos.x = lastX;
        startPos.y = 0;
        startPos.z = lastY;
      }
    }

    for (int l = 0; l < corridors.Count; l++)
    {
      dig(corridors[l].x, corridors[l].y, corridors[l].w, corridors[l].h);
    }

    for (int i = 0; i < templateDoors.Count; i++)
    {
      AddDoor(templateDoors[i]);
    }
  }

  void AddDoor(Vector2 p)
  {

    //add door, if its between walls
    if (CanWalk((int)p.x - 1, (int)p.y) == false && CanWalk((int)p.x + 1, (int)p.y) == false)
    {
      Instantiate(door, new Vector3(p.x + 0.5f, 0, p.y + 0.5f), Quaternion.Euler(0, 0, 0));
    }
    if (CanWalk((int)p.x, (int)p.y - 1) == false && CanWalk((int)p.x, (int)p.y + 1) == false)
    {
      Instantiate(door, new Vector3(p.x + 0.5f, 0, p.y + 0.5f), Quaternion.Euler(0, 90, 0));
    }


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
