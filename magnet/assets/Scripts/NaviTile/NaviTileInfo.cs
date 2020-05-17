using UnityEngine;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

public class NaviTileInfo : MonoSingleton<NaviTileInfo>
{
    private bool[] m_OXMaskBuf = null; //<--- 서버에서 사용할 OX 마스크 버퍼 //1 이면 갈 수 있는 곳 //0 이면 갈 수 없는 곳
    private int X_Count;
    private int Z_Count;
    private float TileSize;
    private Vector3 MinPos;
    private Vector3 MaxPos;
    private TileData[,] TileList;

    public Vector3 GetMinPos()
    {
        return MinPos;
    }

    public float GetTileSize()
    {
        return TileSize;
    }

    struct TileData
    {
        public TileData(float x, float z, bool isMove)
        {
            X = x;
            Z = z;
            IsMove = isMove;
        }

        public bool IsMove;
        public float X;
        public float Z;
    }

    public void LoadTile(string sceneName)
    {
        //string a_temfname = Application.dataPath;
        //a_temfname = a_temfname.Replace('\\', '/');
        //string a_RootPath = a_temfname.Remove(a_temfname.LastIndexOf("/") + 1); //프로젝트가 있는 경로...

        string a_SceneName = System.IO.Path.GetFileNameWithoutExtension(sceneName);
        string a_PathName = "MapMask" + "/" + a_SceneName;

        TextAsset data = Resources.Load(a_PathName, typeof(TextAsset)) as TextAsset;
		LoadTilePart2 (data);
    }

	public IEnumerator LoadTitleAsync(string sceneName){
		string a_SceneName = System.IO.Path.GetFileNameWithoutExtension(sceneName);
		string a_PathName = "MapMask" + "/" + a_SceneName;

		//TextAsset data = Resources.Load(a_PathName, typeof(TextAsset)) as TextAsset;
		ResourceRequest resReq = Resources.LoadAsync(a_PathName, typeof(TextAsset));
		
		while (!resReq.isDone) { 
			yield return null; 
		}
		
		BinaryFormatter bf = new BinaryFormatter();
		TextAsset data = resReq.asset as TextAsset;

		LoadTilePart2 (data);

		yield return null;
	}


	void LoadTilePart2(TextAsset data)
	{
		StringReader sr = new StringReader(data.text);
		string a_strBuf = sr.ReadToEnd();
		
		//if (System.IO.File.Exists(a_PathName) == true)
		{
			//string a_strBuf = System.IO.File.ReadAllText(a_PathName);
			
			JSONObject a_ParseJs = new JSONObject(a_strBuf);
			float aTileSize = float.Parse(a_ParseJs["TileSize"].ToString());
			int aX_Count = int.Parse(a_ParseJs["X_Count"].ToString());
			int aZ_Count = int.Parse(a_ParseJs["Z_Count"].ToString());
			int aTotalCount = int.Parse(a_ParseJs["Total_Count"].ToString());
			float a_MinPosX = float.Parse(a_ParseJs["StartMin_X"].ToString());
			float a_MinPosZ = float.Parse(a_ParseJs["StartMin_Z"].ToString());
			float a_MaxPosX = float.Parse(a_ParseJs["EndMax_X"].ToString());
			float a_MaxPosZ = float.Parse(a_ParseJs["EndMax_Z"].ToString());
			
			X_Count = aX_Count;
			Z_Count = aZ_Count;
			TileSize = aTileSize;
			MinPos = new Vector3(a_MinPosX, 0, a_MinPosZ);
			MaxPos = new Vector3(a_MaxPosX, 0, a_MaxPosZ);
			
			string a_OXMask = a_strBuf.Substring(a_strBuf.IndexOf("[") + 1, a_strBuf.LastIndexOf("]") - a_strBuf.IndexOf("[") - 1);
			string[] a_strSplit = a_OXMask.Split(',');
			if (aTotalCount == a_strSplit.Length && a_strSplit.Length == (aX_Count * aZ_Count)) //타일 숫자가 맞는지? 검증
			{
				m_OXMaskBuf = new bool[aTotalCount];
				TileList = new TileData[Z_Count, X_Count];
				
				for (int a_ZZ = 0; a_ZZ < aZ_Count; a_ZZ++)
				{
					for (int a_XX = 0; a_XX < aX_Count; a_XX++)
					{
						Vector3 cacTlVec = new Vector3(a_MinPosX + (a_XX * aTileSize + (aTileSize / 2.0f)), 0.0f, a_MinPosZ + (a_ZZ * aTileSize + (aTileSize / 2.0f)));
						
						string cacPas = a_strSplit[(a_ZZ * aX_Count) + a_XX];
						if (cacPas == "0")   //1 이면 갈 수 있는 곳 //0 이면 갈 수 없는 곳
						{
							m_OXMaskBuf[(a_ZZ * aX_Count) + a_XX] = false;
							TileList[a_ZZ, a_XX] = new TileData(cacTlVec.x, cacTlVec.z, false);
							continue;
						}
						else
						{
							m_OXMaskBuf[(a_ZZ * aX_Count) + a_XX] = true;
							TileList[a_ZZ, a_XX] = new TileData(cacTlVec.x, cacTlVec.z, true);
						}
					}
				}
			}
		}
	}

    public Vector3 GetMoveableTilePos(int x, int z)
    {
        if (x >= 0 && x <= X_Count)
        {
            if (z >= 0 && z <= Z_Count)
            {
                TileData data = TileList[z, x];

                if (data.IsMove)
                    return new Vector3(data.X, 0f, data.Z);
                else
                    return Vector3.zero;
            }
        }
        return Vector3.zero;
    }

    public Vector3 GetTilePos(int x, int z)
    {
        if (x >= 0 && x <= X_Count)
        {
            if (z >= 0 && z <= Z_Count)
            {
                TileData data = TileList[z, x];
                return new Vector3(data.X, 0f, data.Z);
            }
        }
        return Vector3.zero;
    }

    public bool GetMoveablePos(int x, int z)
    {
        if (x >= 0 && x <= X_Count)
        {
            if (z >= 0 && z <= Z_Count)
            {
                TileData data = TileList[z, x];

                if (data.IsMove)
                    return true;
            }
        }
        return false;
    }

    public bool DirectionToTile(int inX, int inZ, Vector3 direction, out int outX, out int outZ, out Vector3 realPostion)
    {
        int x = inX, z = inZ;

        if (0.3f < direction.x)//우측
        {
            x += 1;
        }
        else if (direction.x < -0.3f)//좌측
        {
            x -= 1;
        }

        if (0.3f < direction.z)//상단
        {
            z += 1;
        }
        else if (direction.z < -0.3f)//하단
        {
            z -= 1;
        }

        TileData data = TileList[z, x];
        if (data.IsMove)
        {
            //이동 가능
            outX = x;
            outZ = z;

            realPostion = new Vector3(data.X, 0, data.Z);

            return true;
        }

        //이동 불가능
        outX = inX;
        outZ = inZ;

        realPostion = Vector3.zero;

        return false;
    }
}
