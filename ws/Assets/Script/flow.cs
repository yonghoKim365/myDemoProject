using UnityEngine;

using System.Collections;




public class flow : MonoBehaviour {

private float scrollSpeed = 0.25F;

 

 // Update is called once per frame

 void Update () { 

  float offset = Time.time * scrollSpeed;

  renderer.material.SetTextureOffset ("_MainTex",new Vector2(0, offset));

 }

}



