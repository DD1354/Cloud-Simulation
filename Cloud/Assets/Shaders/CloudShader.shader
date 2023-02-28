Shader "Custom/CloudShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define MAX_STEPS 100
            #define MAX_DIST 100
            #define SURF_DIST 1e-3
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 ro: TEXCOORD1;
                float3 hitPos: TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float3 boundsMin = float3(0.5,0.5,0.5);
            float3 boundsMax = float3(-0.5,-0.5,-0.5);

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.ro = _WorldSpaceCameraPos;
                o.hitPos = v.vertex;
                return o;
            }

            float GetDist (float3 p){
                float distance = length(p) - 0.025;
                return distance;
            }

            /*
            float Raymarch(float3 rayorigin, float3 raydirection){
                
                float distanceFromOrigin = 0;
                float distanceFromSurface;

                bool hasHit = false;

                for(int i = 0; i < MAX_STEPS; i++)
                {
                    float3 position = rayorigin + distanceFromOrigin * raydirection;
                    distanceFromSurface = GetDist(position);
                    distanceFromOrigin += distanceFromSurface;
                    
                    if(distanceFromSurface<SURF_DIST||distanceFromOrigin>MAX_DIST) 
                    {
                        hasHit=true;
                        break;
                    }
                }
                
                if(hasHit)
                {
                    // just 100 steps, we will want to set more specific values
                    for(int i = 0; i < 100; i++){

                    }
                    //Shit on your lawn
                    // BEEEER
                }

                return distanceFromOrigin;
            }
            */

            float beersLaw(float d){
                // insert beers law
            }

            float lightMarch(float3 marchPoint){
                // Here we march from the light source to the marchPoint of the box
                // for each step, transmittance of light is calculated
            }

            float boxMarch(float3 entryPoint, float rayDir, float maxDistance){

                // Here we march through the box
                    //for each step, call lightmarch

            }

            // Returns (dstToBox, dstInsideBox). If ray misses box, dstInsideBox will be zero
            float2 rayBoxDst(float3 boundsMin, float3 boundsMax, float3 rayOrigin, float3 invRaydir) {
                // Adapted from: http://jcgt.org/published/0007/03/04/

                float3 t0 = (boundsMin - rayOrigin) * invRaydir;
                float3 t1 = (boundsMax - rayOrigin) * invRaydir;
                float3 tmin = min(t0, t1);
                float3 tmax = max(t0, t1);
                
                float dstA = max(max(tmin.x, tmin.y), tmin.z);
                float dstB = min(tmax.x, min(tmax.y, tmax.z));

                // CASE 1: ray intersects box from outside (0 <= dstA <= dstB)
                // dstA is dst to nearest intersection, dstB dst to far intersection

                // CASE 2: ray intersects box from inside (dstA < 0 < dstB)
                // dstA is the dst to intersection behind the ray, dstB is dst to forward intersection

                // CASE 3: ray misses box (dstA > dstB)

                float dstToBox = max(0, dstA);
                float dstInsideBox = max(0, dstB - dstToBox);
                return float2(dstToBox, dstInsideBox);
            }

            fixed4 frag (v2f i) : SV_Target
            {

                float2 uv = i.uv-0.5;
                float3 rayorigin = i.ro;
                float3 raydirection = normalize(i.hitPos-rayorigin);

                // Find the distances to the entry and exit of the box
                float2 rayHitAndExitDist = rayBoxDst(boundsMin,boundsMax,rayorigin,raydirection);
                float rayHitDist = rayHitAndExitDist.x;
                float rayExitDist = rayHitAndExitDist.y;

                // Point of entry
                float3 rayhit = rayorigin + raydirection * rayHitDist;

                // Point of exit
                float3 rayExit = rayorigin + raydirection * rayExitDist;

                fixed4 col = 0;
                
                if(rayHitDist<MAX_DIST)
                {
                    col.r = 1;
                }
            
                return col;
            }
            ENDCG
        }
    }
}