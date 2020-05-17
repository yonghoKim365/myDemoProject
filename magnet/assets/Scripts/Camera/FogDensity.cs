using UnityEngine;
using System.Collections;

public class FogDensity : MonoBehaviour {

		public bool fog;
		public Color fogColor;
		public float fogDensity;
		public float fogStart;
		public float fogEnd;

		public Color ambientLight;
		public float haloStrength;
		public float flareStrength;
		
		bool previousFog;
		Color previousFogColor;
		float previousFogDensity;
		float previousFogStart;
		float previousFogEnd;
		Color previousAmbientLight;
		float previousHaloStrength;
		float previousFlareStrength;

		
		void OnPreRender()
		{
			previousFog = RenderSettings.fog;
			previousFogColor = RenderSettings.fogColor;
			previousFogDensity = RenderSettings.fogDensity;
			previousFogStart = RenderSettings.fogStartDistance;
			previousFogEnd = RenderSettings.fogEndDistance;
			previousAmbientLight = RenderSettings.ambientLight;
			previousHaloStrength = RenderSettings.haloStrength;
			previousFlareStrength = RenderSettings.flareStrength;
			
			if(fog)
			{
				RenderSettings.fog = fog;
				RenderSettings.fogColor = fogColor;
				RenderSettings.fogDensity = fogDensity;
				RenderSettings.fogStartDistance = fogStart;
				RenderSettings.fogEndDistance = fogEnd;
				RenderSettings.ambientLight = ambientLight;
				RenderSettings.haloStrength = haloStrength;
				RenderSettings.flareStrength = flareStrength;
			}
		}
		
		void OnPostRender()
		{
			RenderSettings.fog = previousFog;
			RenderSettings.fogColor = previousFogColor;
			RenderSettings.fogDensity = previousFogDensity;
			RenderSettings.fogStartDistance = previousFogStart;
			RenderSettings.fogEndDistance = previousFogEnd;
			RenderSettings.ambientLight = previousAmbientLight;
			RenderSettings.haloStrength = previousHaloStrength;
			RenderSettings.flareStrength = previousFlareStrength;

		}
	}
