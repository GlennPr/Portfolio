float4 hash4( float4 n ) { return frac(sin(n)*1399763.5453123); }

	#define HASHSCALE3 float3(443.897, 441.423, 437.195)
	#define HASHSCALE1 443.8975
	
	//  1 out, 1 in...
	float hash11(float p)
	{
		float3 p3  = frac(float3(p,p,p) * HASHSCALE1);
		p3 += dot(p3, p3.yzx + 19.19);
		return frac((p3.x + p3.y) * p3.z);
	}

	
	float hash21(float2 p2)
	{
		float3 p3  = frac(float3(p2.xyx) * HASHSCALE1);
		p3 += dot(p3, p3.yzx + 19.19);
		return frac((p3.x + p3.y) * p3.z);
	}
	
	float2 hash12(float p)
	{
		float3 p3 = frac(float3(p,p,p) * HASHSCALE3);
		p3 += dot(p3, p3.yzx + 19.19);
		return frac((p3.xx+p3.yz)*p3.zy);
	}

	float3 hash13(float p)
	{
	   float3 p3 = frac(float3(p,p,p) * HASHSCALE3);
	   p3 += dot(p3, p3.yzx+19.19);
	   return frac((p3.xxy+p3.yzz)*p3.zyx); 
	}

	float2 hash22Alt(float2 p)
	{
		float3 p3 = frac(float3(p.xyx) * float3(437.195, 443.897, 441.423));
		p3 += dot(p3, p3.yzx+19.19);
		return frac((p3.xx+p3.yz)*p3.zy);
	}

	float2 hash22(float2 p)
	{
		float3 p3 = frac(float3(p.xyx) * HASHSCALE3);
		p3 += dot(p3, p3.yzx+19.19);
		return frac((p3.xx+p3.yz)*p3.zy);
	}

	float3 hash23(float2 p)
	{
		float3 p3 = frac(float3(p.xyx) * HASHSCALE3);
		p3 += dot(p3, p3.yxz+19.19);
		return frac((p3.xxy+p3.yzz)*p3.zyx);
	}

	float2 hash32(float3 p3)
	{
		p3 = frac(p3 * HASHSCALE3);
		p3 += dot(p3, p3.yzx+19.19);
		return frac((p3.xx+p3.yz)*p3.zy);
	}

	float hash31(float3 p3)
	{
		p3  = frac(p3 * HASHSCALE1);
		p3 += dot(p3, p3.yzx + 19.19);
		return frac((p3.x + p3.y) * p3.z);
	}

	#define HASHSCALE4 float4(443.897, 441.423, 437.195, 444.129)
	float4 hash34(int3 p)
	{
		float4 p4 = frac(p.xyzx  * HASHSCALE4);
		p4 += dot(p4, p4.wzxy+19.19);
		return frac((p4.xxyz+p4.yzzw)*p4.zywx);
	}

	#define R2Ratio float2 (0.7548776662466927600500267982588, 0.56984029099805326591218186327522)
	float2 R2(float n, float seed)
	{
		return frac(seed + R2Ratio * n);
	}

	// PRNG
	float RandomU(in uint id, in float u)
	{
		u = u * 0.0018372;
		float v = id * 0.0013345;
		float f = dot(float2(12.9898, 78.233), float2(u, v));
		return frac(43758.5453 * sin(f));
	}

	float Random(float u, float v)
	{
		float f = dot(float2(12.9898, 78.233), float2(u, v));
		return frac(43758.5453 * sin(f));
	}

	// Nearly uniformly distributed random vector in the unit sphere.
	float3 RandomPoint(in float id)
	{
		float u = Random(id * 0.01334, 0.3728) * 3.14159 * 2;
		float z = Random(0.8372, id * 0.01197) * 2 - 1;
		float l = Random(4.438, id * 0.01938 - 4.378);
		return float3(float2(cos(u), sin(u)) * sqrt(1 - z * z), z) * sqrt(l);
	}

	float3 hashI13( uint n ) 
	{
		// integer hash copied from Hugo Elias
		n = (n << 13U) ^ n;
		n = n * (n * n * 15731U + 789221U) + 1376312589U;
		uint3 k = n * uint3(n,n*16807U,n*48271U);
		return float3( k & uint3(0x7fffffffU, 0x7fffffffU,0x7fffffffU))/float(0x7fffffff);
	}

