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
  List<Vector2>uvs = new List<Vector2>();

  public WorldGenerator wget;

  // Start is called before the first frame update
  void Start()
  {
    CreateVoxel(new Vector3(0, 0, 0));
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void CreateVoxel(Vector3 pos)
  {
    for (int face = 0; face < 6; face++)
    {
      vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTriangles[face, 0]]);
      vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTriangles[face, 1]]);
      vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTriangles[face, 2]]);
      vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTriangles[face, 3]]);

      uvs.Add(VoxelData.uvs[0]);
      uvs.Add(VoxelData.uvs[1]);
      uvs.Add(VoxelData.uvs[2]);
      uvs.Add(VoxelData.uvs[3]);

      triangles.Add(vertexIndex);
      triangles.Add(vertexIndex + 1);
      triangles.Add(vertexIndex + 2);
      triangles.Add(vertexIndex + 2);
      triangles.Add(vertexIndex + 1);
      triangles.Add(vertexIndex + 3);
      vertexIndex += 4;

    }

    Mesh mesh = new Mesh();
    mesh.vertices = vertices.ToArray();
    mesh.triangles = triangles.ToArray();
    mesh.uv = uvs.ToArray();
    mesh.RecalculateNormals();
    meshFilter.mesh = mesh;
  }

  
  
}
