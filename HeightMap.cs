using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMap : MonoBehaviour
{
    public float width = 0.1f;
    public int N = 10;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    // 用来存放顶点数据
    List<Vector3> verts;
    List<int> indices;//序号


    public float height=6f;
    public Texture2D heightMap;    //png格式


    private void Awake()
    {
    }

    private void Start()
    {
        verts = new List<Vector3>();
        indices = new List<int>();

        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        Generate();
    }

    public void Generate()
    {
        ClearMeshData();

        // 把数据填写好
        AddMeshData();

        // 把数据传递给Mesh，生成真正的网格
        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        //mesh.uv = uvs.ToArray();
        mesh.triangles = indices.ToArray();

        mesh.RecalculateNormals(); //重新计算网格法线
        mesh.RecalculateBounds(); //重新计算从网格包围体的顶点

        meshFilter.mesh = mesh; //将网格放入生成
        // 碰撞体专用的mesh，只负责物体的碰撞外形
        meshCollider.sharedMesh = mesh;
    }

    void ClearMeshData()
    {
        verts.Clear();
        indices.Clear();
    }

    void AddMeshData()
    {
        int NMap = 512;
        Color32[] colors = heightMap.GetPixels32(); //color32得r g b a,color直接得颜色
        //Color a = Color.red;
        
        for(int z=0;z<N;z++)
        {

            for(int x=0;x<N;x++)
            {
                int xx = Mathf.RoundToInt( x * 1.0f / N *NMap); 
                int zz = Mathf.RoundToInt(z * 1.0f / N *NMap);
                float y = colors[zz*NMap+xx].r/255f*height;
                Vector3 p= new Vector3(x,y,z)*width;
                verts.Add(p);
            }
        }

        //indices.Add(0); indices.Add(1); indices.Add(2);
        //indices.Add(0); indices.Add(2); indices.Add(3);
        for (int z = 0; z < N-1; z++)
        {
            for (int x = 0; x < N-1; x++)
            {
                int index = z * N + x; //左下
                int index1 = (z + 1) * N + x; //左上
                int index2 = (z + 1) * N + x+1; //右上
                int index3 = z * N + x+1; //右下

                indices.Add(index); indices.Add(index1); indices.Add(index2);
                indices.Add(index); indices.Add(index2); indices.Add(index3);
            }
        }
    }
    float lastUpdateTime = 0;
    private void Update()
    {
        if(Time.time>=lastUpdateTime+0.3f)
        {
            Generate();
            lastUpdateTime = Time.time;
        }
    }
}
