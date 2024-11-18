Shader "Custom/Wireframe/s_DebugFixedArrow"
{
    Properties
    {
        _WireColor ("Wire Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "Fixed Wireframe Arrow"
            Cull Off
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma require geometry
            #pragma geometry geom
            #pragma fragment frag
            #pragma target 4.5

            #include "UnityCG.cginc"

            float4 _WireColor;

            struct v2g
            {
                float4 pos : SV_POSITION;
            };

            struct g2f
            {
                float4 pos : SV_POSITION;
            };

            v2g vert (appdata_base v)
            {
                v2g o;
                // Position is irrelevant; the geometry shader will handle the positions
                o.pos = float4(0, 0, 0, 1);
                return o;
            }

            [maxvertexcount(24)]
            void geom(point v2g points[1], inout LineStream<g2f> lineStream)
            {
                // Define the arrow's vertices in fixed world space
                float3 vertices[7] = {

                    // Arrow Shaft
                    float3(0, 0, 0),  // Vertex 0
                    float3(0, 1, 0),  // Vertex 1

                    // Arrow Base
                    float3(-.25, 1, -.25),  // Vertex 2
                    float3(-.25, 1, 0.25),  // Vertex 3
                    float3(0.25, 1, 0.25),  // Vertex 4
                    float3(0.25, 1, -.25),  // Vertex 5

                    // Arrow Tip (Apex)
                    float3(0, 1.5, 0),  // Vertex 6 Apex
                };

                // Define only the bottom and vertical edges as pairs of vertex indices
                int edges[18] = {
                    0, 1,   // arrow shaft line

                    2, 3,   // arrow base line 1
                    3, 4,   // arrow base line 2
                    4, 5,   // arrow base line 3
                    5, 2,   // arrow base line 4

                    2, 6,   // arrow tip line 1
                    3, 6,   // arrow tip line 2
                    4, 6,   // arrow tip line 3
                    5, 6,   // arrow tip line 4
                };

                // Emit each edge as a separate line
                for (int e = 0; e < 18; e += 2)
                {
                    g2f o;

                    // Transform the first vertex of the edge
                    float4 worldPos = float4(vertices[edges[e]], 1.0);
                    o.pos = UnityWorldToClipPos(worldPos.xyz);
                    lineStream.Append(o);

                    // Transform the second vertex of the edge
                    worldPos = float4(vertices[edges[e + 1]], 1.0);
                    o.pos = UnityWorldToClipPos(worldPos.xyz);
                    lineStream.Append(o);

                    // End each line segment explicitly to prevent unintended connections
                    lineStream.RestartStrip();
                }
            }

            float4 frag (g2f i) : SV_Target
            {
                return _WireColor;
            }
            ENDHLSL
        }
    }
    FallBack Off
}
