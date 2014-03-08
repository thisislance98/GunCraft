/* All contents herein copyright 2010, David Butler */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TerrainPrefabBrain : MonoBehaviour 
{
	public GameObject ExplodingCubesPrefab;
	public GameObject MagicCubePrefab;
	public GameObject smallExplosionPrefab;
	public GameObject RewardPrefab;
	public Material Mat;
    private MeshFilter m_meshFilter;
	private static Transform _player = null;
	private static Dictionary<IntCoords,int> _blockDamage = new Dictionary<IntCoords, int>();
	private static int[] _blockTypeHitCounts = new int[] {0,1,1,1,2,30,10};

    //private int[, ,] m_tmpdata;

    bool m_leftButtonDown = false;
    bool m_rightButtonDown = false;

    //public Texture2D[] textures;
	Rect[] groundUVs;
    //public Texture dirt;
    //public Texture grass;
	
	private Texture2D groundTexture;


    Vector3 offset;

    int chunkSize;


    GameObject[] m_neighbors = new GameObject[6];
    public enum NeighborDir
    {
        Z_PLUS,
        Z_MINUS,
        Y_PLUS,
        Y_MINUS,
        X_PLUS,
        X_MINUS
    }

	// Use this for initialization
	void Start () 
    {
		/*groundTexture = new Texture2D(2048, 2048);
		groundUVs = groundTexture.PackTextures(textures, 0, 2048);*/
		if (_player == null)
			_player = GameObject.FindGameObjectWithTag("Player").transform;
        chunkSize = TerrainBrain.chunkSize;
        offset = transform.position;
        //m_tmpdata = TerrainBrain.Instance().getTerrainData(transform.position); //new int[10, 10, 10];


        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshCollider>();
        
        m_meshFilter = this.GetComponent<MeshFilter>();

        Mesh newMesh = new Mesh();
        m_meshFilter.mesh = newMesh;

		groundUVs = TerrainBrain.Instance().getUVs();
		//Debug.Log("UV size = " + groundUVs.Length.ToString());

        //Debug.Log("Chunk is in location: " + transform.position);
        regenerateMesh();
		
		renderer.material.shader = Shader.Find("Mobile/Diffuse");
    
	//	   renderer.material.shader = shader;
       	 renderer.material.mainTexture = TerrainBrain.Instance().getGroundTexture(); //groundTexture;
	//	renderer.material = Mat;
  //      renderer.material.SetFloat("_LightPower", 0.5f);

        this.enabled = true;
	}


    public void setNeighbor(NeighborDir dir, GameObject tpb)
    {
        m_neighbors[(int)dir] = tpb;
    }

    public GameObject getNeighbor(NeighborDir dir)
    {
        return m_neighbors[(int)dir];
    }
	

    void OnMouseOver()
    {
        if (Input.GetMouseButton(0) && !m_leftButtonDown)
        {
  //          m_leftButtonDown = true;
  //          doLeftClick();
        }
        else if (!Input.GetMouseButton(0))
        {
            m_leftButtonDown = false;
        }
        
        if (Input.GetMouseButton(1) && !m_rightButtonDown)
        {
            m_rightButtonDown = true;
			doRightClick();

        }
        else if (!Input.GetMouseButton(1))
        {
            m_rightButtonDown = false;
        }
    }

    int getTerrainDensity(Vector3 pos)
    {
        /*if (pos.z < 0 || pos.z > 9 || pos.y < 0 || pos.y > 9 || pos.x < 0 || pos.x > 9)
            return 0;
        else
        {*/
            pos += transform.position;
            return TerrainBrain.Instance().getTerrainDensity((int)pos.x, (int)pos.y, (int)pos.z);
        //}
            //return m_tmpdata[(int)pos.x, (int)pos.y, (int)pos.z];
    }

    void addUV(int density, ref List<Vector2> uvs)
    {
        density = Mathf.Clamp(density-1, 0, groundUVs.Length);
		
        const float epsilon = 0.001f;
		/*
       // 0.125
        // (0,0), (1,1), (1,0), (0,1)
        
        float left = density * 0.125f + epsilon;
        float right = (density + 1) * 0.125f - epsilon;
        float top = 1.0f - epsilon;
        float bottom = 0.875f + epsilon;

        uvs.Add(new Vector2(left, bottom));
        uvs.Add(new Vector2(right, top));
        uvs.Add(new Vector2(right, bottom));
        uvs.Add(new Vector2(left, top));*/
		uvs.Add(new Vector2(groundUVs[density].x+epsilon, groundUVs[density].y+epsilon));
		uvs.Add(new Vector2(groundUVs[density].x + groundUVs[density].width - epsilon, groundUVs[density].y + groundUVs[density].height - epsilon));
		uvs.Add(new Vector2(groundUVs[density].x + groundUVs[density].width - epsilon, groundUVs[density].y + epsilon));
		uvs.Add(new Vector2(groundUVs[density].x + epsilon, groundUVs[density].y + groundUVs[density].height - epsilon));
    }

    void regenerateMesh()
    {
		offset = transform.position;
        Mesh subMesh = this.GetComponent<MeshFilter>().mesh;
        MeshCollider mc = gameObject.GetComponent<MeshCollider>();
        
        List<int> indices = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> verts = new List<Vector3>();
        List<Color> colors = new List<Color>();

        float shade = 0.0f;

        subMesh.Clear();

        int curVert = 0;
        Vector3 curVec = new Vector3(0, 0, 0);

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    shade = 0.45f;// (float)rand.NextDouble();
                    curVec.x = x;
                    curVec.y = y;
                    curVec.z = z;

                    int density = getTerrainDensity(curVec);
                    if (density == 0)
                        continue;

                    /*if (getTerrainValue(curVec) != d)// && d != 2)
                        continue;*/

                    curVec.z--;

                    if (getTerrainDensity(curVec) == 0)
                    {
                        verts.Add(new Vector3(x, y, z));
                        verts.Add(new Vector3(x + 1, y + 1, z));
                        verts.Add(new Vector3(x + 1, y, z));
                        verts.Add(new Vector3(x, y + 1, z));

                        colors.Add(new Color(shade, shade, shade));
                        colors.Add(new Color(shade, shade, shade));
                        colors.Add(new Color(shade, shade, shade));
                        colors.Add(new Color(shade, shade, shade));

                        addUV(density, ref uvs);
                        /*uvs.Add(new Vector2(0, 0));
                        uvs.Add(new Vector2(1, 1));
                        uvs.Add(new Vector2(1, 0));
                        uvs.Add(new Vector2(0, 1));*/

                        indices.Add(curVert);
                        indices.Add(curVert + 1);
                        indices.Add(curVert + 2);

                        indices.Add(curVert);
                        indices.Add(curVert + 3);
                        indices.Add(curVert + 1);

                        curVert += 4;
                    }
                    curVec.z += 2;
                    if (getTerrainDensity(curVec) == 0)
                    {
                        verts.Add(new Vector3(x, y, z + 1));
                        verts.Add(new Vector3(x + 1, y + 1, z + 1));
                        verts.Add(new Vector3(x + 1, y, z + 1));
                        verts.Add(new Vector3(x, y + 1, z + 1));

                        colors.Add(new Color(shade, shade, shade));
                        colors.Add(new Color(shade, shade, shade));
                        colors.Add(new Color(shade, shade, shade));
                        colors.Add(new Color(shade, shade, shade));

                        addUV(density, ref uvs);
                        /*uvs.Add(new Vector2(0, 0));
                        uvs.Add(new Vector2(1, 1));
                        uvs.Add(new Vector2(1, 0));
                        uvs.Add(new Vector2(0, 1));*/

                        indices.Add(curVert);
                        indices.Add(curVert + 2);
                        indices.Add(curVert + 1);

                        indices.Add(curVert);
                        indices.Add(curVert + 1);
                        indices.Add(curVert + 3);

                        curVert += 4;
                    }
                    curVec.z = z;

                    curVec.y--;
                    if (getTerrainDensity(curVec) == 0)
                    {
                        verts.Add(new Vector3(x, y, z));
                        verts.Add(new Vector3(x + 1, y, z + 1));
                        verts.Add(new Vector3(x + 1, y, z));
                        verts.Add(new Vector3(x, y, z + 1));

                        colors.Add(new Color(shade, shade, shade));
                        colors.Add(new Color(shade, shade, shade));
                        colors.Add(new Color(shade, shade, shade));
                        colors.Add(new Color(shade, shade, shade));

                        addUV(density, ref uvs);
                        /*uvs.Add(new Vector2(0, 0));
                        uvs.Add(new Vector2(1, 1));
                        uvs.Add(new Vector2(1, 0));
                        uvs.Add(new Vector2(0, 1));*/

                        indices.Add(curVert);
                        indices.Add(curVert + 2);
                        indices.Add(curVert + 1);

                        indices.Add(curVert);
                        indices.Add(curVert + 1);
                        indices.Add(curVert + 3);

                        curVert += 4;
                    }
                    curVec.y += 2;
					shade=0.5f;
                    if (getTerrainDensity(curVec) == 0)
                    {
                        verts.Add(new Vector3(x, y + 1, z));
                        verts.Add(new Vector3(x + 1, y + 1, z + 1));
                        verts.Add(new Vector3(x + 1, y + 1, z));
                        verts.Add(new Vector3(x, y + 1, z + 1));

                        colors.Add(new Color(shade, shade, shade));
                        colors.Add(new Color(shade, shade, shade));
                        colors.Add(new Color(shade, shade, shade));
                        colors.Add(new Color(shade, shade, shade));

                        addUV(density, ref uvs);
                        /*uvs.Add(new Vector2(0, 0));
                        uvs.Add(new Vector2(1, 1));
                        uvs.Add(new Vector2(1, 0));
                        uvs.Add(new Vector2(0, 1));*/

                        indices.Add(curVert);
                        indices.Add(curVert + 1);
                        indices.Add(curVert + 2);

                        indices.Add(curVert);
                        indices.Add(curVert + 3);
                        indices.Add(curVert + 1);

                        curVert += 4;
                    }
                    curVec.y = y;
					shade = 0.45f;
                    curVec.x--;
                    if (getTerrainDensity(curVec) == 0)
                    {
                        verts.Add(new Vector3(x, y, z));
                        verts.Add(new Vector3(x, y + 1, z + 1));
                        verts.Add(new Vector3(x, y + 1, z));
                        verts.Add(new Vector3(x, y, z + 1));

                        colors.Add(new Color(shade, shade, shade));
                        colors.Add(new Color(shade, shade, shade));
                        colors.Add(new Color(shade, shade, shade));
                        colors.Add(new Color(shade, shade, shade));

                        addUV(density, ref uvs);
                        /*uvs.Add(new Vector2(0, 0));
                        uvs.Add(new Vector2(1, 1));
                        uvs.Add(new Vector2(1, 0));
                        uvs.Add(new Vector2(0, 1));*/

                        indices.Add(curVert);
                        indices.Add(curVert + 3);
                        indices.Add(curVert + 1);

                        indices.Add(curVert);
                        indices.Add(curVert + 1);
                        indices.Add(curVert + 2);

                        curVert += 4;
                    }
                    curVec.x += 2;
                    if (getTerrainDensity(curVec) == 0)
                    {
                        verts.Add(new Vector3(x + 1, y, z));
                        verts.Add(new Vector3(x + 1, y + 1, z + 1));
                        verts.Add(new Vector3(x + 1, y + 1, z));
                        verts.Add(new Vector3(x + 1, y, z + 1));

                        colors.Add(new Color(shade, shade, shade));
                        colors.Add(new Color(shade, shade, shade));
                        colors.Add(new Color(shade, shade, shade));
                        colors.Add(new Color(shade, shade, shade));

                        addUV(density, ref uvs);
                        /*uvs.Add(new Vector2(0, 0));
                        uvs.Add(new Vector2(1, 1));
                        uvs.Add(new Vector2(1, 0));
                        uvs.Add(new Vector2(0, 1));*/

                        indices.Add(curVert);
                        indices.Add(curVert + 1);
                        indices.Add(curVert + 3);

                        indices.Add(curVert);
                        indices.Add(curVert + 2);
                        indices.Add(curVert + 1);

                        curVert += 4;
                    }
                }
            }
        }

        subMesh.vertices = verts.ToArray();
        subMesh.triangles = indices.ToArray();
        subMesh.uv = uvs.ToArray();
        subMesh.colors = colors.ToArray();

        subMesh.Optimize();
        subMesh.RecalculateNormals(); 
        
        //if (mc.sharedMesh == null) mc.sharedMesh = subMesh;
		mc.sharedMesh = new Mesh();
		mc.sharedMesh = subMesh;

	//	mc.sharedMesh.Optimize();
    }

    public static void SolveTangents(Mesh mesh)
    {
        int triangleCount = mesh.triangles.Length / 3;
        int vertexCount = mesh.vertices.Length;

        Vector3[] tan1 = new Vector3[vertexCount];
        Vector3[] tan2 = new Vector3[vertexCount];

        Vector4[] tangents = new Vector4[vertexCount];

        for (long a = 0; a < triangleCount; a += 3)
        {
            long i1 = mesh.triangles[a + 0];
            long i2 = mesh.triangles[a + 1];
            long i3 = mesh.triangles[a + 2];

            Vector3 v1 = mesh.vertices[i1];
            Vector3 v2 = mesh.vertices[i2];
            Vector3 v3 = mesh.vertices[i3];

            Vector2 w1 = mesh.uv[i1];
            Vector2 w2 = mesh.uv[i2];
            Vector2 w3 = mesh.uv[i3];

            float x1 = v2.x - v1.x;
            float x2 = v3.x - v1.x;
            float y1 = v2.y - v1.y;
            float y2 = v3.y - v1.y;
            float z1 = v2.z - v1.z;
            float z2 = v3.z - v1.z;

            float s1 = w2.x - w1.x;
            float s2 = w3.x - w1.x;
            float t1 = w2.y - w1.y;
            float t2 = w3.y - w1.y;

            float r = 1.0f / (s1 * t2 - s2 * t1);

            Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;

            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;
        }


        for (long a = 0; a < vertexCount; ++a)
        {
            Vector3 n = mesh.normals[a];
            Vector3 t = tan1[a];

            Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
            tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);

            tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
        }

        mesh.tangents = tangents;
    }

	public static GameObject findTerrainChunk(Vector3 worldPos)
	{
		int chunkX = (int)Mathf.Floor( (worldPos.x / TerrainBrain.chunkSize));
		int chunkY = (int)Mathf.Floor( (worldPos.y / TerrainBrain.chunkSize));
		int chunkZ = (int)Mathf.Floor( (worldPos.z / TerrainBrain.chunkSize));

		return findTerrainChunk(chunkX,chunkY,chunkZ);
	}
	
	public static GameObject findTerrainChunk(int x, int y, int z)
    {
        GameObject ob = GameObject.Find("TerrainChunk (" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")");
		if (ob == null)
		{
//			Debug.LogWarning("Trouble finding chunk " + x.ToString() + ", " + y.ToString() + ", " + z.ToString());
		}
		return ob;
    }

    GameObject findNeighbor(NeighborDir dir, int x, int y, int z)
    {
        switch (dir)
        {
            case NeighborDir.X_MINUS:
                //if (m_neighbors[0] != null)
                //    return m_neighbors[0];
                m_neighbors[0] = findTerrainChunk(x - 1, y, z);
                return m_neighbors[0];
            case NeighborDir.X_PLUS:
                //if (m_neighbors[1] != null)
                 //   return m_neighbors[1];
                m_neighbors[1] = findTerrainChunk(x + 1, y, z);
                return m_neighbors[1];
            case NeighborDir.Y_MINUS:
                //if (m_neighbors[2] != null)
                //    return m_neighbors[2];
                m_neighbors[2] = findTerrainChunk(x, y - 1, z);
                return m_neighbors[2];
            case NeighborDir.Y_PLUS:
                //if (m_neighbors[3] != null)
                //    return m_neighbors[3];
                m_neighbors[3] = findTerrainChunk(x, y + 1, z);
                return m_neighbors[3];
            case NeighborDir.Z_MINUS:
                //if (m_neighbors[4] != null)
                //    return m_neighbors[4];
                m_neighbors[4] = findTerrainChunk(x, y, z - 1);
                return m_neighbors[4];
            case NeighborDir.Z_PLUS:
                //if (m_neighbors[5] != null)
                //    return m_neighbors[5];
                m_neighbors[5] = findTerrainChunk(x, y, z + 1);
                return m_neighbors[5];
            default:
                return null;
        }
    }

	void doRightClick()
	{
	
	//	ShootCube(false);  
	}
	
	Vector3 GetCubeCenterInWorldCoords(Vector3 worldPos)
	{
		float x = Mathf.Floor(worldPos.x) + .5f;
		float y = Mathf.Floor(worldPos.y) + .5f;
		float z = Mathf.Floor(worldPos.z) + .5f;

		return new Vector3(x,y,z);
	}

	void DestroyCubeFromHit(Vector3 hitPos, Vector3 centerPos, int hitCubeDensity)
	{
		TerrainBrain.Instance().setTerrainDensity(hitPos,0);
		PickupManager.Instance.OnBlockDestoyed(centerPos,hitCubeDensity);
		TextureManager.Instance.OnDestroyedBlock(hitCubeDensity);
	}
	
    public bool OnBulletHit(Vector3 hitPos, ShotType shotType,int terrainDensity, bool applyDamage)
    {
        //float startTime = Time.realtimeSinceStartup;
		bool destroyCube = (shotType == ShotType.Destroy);
		bool didModify = false;
		Vector3 center = GetCubeCenterInWorldCoords(hitPos);
		int hitCubeDensity = TerrainBrain.Instance().getTerrainDensity(hitPos);


		if (destroyCube)
		{
			int blockHitCount = _blockTypeHitCounts[hitCubeDensity];
		
			// just destroy the block if its hit count is 1 
			if (blockHitCount <= 1 || applyDamage == false)
			{
				DestroyCubeFromHit(hitPos,center,hitCubeDensity);
				didModify = true;
			}
			else // otherwise check hits damagage
			{
				IntCoords blockCoords = TerrainBrain.GetCubeCoords(hitPos);


				// has the block already taken damage
				if (_blockDamage.ContainsKey(blockCoords))
				{
					int damage = _blockDamage[blockCoords];
					damage++;

					// if the block has taken enough damage destroy it
					if (damage >= blockHitCount)
					{
						DestroyCubeFromHit(hitPos,center,hitCubeDensity);
						didModify = true;
						_blockDamage.Remove(blockCoords);
					}
					else // otherwise just increase it's damage
						_blockDamage[blockCoords] = damage;

				}
				else // this was the first hit on a block that has more than 1 for hit count so just set its damage to 1
				{
					_blockDamage[blockCoords] = 1;
				}
			}



		}
		else // create a cube
		{


			Vector3 closestPoint = _player.collider.ClosestPointOnBounds(center);
			float dist = Vector3.Distance(closestPoint,center);

			// dont create cube if it's too close to the player
			if (dist <= .5f)
				return false;


			didModify = true;

			TextureManager.Instance.OnCreatedBlock(terrainDensity);
			TerrainBrain.Instance().setTerrainDensity(hitPos,terrainDensity);

		}

		if (didModify)
		{
		    int chunkX = (int)(offset.x / chunkSize);
		    int chunkY = (int)(offset.y / chunkSize);
		    int chunkZ = (int)(offset.z / chunkSize);


//			Debug.Log("center pos: " + GetCubeCenterInWorldCoords(hitPos) + "offset : " + offset.x + "," + offset.y + "," + offset.z );
		    GameObject n = null;

			if (hitPos.x - offset.x < 1)
		    {
		        n = findNeighbor(NeighborDir.X_MINUS, chunkX, chunkY, chunkZ); // findTerrainChunk(chunkX - 1, chunkY, chunkZ); // getNeighbor(NeighborDir.X_MINUS);
//		       	if (n != null)
//					Debug.Log ("regen left");
//				else
//					Debug.Log("coulnt find left");


				if (n != null) n.SendMessage("regenerateMesh"); //n.GetComponent<TerrainPrefabBrain>().regenerateMesh();
		    }
			else if (hitPos.x - offset.x > chunkSize - 1)
		    {
		        n = findNeighbor(NeighborDir.X_PLUS, chunkX, chunkY, chunkZ);  //findTerrainChunk(chunkX + 1, chunkY, chunkZ);
		        if (n != null) n.SendMessage("regenerateMesh");
//				if (n != null)
//					Debug.Log ("regen right");
//				else
//					Debug.Log("coulnt find right");
		    }
			if (hitPos.y - offset.y < 1)
		    {
		        n = findNeighbor(NeighborDir.Y_MINUS, chunkX, chunkY, chunkZ);  //findTerrainChunk(chunkX, chunkY - 1, chunkZ);
		        if (n != null) n.SendMessage("regenerateMesh");
//				if (n != null)
//					Debug.Log ("regen down");
//				else
//					Debug.Log("coulnt find down");
		    }
			else if (hitPos.y - offset.y > chunkSize - 1)
		    {
		        n = findNeighbor(NeighborDir.Y_PLUS, chunkX, chunkY, chunkZ); //findTerrainChunk(chunkX, chunkY + 1, chunkZ);
		        if (n != null) n.SendMessage("regenerateMesh");
//				if (n != null)
//					Debug.Log ("regen up");
//				else
//					Debug.Log("coulnt find up");
		    }
			if (hitPos.z - offset.z < 1)
		    {
		        n = findNeighbor(NeighborDir.Z_MINUS, chunkX, chunkY, chunkZ);  //findTerrainChunk(chunkX, chunkY, chunkZ - 1);
		        if (n != null) n.SendMessage("regenerateMesh");
//				if (n != null)
//					Debug.Log ("regen back");
//				else
//					Debug.Log("coulnt find back");
		    }
			else if (hitPos.z - offset.z > chunkSize - 1)
		    {
		        n = findNeighbor(NeighborDir.Z_PLUS, chunkX, chunkY, chunkZ);  //findTerrainChunk(chunkX, chunkY, chunkZ + 1);
		        if (n != null) n.SendMessage("regenerateMesh");
//				if (n != null)
//					Debug.Log ("regen forward");
//				else
//					Debug.Log("coulnt find forward");
		    }

			bool regenerating = (n != null);


		    regenerateMesh();
		}
		
//		if (destroyCube)
//		{
//			Instantiate(smallExplosionPrefab,hitPos,Quaternion.identity);	
//
//		}
		return didModify;
    }
	

	

}
