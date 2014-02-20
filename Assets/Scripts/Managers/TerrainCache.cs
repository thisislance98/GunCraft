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

	public bool IsSavingWorld;

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

		IsSavingWorld = true;
		
		Debug.Log("saving started");
		List<ParseObject> objs = new List<ParseObject>();
		
		List<string> coordHashList = new List<string>();
		
		foreach(IntCoords coord in m_modifiedChunks)
		{
			string hash = "x" + coord.X.ToString() + "y" + coord.Y.ToString() + "z" + coord.Z.ToString();
			coordHashList.Add(hash);
		}
		
		var query = ParseObject.GetQuery("World").WhereContainedIn("LocationHash",coordHashList);

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

			// now add chunks to the database that are new
			foreach(IntCoords coord in m_modifiedChunks)
			{
				int[,,] chunk = m_data[coord];
				string str = ChunkToString(chunk);
				
				ParseObject parseObj = new ParseObject("World");
				string hash = "x" + coord.X.ToString() + "y" + coord.Y.ToString() + "z" + coord.Z.ToString();
				parseObj["x"] = coord.X;
				parseObj["y"] = coord.Y;
				parseObj["z"] = coord.Z;
				parseObj["LocationHash"] = hash;
				parseObj["chunk"] = str;
				
				objs.Add(parseObj);
				m_modifiedChunks.Remove(coord);
			}
		}).ContinueWith(t => // now save all the parse objects
		                {
		
			Task task = ParseObject.SaveAllAsync(objs);
			task.ContinueWith(q => 
			                  {
				Debug.Log("saving complete");
				IsSavingWorld = false;
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

	IntCoords GetChunkCoordsFromWorldPos(int x, int y, int z)
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
		
		return new IntCoords(ciX, ciY, ciZ);
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
			getDensity(x,y,z);
//			Debug.Log("Couldn't find coords: " + coords.ToString());
//			return;
		}
		
		//Debug.Log("Setting chunk " + coords.ToString() + ", index (" + modX.ToString() + ", " + modY.ToString() + ", " + modZ.ToString() + ") to nil.");
		m_data[coords][modX, modY, modZ] = density;
	}
	
}