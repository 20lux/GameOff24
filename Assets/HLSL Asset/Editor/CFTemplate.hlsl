#ifndef #SCRIPTNAME#_INCLUDED
#define #SCRIPTNAME#_INCLUDED

void #SCRIPTNAME#_float(float3 In, out float3 Out)
{
	// This example function just swaps around RGB values
	float x = In.y;
	float y = In.z;
	float z = In.x;

	Out = float3(x, y, z);
}

#endif
