using UnityEngine;

using System.Collections;




public class flow01 : MonoBehaviour {

private float scrollSpeed = 0.15F;

 

 // Update is called once per frame

 void Update () { 

  float offset = Time.time * scrollSpeed;

  renderer.material.SetTextureOffset ("_MainTex",new Vector2(0, offset));

 }

}



