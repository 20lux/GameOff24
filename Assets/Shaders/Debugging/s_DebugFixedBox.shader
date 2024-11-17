Shader "Custom/Wireframe/s_DebugFixedBox"
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
            Name "Fixed Wireframe Box"
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
                // Define the box's vertices in fixed world space
                float3 vertices[8] = {
                    float3(-0.5, -0.5, -0.5),  // Vertex 0
                    float3(-0.5, -0.5,  0.5),  // Vertex 1
                    float3( 0.5, -0.5,  0.5),  // Vertex 2
                    float3( 0.5, -0.5, -0.5),  // Vertex 3
                    float3(-0.5,  0.5, -0.5),  // Vertex 4
                    float3(-0.5,  0.5,  0.5),  // Vertex 5
                    float3( 0.5,  0.5,  0.5),  // Vertex 6
                    float3( 0.5,  0.5, -0.5)   // Vertex 7
                };

                // Define only the bottom and vertical edges as pairs of vertex indices
                int edges[24] = {
                    0, 1,   // bottom line 1
                    1, 2,   // bottom line 2
                    2, 3,   // bottom line 3
                    3, 0,   // bottom line 4

                    0, 4,   // side line 1
                    1, 5,   // side line 2
                    2, 6,   // side line 3
                    3, 7,    // side line 4

                    4, 5,   // side line 1
                    5, 6,   // side line 2
                    6, 7,   // side line 3
                    7, 4    // side line 4
                };

                // Emit each edge as a separate line
                for (int e = 0; e < 24; e += 2)
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
