/* All contents herein copyright 2010, David Butler */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Parse;
using System.Threading.Tasks;


class IntCoords : IEquatable<IntCoords>
{
    public int X;
    public int Y;
    public int Z;



    public IntCoords(int x, int y, int z)
    {
        X = x; Y = y; Z = z;
    }

	public IntCoords(string coords)
	{
		string[] xyzArray = coords.Split(new char[]{','});	
		X = int.Parse(xyzArray[0]);
		Y = int.Parse(xyzArray[1]);
		Z = int.Parse(xyzArray[2]);
	}
		
    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode() * 27644437 ^ Z.GetHashCode() * 1073676287;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        return Equals((IntCoords)obj);
    }

    public bool Equals(IntCoords other)
    {
		if (other == null || GetType() != other.GetType())
			return false;

        return other.X.Equals(X) && other.Y.Equals(Y) && other.Z.Equals(Z);
    }

    public override string ToString()
    {
        return  X.ToString() + "," + Y.ToString() + "," + Z.ToString();
    }
}

class LightCache
{
	
}

class TerrainCache
{
    // Caches the density for terrain blocks, will also store updates eventually
    //Dictionary<Vector3, int[,,]> m_data = new Dictionary<Vector3,int[,,]>();
    Dictionary<IntCoords, int[, ,]> m_data = new Dictionary<IntCoords, int[, ,]>();

	
    IntCoords m_lastCoords;
    int[, ,] m_lastData;
	List<IntCoords> m_modifiedChunks = new List<IntCoords>();
	
	// gets the chunk at the give int world coords.. if it doesn't not exist.. create it
    public int[,,] getChunk(int x, int y, int z)
    {
        IntCoords coords = new IntCoords(x, y, z);
        if (coords.Equals(m_lastCoords))
            return m_lastData;

        m_lastCoords = coords;

        if (m_data.ContainsKey(coords))
        {
            m_lastData = m_data[coords];
            return m_lastData;
        }
		
		// has this chunk been modified previously
		string coordString = coords.ToString();
		
//		if (PlayerPrefs.HasKey(coordString))
//		{
//
//			string chunkString = PlayerPrefs.GetString(coordString);
//			m_lastData = StringToChunk(chunkString);
//			m_data[coords] = m_lastData;
//			return m_lastData;
//		}
		
        int chunkSize = TerrainBrain.chunkSize;

        m_lastData = new int[chunkSize, chunkSize, chunkSize];
        TerrainBrain tb = TerrainBrain.Instance();

        for (int t = 0; t < chunkSize; t++)
        {
            for (int u = 0; u < chunkSize; u++)
            {
                for (int v = 0; v < chunkSize; v++)
                {
                    Vector3 loc = new Vector3((float)(x+t), (float)(y+u), (float)(z+v)) / TerrainBrain.noiseMultiplier;
                    //Vector3 cpos = new Vector3((float)t, (float)u, (float)v);
					
                    m_lastData[t, u, v] = tb.GetRandomDensity(loc);
					
					
                }
            }
        }

        //Debug.Log("Stored cache:" + coords.ToString());
        m_data[coords] = m_lastData;
		
		
			
        return m_lastData;
    }
	
	string ChunkToString(int[,,] chunk)
	{
		int chunkSize = TerrainBrain.chunkSize;
		byte[] byteArray = new byte[sizeof(int) * chunkSize * chunkSize * chunkSize];
		Buffer.BlockCopy(chunk,0, byteArray,0, byteArray.Length);
			
		return Convert.ToBase64String(byteArray);
	}
	
	int[,,] StringToChunk(string chunkString)
	{
		int chunkSize = TerrainBrain.chunkSize;
		byte[] byteArray = Convert.FromBase64String(chunkString);
		int[,,] chunk = new int[chunkSize,chunkSize,chunkSize];
		
		Buffer.BlockCopy(byteArray,0,chunk,0,byteArray.Length);
		return chunk;
	}

	public IEnumerator LoadWorld()
	{
		ParseQuery<ParseObject> query = ParseObject.GetQuery("World");

		Task<IEnumerable<ParseObject>> task = query.FindAsync();

		while (!task.IsCompleted) yield return null;

		IEnumerable<ParseObject> allObjs = task.Result;
		
		// first update the objects that have been modified and are currently in the database
		foreach (ParseObject obj in allObjs)
		{
			int x = obj.Get<int>("x");
			int y = obj.Get<int>("y");
			int z = obj.Get<int>("z");
			IntCoords coord = new IntCoords(x,y,z);
			
			m_data[coord] = StringToChunk(obj.Get<string>("chunk"));

			GameObject chunkObj = TerrainPrefabBrain.findTerrainChunk(x/TerrainBrain.chunkSize,y/TerrainBrain.chunkSize,z/TerrainBrain.chunkSize);
	
			
			if (chunkObj != null)
				chunkObj.SendMessage("regenerateMesh");

		}
	}
	
	public void SaveWorld()
	{
		if (m_modifiedChunks.Count == 0)
			return;

		Debug.Log("saving started");
		List<ParseObject> objs = new List<ParseObject>();

		List<int> xList = new List<int>();
		List<int> yList = new List<int>();
		List<int> zList = new List<int>();

		foreach(IntCoords coord in m_modifiedChunks)
		{
			xList.Add(coord.X);
			yList.Add(coord.Y);
			zList.Add(coord.Z);

		}

		var query = ParseObject.GetQuery("World").WhereContainedIn("x",xList).WhereContainedIn("y",yList).WhereContainedIn("z",zList);

//		ParseQuery<ParseObject> query = ParseObject.GetQuery("World").
//			WhereEqualTo("x",m_modifiedChunks[0].X);//.
//		//		WhereEqualTo("y",m_modifiedChunks[0].Y).
//		//		WhereEqualTo("z",m_modifiedChunks[0].Z);

//		Debug.Log("1_1");
//
//		ParseQuery<ParseObject> combinedQuery = null;
//		// create a compound query to find all the chunks that have been modified and are in the database
//		for (int i=1; i < m_modifiedChunks.Count; i++)
//		{
//			IntCoords coord = m_modifiedChunks[i];
//			Debug.Log("1_11");
//			ParseQuery<ParseObject> newQuery = ParseObject.GetQuery("World").
//				WhereEqualTo("x",coord.X);//.
//		//			WhereEqualTo("y",coord.Y).
//	//				WhereEqualTo("z",coord.Z);
//			Debug.Log("1_2");
//
//		
//			var lotsOfWins = ParseObject.GetQuery("World")
//				.WhereGreaterThan("x", 150);
//			
//			var fewWins = ParseObject.GetQuery("World")
//				.WhereLessThan("y", 5);
//			
//			ParseQuery<ParseObject> q = lotsOfWins.Or(fewWins);
//
////			List<ParseQuery<ParseObject>> list = new List<ParseQuery<ParseObject>>();
////			list.Add(newQuery);
////			list.Add(query);
////			ParseQuery<ParseObject> combined = ParseQuery<ParseObject>.Or(new ArrayList({query,newQuery}));
////			if (combinedQuery == null)
////				combinedQuery = query.Or(newQuery);
////			else
////				combinedQuery = combinedQuery.Or(newQuery);
//	
//			Debug.Log("1_3");
//	//		PlayerPrefs.SetString(coordString,ChunkToString(chunk));
//		}

		Debug.Log("2");
		query.FindAsync().ContinueWith(t =>
		{
			IEnumerable<ParseObject> allObjs = t.Result;

			// first update the objects that have been modified and are currently in the database
			foreach (ParseObject obj in allObjs)
			{
				IntCoords coord = new IntCoords(obj.Get<int>("x"),obj.Get<int>("y"),obj.Get<int>("z"));

				if (m_modifiedChunks.Contains(coord))
				{
					int[,,] chunk = m_data[coord];
					string str = ChunkToString(chunk);

					obj["chunk"] = str;
					objs.Add(obj);
					m_modifiedChunks.Remove(coord);

				}
			}
			Debug.Log("3");
			// now add chunks to the database that are new
			foreach(IntCoords coord in m_modifiedChunks)
			{
				int[,,] chunk = m_data[coord];
				string str = ChunkToString(chunk);

				ParseObject parseObj = new ParseObject("World");
				parseObj["x"] = coord.X;
				parseObj["y"] = coord.Y;
				parseObj["z"] = coord.Z;
				parseObj["chunk"] = str;
				
				objs.Add(parseObj);
				m_modifiedChunks.Remove(coord);
			}
		}).ContinueWith(t => // now save all the parse objects
		{
			Debug.Log("4");
			Task task = ParseObject.SaveAllAsync(objs);
			task.ContinueWith(q => 
			                  {
				Debug.Log("saving complete");
				m_modifiedChunks.Clear();
			});
		});




	}
	
	// gets the density of the cube give world coords as ints
    public int getDensity(int x, int y, int z)
    {
        int chunkSize = TerrainBrain.chunkSize;

        int absX = Math.Abs(x);
        int absY = Math.Abs(y);
        int absZ = Math.Abs(z);

        int modX = x < 0 ? (chunkSize - absX % chunkSize) % chunkSize : x % chunkSize;
        int modY = y < 0 ? (chunkSize - absY % chunkSize) % chunkSize : y % chunkSize;
        int modZ = z < 0 ? (chunkSize - absZ % chunkSize) % chunkSize : z % chunkSize;

        int ciX = x - modX; 
        int ciY = y - modY; 
        int ciZ = z - modZ; 

        //Debug.Log(x.ToString() + ", " + y.ToString() + ", " + z.ToString()); 
        
	
        int[,,] chunk = getChunk(ciX, ciY, ciZ);
        return chunk[modX, modY, modZ];
    }

//	void OnApplicationPause(bool paused) 
//	{
//		if (paused)
//			SaveWorld();
//	}
	
	public void setDensity(int x, int y, int z)
	{
		setDensity(x,y,z,0);	
	}
	
    public void setDensity(int x, int y, int z, int density)
    {
        int chunkSize = TerrainBrain.chunkSize;

        int absX = Math.Abs(x);
        int absY = Math.Abs(y);
        int absZ = Math.Abs(z);

        int modX = x < 0 ? (chunkSize - absX % chunkSize) % chunkSize : x % chunkSize;
        int modY = y < 0 ? (chunkSize - absY % chunkSize) % chunkSize : y % chunkSize;
        int modZ = z < 0 ? (chunkSize - absZ % chunkSize) % chunkSize : z % chunkSize;

        int ciX = x - modX;
        int ciY = y - modY;
        int ciZ = z - modZ;

        IntCoords coords = new IntCoords(ciX, ciY, ciZ);
		if (m_modifiedChunks.Contains (coords) == false)
			m_modifiedChunks.Add(coords);

        // Shouldn't be altering the density of unloaded chunks anyway, eh?
        if (!m_data.ContainsKey(coords))
        {
            Debug.Log("Couldn't find coords: " + coords.ToString());
            return;
        }
		
        //Debug.Log("Setting chunk " + coords.ToString() + ", index (" + modX.ToString() + ", " + modY.ToString() + ", " + modZ.ToString() + ") to nil.");
        m_data[coords][modX, modY, modZ] = density;
    }

}


public class TerrainBrain : MonoBehaviour 
{
	public GameObject prefab;
	static TerrainBrain m_instance;

    public static float noiseMultiplier = 25.0f;
    public static int chunkSize = 10;
	float startGravity;

    Vector3 playerStart = new Vector3(0.5f, 30.5f, 0.5f);
    
	TerrainCache m_tcache = new TerrainCache();

	Texture2D[] textures;
	
	Texture2D groundTexture;
	Rect[] groundUVs;
	
	Vector3 currentPos = Vector3.zero;
	Vector3 lastPos = Vector3.zero;
	
	// Distance in blocks to load terrain
	float viewDistance = 30.0f;
	int blockLoadDistance = 50;
	
	//Queue<GameObject> m_freePool = new Queue<GameObject>();
	Queue<Vector3> m_terrainToCreate = new Queue<Vector3>();
	//float loadDelay = 0.02f;
	//float lastTerrainUpdate = 0.0f;
	
	GameObject[,,] m_meshCache;
	
	public static TerrainBrain Instance()
	{
		
		if (m_instance == null)
		{
			Debug.LogWarning("Lost terrain brain, reaquiring.");
			GameObject ob = GameObject.Find("TerrainManager");
			if (ob == null)
			{
				Debug.LogError("Could not reaquire terrain brain.");
				return null;
			}
			
			m_instance = ob.GetComponent<TerrainBrain>();
			if (m_instance == null)
			{
				Debug.LogError("Could not reaquire terrain brain component.");
				return null;
			}
		}
		
		return m_instance;
	}
	
	public Texture2D getGroundTexture() { return groundTexture; }
	public Rect[] getUVs() { return groundUVs; }
	
	int m_loaded = 10;
	
	// Use this for initialization
	void Start () 
	{
        m_instance = this;

		Application.targetFrameRate = 60;

		textures = TextureManager.Instance.Textures;
	
		Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);

		blockLoadDistance = Mathf.CeilToInt((viewDistance-chunkSize)/chunkSize);
		//Debug.Log("Load distance = " + blockLoadDistance.ToString());
	
		m_meshCache = new GameObject[blockLoadDistance*2+1,blockLoadDistance*2+1,blockLoadDistance*2+1];

		currentPos = playerStart;
		currentPos.x = Mathf.Floor(currentPos.x / chunkSize);
		currentPos.y = Mathf.Floor(currentPos.y / chunkSize);
		currentPos.z = Mathf.Floor(currentPos.z / chunkSize);
		lastPos = currentPos;
	
		groundTexture = new Texture2D(2048, 2048);
		groundTexture.anisoLevel = 9;
	
		groundUVs = groundTexture.PackTextures(textures, 0, 2048);
	
		//Debug.Log("groundUV size = " + groundUVs.Length.ToString());
		//Debug.Log("Ground UVs = " + groundUVs.ToString());

        //GameObject.Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        //GameObject.Instantiate(prefab, new Vector3(0, -10, 0), Quaternion.identity);
		
		Vector3 scanStart = new Vector3(0,100,0);
		int d = 0;

		while (d == 0)
		{
			scanStart.y--;
			d = getTerrainDensity(Mathf.RoundToInt(scanStart.x), Mathf.RoundToInt(scanStart.y), Mathf.RoundToInt(scanStart.z));

		}

		scanStart.y+=3;
		scanStart.x += 0.5f;
		scanStart.z += 0.5f;
		playerStart = scanStart;
		Debug.Log("Start pos = " + scanStart.ToString());

//        Camera.main.transform.position = playerStart;
		GUIText status = GameObject.Find("StatusDisplay").GetComponent<GUIText>();
		status.text = "Loading...";

		startGravity = Camera.main.transform.parent.GetComponent<vp_FPSController>().PhysicsGravityModifier;
		Camera.main.transform.parent.GetComponent<vp_FPSController>().PhysicsGravityModifier=0;		
		Debug.Log("1");
//        stepLoad();

		StartCoroutine( m_tcache.LoadWorld() );
	}
	
	public void SaveWorld()
	{
		m_tcache.SaveWorld();	
	}
	
	int[] getCachedChunkPos(int x, int y, int z)
	{
		// returns position in the cache array based on chunk's world location
		int size = blockLoadDistance*2+1;
		
		int[] pos = new int[3];
		pos[0] = x < 0 ? (size - (-x % size)) % size : x % size;
		pos[1] = y < 0 ? (size - (-y % size)) % size : y % size;
		pos[2] = z < 0 ? (size - (-z % size)) % size : z % size;
		
		return pos;
	}
	
	// creates all the initial chunks and adds them to the chunk cache 
    void stepLoad()
    {
		//Debug.Log("Step loading from " + currentPos.ToString());
        int startX = Mathf.RoundToInt(currentPos.x)-blockLoadDistance;
        int endX = Mathf.RoundToInt(currentPos.x)+blockLoadDistance;
        int startY = Mathf.RoundToInt(currentPos.y)-blockLoadDistance;
        int endY = Mathf.RoundToInt(currentPos.y)+blockLoadDistance;
        int startZ = Mathf.RoundToInt(currentPos.z)-blockLoadDistance;
        int endZ = Mathf.RoundToInt(currentPos.z)+blockLoadDistance;
		
		//Debug.Log("Initial Generation from " + new Vector3(startX, startY, startZ).ToString() + " to " + new Vector3(endX, endY, endZ));
		
        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                for (int z = startZ; z <= endZ; z++)
                {
					//m_terrainToCreate.Enqueue(new Vector3(x,y,z));
                    GameObject newObj = GameObject.Instantiate(prefab, new Vector3(x * chunkSize, y * chunkSize, z * chunkSize), Quaternion.identity) as GameObject;
                    newObj.name = "TerrainChunk (" + x.ToString() + ", Textures" + y.ToString() + ", " + z.ToString() + ")";
					int[] cachePos = getCachedChunkPos(x,y,z);
					m_meshCache[cachePos[0], cachePos[1], cachePos[2]] = newObj;
                }
            }
        }
    }

	
	void HandleMovement ()
	{
		if (Camera.main == null)
			return;
		
		currentPos = Camera.main.transform.position;
		currentPos.x = Mathf.Floor(currentPos.x / chunkSize);
		currentPos.y = Mathf.Floor(currentPos.y / chunkSize);
		currentPos.z = Mathf.Floor(currentPos.z / chunkSize);
		
		
		if (currentPos != lastPos)
		{
			
			Vector3 movement = currentPos-lastPos;
			
			int moveX = Mathf.RoundToInt(movement.x);
			int moveY = Mathf.RoundToInt(movement.y);
			int moveZ = Mathf.RoundToInt(movement.z);
			
			if (moveX != 0)
			{
				
				//int x = Mathf.RoundToInt(lastPos.x) - moveX*blockLoadDistance;
				int newX = Mathf.RoundToInt(lastPos.x) + moveX*(blockLoadDistance + 1);
				
				for (int y = Mathf.RoundToInt(lastPos.y) - blockLoadDistance; y <= Mathf.RoundToInt(lastPos.y) + blockLoadDistance; y++)
				{
					for (int z = Mathf.RoundToInt(lastPos.z) - blockLoadDistance; z <= Mathf.RoundToInt(lastPos.z) + blockLoadDistance; z++)
					{
		
						m_terrainToCreate.Enqueue(new Vector3(newX, y, z));
					}
				}
			}
			if (moveY != 0)
			{
				//int y = Mathf.RoundToInt(lastPos.y) - moveY*blockLoadDistance;
				int newY = Mathf.RoundToInt(lastPos.y) + moveY*(blockLoadDistance + 1);
				
				for (int x = Mathf.RoundToInt(lastPos.x) - blockLoadDistance; x <= Mathf.RoundToInt(lastPos.x) + blockLoadDistance; x++)
				{
					for (int z = Mathf.RoundToInt(lastPos.z) - blockLoadDistance; z <= Mathf.RoundToInt(lastPos.z) + blockLoadDistance; z++)
					{
		
						m_terrainToCreate.Enqueue(new Vector3(x, newY, z));
					}
				}
			}
			if (moveZ != 0)
			{
				//int z = Mathf.RoundToInt(lastPos.z) - moveZ*blockLoadDistance;
				int newZ = Mathf.RoundToInt(lastPos.z) + moveZ*(blockLoadDistance + 1);
				
				for (int x = Mathf.RoundToInt(lastPos.x) - blockLoadDistance; x <= Mathf.RoundToInt(lastPos.x) + blockLoadDistance; x++)
				{
					for (int y = Mathf.RoundToInt(lastPos.y) - blockLoadDistance; y <= Mathf.RoundToInt(lastPos.y) + blockLoadDistance; y++)
					{
		
						m_terrainToCreate.Enqueue(new Vector3(x, y, newZ));
					}
				}
			}
		}
		
		lastPos = currentPos;
	}

	
	// Update is called once per frame
	void Update () 
    {
		if (Input.GetKey(KeyCode.F))
		{
			Debug.Log("saving world");
			SaveWorld();
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (Input.mousePosition.x >= 0 && Input.mousePosition.x <= Screen.width && 
				Input.mousePosition.y >= 0 && Input.mousePosition.y <= Screen.height)
			{
//				Screen.lockCursor = true;
			}
		}

		
		// If chunks of terrain are waiting to be loaded, generate whatever can be done in less than 1/60 second
		float curTime = Time.realtimeSinceStartup;
		while (m_terrainToCreate.Count > 0 && (Time.realtimeSinceStartup - curTime < 0.016 || m_loaded != 0))
		{
			Vector3 loc = m_terrainToCreate.Dequeue();
			int[] pos = getCachedChunkPos(Mathf.RoundToInt(loc.x), Mathf.RoundToInt(loc.y), Mathf.RoundToInt(loc.z));
			GameObject newObj = m_meshCache[pos[0], pos[1], pos[2]];// = m_freePool.Dequeue();
			
			newObj.active = true;				
			newObj.name = "TerrainChunk (" + loc.x.ToString() + ", " + loc.y.ToString() + ", " + loc.z.ToString() + ")";
			newObj.transform.position = loc*chunkSize;
			newObj.SendMessage("regenerateMesh");

		}

	
		// Current workaround to get loading GUI up before generating the terrain
		if (m_loaded > 1)
		{
			m_loaded--;
			return;
		}
		else if (m_loaded == 1)// && Time.time - _timeAtLaterGenerate > 2)
		{
			Camera.main.transform.parent.GetComponent<vp_FPSController>().PhysicsGravityModifier = startGravity;
			stepLoad();
			m_loaded = 0;
			GUIText status = GameObject.Find("StatusDisplay").GetComponent<GUIText>();
			status.text = "";
			//	Camera.main.transform.position = playerStart;
			Camera.main.SendMessage("startRunningFPS");
		}


		
		HandleMovement ();
		
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}

    public void generateTerrainChunk(int x, int y, int z)
    {
        m_tcache.getChunk(x, y, z);
    }
	
	public int getTerrainDensity(Vector3 cubeWorldPos)
	{
		int x = Mathf.CeilToInt(cubeWorldPos.x) - 1;
        int y = Mathf.CeilToInt(cubeWorldPos.y) - 1;
        int z = Mathf.CeilToInt(cubeWorldPos.z) - 1;
		
		return getTerrainDensity(x,y,z);
	}
	
    public int getTerrainDensity(int x, int y, int z)
    {
        //if (y < 0) return 1;
        //else return 0;
        //return 1;
        return m_tcache.getDensity(x, y, z);
        //int[, ,] chunk = m_tcache.getChunk(x - (x % 10), y - (y % 10), z - (z % 10));
        //return chunk[x % 10, y % 10, z % 10];
    }

    public void setTerrainDensity(Vector3 cubeWorldPos, int density)
    {
	    int x = Mathf.CeilToInt(cubeWorldPos.x) - 1;
        int y = Mathf.CeilToInt(cubeWorldPos.y) - 1;
        int z = Mathf.CeilToInt(cubeWorldPos.z) - 1;
		
        m_tcache.setDensity(x, y, z, density);
    }

    public int GetRandomDensity(Vector3 loc)
    {
        //Debug.Log(loc.y);
        float offsetFactor = 500000.0f;
        Vector3 offset = new Vector3(loc.x + offsetFactor, loc.y + offsetFactor, loc.z + offsetFactor);
        float n = noise(loc.x, loc.y, loc.z);
        int d = Mathf.Clamp((int)((1.0f - loc.y + n)*10.0f), 0, 4);
        d = (1.0f  + noise(offset.x, offset.y, offset.z)) > 1.15f ? 0 : d;
        if (d < 0) d = 0;
        return d;
    }

    private int[] A = new int[3];
    private float s, u, v, w;
    private int i, j, k;
    private float onethird = 0.333333333f;
    private float onesixth = 0.166666667f;
    private int[] T;
    //private int[] T = {0x15, 0x38, 0x32, 0x2c, 0x0d, 0x13, 0x07, 0x2a};
    // Simplex Noise Generator
    float noise(float x, float y, float z)
    {
        if (T == null)
        {
            System.Random rand = new System.Random(1000);
		
            T = new int[8];
            for (int q = 0; q < 8; q++)
                T[q] = rand.Next();
        }

        s = (x + y + z) * onethird;
        i = fastfloor(x + s);
        j = fastfloor(y + s);
        k = fastfloor(z + s);

        s = (i + j + k) * onesixth;
        u = x - i + s;
        v = y - j + s;
        w = z - k + s;

        A[0] = 0; A[1] = 0; A[2] = 0;

        int hi = u >= w ? u >= v ? 0 : 1 : v >= w ? 1 : 2;
        int lo = u < w ? u < v ? 0 : 1 : v < w ? 1 : 2;

        return kay(hi) + kay(3 - hi - lo) + kay(lo) + kay(0);
    }

    float kay(int a)
    {
        s = (A[0] + A[1] + A[2]) * onesixth;
        float x = u - A[0] + s;
        float y = v - A[1] + s;
        float z = w - A[2] + s;
        float t = 0.6f - x * x - y * y - z * z;
        int h = shuffle(i + A[0], j + A[1], k + A[2]);
        A[a]++;
        if (t < 0) return 0;
        int b5 = h >> 5 & 1;
        int b4 = h >> 4 & 1;
        int b3 = h >> 3 & 1;
        int b2 = h >> 2 & 1;
        int b1 = h & 3;

        float p = b1 == 1 ? x : b1 == 2 ? y : z;
        float q = b1 == 1 ? y : b1 == 2 ? z : x;
        float r = b1 == 1 ? z : b1 == 2 ? x : y;

        p = b5 == b3 ? -p : p;
        q = b5 == b4 ? -q : q;
        r = b5 != (b4 ^ b3) ? -r : r;
        t *= t;
        return 8 * t * t * (p + (b1 == 0 ? q + r : b2 == 0 ? q : r));
    }

    int shuffle(int i, int j, int k)
    {
        return b(i, j, k, 0) + b(j, k, i, 1) + b(k, i, j, 2) + b(i, j, k, 3) + b(j, k, i, 4) + b(k, i, j, 5) + b(i, j, k, 6) + b(j, k, i, 7);
    }

    int b(int i, int j, int k, int B)
    {
        return T[b(i, B) << 2 | b(j, B) << 1 | b(k, B)];
    }

    int b(int N, int B)
    {
        return N >> B & 1;
    }

    int fastfloor(float n)
    {
        return n > 0 ? (int)n : (int)n - 1;
    }
}
