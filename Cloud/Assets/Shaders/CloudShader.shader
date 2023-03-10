Shader "Custom/CloudShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Density ("Density", Range(0.1,1)) = 0.5

    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        
        Zwrite Off
        Cull Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define MAX_STEPS 100
            #define MAX_DIST 100
            #define SURF_DIST 1e-3
            #define STEP_DIST 1e-1
            
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

            float3 boundsMin = float3(-0.5,-0.5,-0.5);
            float3 boundsMax = float3(0.5,0.5,0.5);
            float _Density;
            float lightAbsorptionTowardSun = 1;

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

            float beersLaw(float d) {
                float beer = exp(-d);
                return beer;
            }

            float2 cRaycRay(){
                
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

            float lightMarch(float3 marchPoint){

                //return 3;
                float3 dirToLight = _WorldSpaceLightPos0.xyz;
                float vvvvvv = normalize(marchPoint-dirToLight);

                // get distance to exit
                float dstInsideBox = rayBoxDst(boundsMin, boundsMax, marchPoint, 1/vvvvvv).y;
                //float totalDistance = dstInsideBox.y - dstInsideBox.x;

                float transmittance = 1;
                float totalDensity = 0;

                 //initial distance?
                marchPoint += dirToLight * STEP_DIST * .5;

                float numSteps = dstInsideBox / STEP_DIST;

                for(int step = 0; step < numSteps; step ++) {

                    totalDensity += _Density;
                    //float density = Random.Range(0.0f, 1.0f);
                    //totalDensity += max(0, density * stepSize);
                    marchPoint += dirToLight * STEP_DIST;
                }
                

                transmittance = beersLaw(totalDensity*lightAbsorptionTowardSun);

                //float clampedTransmittance = darknessThreshold + transmittance * (1-darknessThreshold);
                return transmittance;

            }

            float4 boxMarch(float3 entryPoint, float3 rayDir, float entryDistance, float exitDistance){

                // Find the maximum distance to travel
                float maxDistance = entryDistance - exitDistance;
                
                float transmittance = 1;
                float lightTransmittance = 0;
                float density = _Density;
                float3 lightEnergy = 0;
                float stepSize = 0.5;
                float dstTravelled = 0;

                /*
                for(int i = 0; i < 5; i ++){
                    transmittance *= exp(-density * stepSize);
                }

                return float4(lightEnergy, transmittance);
                */

                // Here we march through the box with a set step size
                for(int travelDistance = 0; travelDistance < maxDistance; travelDistance += stepSize){

                    
                    float3 rayPosition = entryPoint + rayDir * travelDistance;
                    
                    // Calculate the light transmittance at the specified point


                    if (density > 0) {

                        lightTransmittance = lightMarch(rayPosition);
                        lightEnergy += density * stepSize * transmittance * lightTransmittance;
                        transmittance *= exp(-density * stepSize);
                    
                        if (transmittance < 0.01) {
                            break;
                        }
                    }

                }
                
                return float4(lightEnergy, transmittance);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv-0.5;
                float3 rayorigin = i.ro;
                float3 raydirection = (normalize(i.hitPos-rayorigin));

                // Find the distances to the entry and exit of the box
                float2 rayHitAndExitDist = rayBoxDst(boundsMin,boundsMax,rayorigin,1/raydirection);
                float rayHitDist = rayHitAndExitDist.x;
                float rayExitDist = rayHitAndExitDist.y;

                // Point of entry
                float3 rayhit = rayorigin + raydirection * rayHitDist;

                // Point of exit
                float3 rayExit = rayorigin + raydirection * rayExitDist;

                float4 lightAndTransmittance = boxMarch(rayhit,raydirection,rayHitDist,rayExitDist);

                fixed4 col = float4(lightAndTransmittance.x,lightAndTransmittance.y,lightAndTransmittance.z,1);
                //if(lightAndTransmittance.w>0) col = 1;
                //else 
                float a;
                if(lightAndTransmittance.w = 1){
                    a = 1;
                }else a = 0;
                //float cloud = 0.1 - lightAndTransmittance.w;
                
            
                return float4(lightAndTransmittance.x,lightAndTransmittance.y,lightAndTransmittance.z,a);
            }
            ENDCG
        }
    }
}