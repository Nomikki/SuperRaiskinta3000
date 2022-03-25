using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData
{

  public static readonly int dungeonSize = 80;
  public static readonly int chunkWidth = 16;

  public static readonly Vector3[] voxelVerts = new Vector3[8] {
    new Vector3(0, 0, 0), // 0
    new Vector3(1, 0, 0), // 1
    new Vector3(1, 1, 0), // 2
    new Vector3(0, 1, 0), // 3

    new Vector3(0, 0, 1), // 4
    new Vector3(1, 0, 1), // 5
    new Vector3(1, 1, 1), // 6
    new Vector3(0, 1, 1), // 7
    };

    public static readonly int[,] voxelTriangles = new int[6, 4] {
      {0, 3, 1, 2}, //0 back face
      {5, 6, 4, 7}, //1 front face
      {7, 3, 6, 2}, //2 top face
      {5, 1, 4, 0}, //3 bottom face
      {4, 7, 0, 3}, //4 left face
      {1, 2, 5, 6}, //5 right face
    };


    public static readonly Vector2[] uvs = new Vector2[4] {
      new Vector2(0, 0),
      new Vector2(0, 1),
      new Vector2(1, 0),
      new Vector2(1, 1),
    };

}

