using UnityEngine;
using System.Collections;

sealed public class ElectricEffect : MonoBehaviour {

	public string type;

	public int lineNum = 4; 
	public int totalLine = 2;

	public LineRenderer[] line;
	
	public static float[] posX;
	public static float[] posY;
	
	public float offsetX = 40.0f;
	public float offsetY = 30.0f;
	
	public float delay = 0.03f;

	public float lineOffsetY = 10.0f;

	public int lineRandomValue = 10;

	public Transform goStartEffect;
	public Transform goEndEffect;



	private static bool _isInit = false;

	void Awake()
	{

#if UNITY_EDITOR
		if(GameManager.me == null && _isInit == false)
		{
			init();
		}
#endif
	}

	void init()
	{
		_isInit = true;
		
		if(posX == null)
		{
			posX = new float[100];
			posY = new float[100];
			
			for(int i = 0; i < 100; ++i)
			{
				posX[i] = UnityEngine.Random.Range(-offsetX, offsetX);
				posY[i] = UnityEngine.Random.Range(-offsetY, offsetY);
			}
		}
	}

	
	public Monster cFrom;
	public Monster cTo;
	
	public Transform shootingPoint = null;
	
	private Vector3 _v;
	
	public Vector3 vFrom;
	public Vector3 vTo;
	
	public Transform tFrom;
	public Transform tTo;	
	
	
	public enum Mode
	{
		CHARACTER, TRANSFORM, POSITION
	}
	
	public Mode mode = Mode.POSITION;
	
	private float _delay = 0.0f;
	
	//public void update()
	void LateUpdate()
	{

#if UNITY_EDITOR

		if(GameManager.me == null) _delay -= Time.smoothDeltaTime;
		else _delay -= GameManager.globalDeltaTime;
#else 
		_delay -= GameManager.globalDeltaTime;
#endif

	
		
		if(_delay > 0) return;
		else _delay += delay;
		
		updateLine();
	}
	
	private Vector3 _vt;
	private void updateLine()
	{
		if(mode == Mode.CHARACTER)
		{
			if(cFrom != null && cTo != null) //&& cFrom.gameObject.activeSelf && cTo.gameObject.activeSelf)// && cFrom.hp > 0 && cTo.hp > 0)
			{
				if(shootingPoint != null)
				{
					vFrom = shootingPoint.position;
				}
				else
				{
					vFrom = cFrom.cTransformPosition;
					vFrom.z -= cFrom.hitObject.depth * 0.2f;		
					vFrom.y += cFrom.bodyYCenter;
				}
				
				vTo = cTo.cTransformPosition;
				vTo.y += cTo.bodyYCenter;
				vTo.z -= cTo.hitObject.depth * 0.2f;

				for(int j = 0; j < totalLine; ++j)
				{
					_vt = vFrom;
					_vt.y -= j * lineOffsetY;
					line[j].SetPosition(0, _vt);
				}

				
				for(int i = 1; i < lineNum; ++i)
				{
					_v = Vector3.Lerp(vFrom,vTo,i*(1.0f/lineNum));

					for(int j = 0; j < totalLine; ++j)
					{
						if(posIndex >= 100) posIndex = 0;

						_vt = _v;

						_vt.x += posX[posIndex];
						_vt.y += posY[posIndex];

						_vt.y -= j * lineOffsetY + UnityEngine.Random.Range(0,lineRandomValue+1) - (lineRandomValue)/2;

						line[j].SetPosition(i, _vt);

						++posIndex;
					}
				}

				for(int j = 0; j < totalLine; ++j)
				{
					_vt = vTo;
					_vt.y -= j *lineOffsetY;
					line[j].SetPosition(lineNum, _vt);
				}
			}			
		}
		else if(mode == Mode.POSITION)
		{
			for(int j = 0; j < totalLine; ++j)
			{
				_vt = vFrom;
				_vt.y -= j *lineOffsetY;
				line[j].SetPosition(0, _vt);
			}

			
			for(int i = 1; i < lineNum; ++i)
			{
				if(posIndex >= 100) posIndex = 0;
				
				_v = Vector3.Lerp(vFrom,vTo,i*(1.0f/lineNum));
				_v.x += posX[posIndex];
				_v.y += posY[posIndex];

				for(int j = 0; j < totalLine; ++j)
				{
					_vt = _v;
					_vt.y -= j * lineOffsetY + UnityEngine.Random.Range(0,lineRandomValue+1) - (lineRandomValue)/2;
					line[j].SetPosition(i, _vt);
				}

				++posIndex;
			}

			for(int j = 0; j < totalLine; ++j)
			{
				_vt = vTo;
				_vt.y -= j *lineOffsetY;
				line[j].SetPosition(lineNum, _vt);			
			}

		}		
		else if(mode == Mode.TRANSFORM)
		{
			vFrom = tFrom.position;				
			vTo = tTo.position;

			for(int j = 0; j < totalLine; ++j)
			{
				_vt = vFrom;
				_vt.y -= j *lineOffsetY;
				line[j].SetPosition(0, _vt);
			}

			
			for(int i = 1; i < lineNum; ++i)
			{
				if(posIndex >= 100) posIndex = 0;
				
				_v = Vector3.Lerp(vFrom,vTo,i*(1.0f/lineNum));
				_v.x += posX[posIndex];
				_v.y += posY[posIndex];

				for(int j = 0; j < totalLine; ++j)
				{
					_vt = _v;
					_vt.y -= j * lineOffsetY + UnityEngine.Random.Range(0,lineRandomValue+1) - (lineRandomValue)/2;
					line[j].SetPosition(i, _vt);
				}

				++posIndex;
			}
				
			for(int j = 0; j < totalLine; ++j)
			{
				_vt = vTo;
				_vt.y -= j *lineOffsetY;
				line[j].SetPosition(lineNum, _vt);			
			}
		}

		if(goStartEffect != null) goStartEffect.position = vFrom;
		if(goEndEffect != null) goEndEffect.position = vTo;
	}
	
	
	
	int posIndex = 0;
	

	public bool isEnabled
	{
		set
		{
			if(value)
			{
				if(_isInit == false) init();
				updateLine();
				_delay = delay;
			}
			else
			{
				cFrom = null;
				cTo = null;
				shootingPoint= null;
			}
			
			gameObject.SetActive(value);
		}
	}	
	
}


/*
var targetObject : GameObject;
private var lineRenderer : LineRenderer;

function Start() {
	lineRenderer = GetComponent(LineRenderer);
}

function Update () {
	lineRenderer.SetPosition(0,this.transform.localPosition);
	
	for(var i:int=1;i<4;i++)
	{
		var pos = Vector3.Lerp(this.transform.localPosition,targetObject.transform.localPosition,i/4.0f);
		
		pos.x += Random.Range(-0.4f,0.4f);
		pos.y += Random.Range(-0.4f,0.4f);
		
		lineRenderer.SetPosition(i,pos);
	}
	
	lineRenderer.SetPosition(4,targetObject.transform.localPosition);
}
*/
