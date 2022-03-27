using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
  public MeshRenderer meshRenderer;
  public MeshFilter meshFilter;
  public MeshCollider meshCollider;

  int vertexIndex = 0;
  List<Vector3> vertices = new List<Vector3>();
  List<int> triangles = new List<int>();
  List<Vector2> uvs = new List<Vector2>();

  public WorldGenerator worldGenerator;
  

  // Start is called before the first frame update
  void Start()
  {
    //CreateVoxel(new Vector3(0, 0, 0));
  }


  public void CreateVoxel(Vector3 pos)
  {
    for (int face = 0; face < 6; face++)
    {
      int px = (int)pos.x;
      int py = (int)pos.z;
      bool createFace = false;

      if (worldGenerator.CanWalk(px, py))
      {
        if (face == 2) createFace = true;
        if (face == 3) createFace = true;
      }
      else
      {
        if (face == 0 && worldGenerator.CanWalk(px, py - 1)) createFace = true;
        if (face == 1 && worldGenerator.CanWalk(px, py + 1)) createFace = true;

        if (face == 4 && worldGenerator.CanWalk(px - 1, py)) createFace = true;
        if (face == 5 && worldGenerator.CanWalk(px + 1, py)) createFace = true;



      }


      if (createFace)
      {
        vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTriangles[face, 0]]);
        vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTriangles[face, 1]]);
        vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTriangles[face, 2]]);
        vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTriangles[face, 3]]);

        if (face == 0 || face == 1 || face == 4 || face == 5)
          AddTexture(0);
        else if (face == 2)
          AddTexture(2);
        else if (face == 3)
          AddTexture(1);



        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 3);
        vertexIndex += 4;
      }

    }

    Mesh mesh = new Mesh();
    mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
    mesh.vertices = vertices.ToArray();
    mesh.triangles = triangles.ToArray();
    mesh.uv = uvs.ToArray();
    mesh.RecalculateNormals();
    meshFilter.mesh = mesh;
    meshCollider.sharedMesh = mesh;
  }

  void AddTexture(int textureID)
  {
    float y = textureID / VoxelData.TextureAtlasSizeInBlocks;
    float x = textureID - (y * VoxelData.TextureAtlasSizeInBlocks);

    x *= VoxelData.NormalizedBlockTextureSize;
    y *= VoxelData.NormalizedBlockTextureSize;

    y = 1.0f - y - VoxelData.NormalizedBlockTextureSize;
    float norm = VoxelData.NormalizedBlockTextureSize;

    uvs.Add(new Vector2(x, y));
    uvs.Add(new Vector2(x, y + norm));
    uvs.Add(new Vector2(x + norm, y));
    uvs.Add(new Vector2(x + norm, y + norm));

  }

  public void Populate(Vector2 pos)
  {
    for (int x = 0; x < VoxelData.chunkWidth; x++)
    {
      for (int z = 0; z < VoxelData.chunkWidth; z++)
      {
        int cx = (int)pos.x + x;
        int cz = (int)pos.y + z;

        if (worldGenerator.CreateMe(cx, cz))
          CreateVoxel(new Vector3(pos.x + x, 0, pos.y + z));

      }
    }

  }
}
