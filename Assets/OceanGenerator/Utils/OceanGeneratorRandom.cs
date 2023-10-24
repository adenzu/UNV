using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanGeneratorRandom
{

	private System.Random random;

	public OceanGeneratorRandom(int seed)
	{
		random = new System.Random(seed);
	}

	public float Value()
	{
		return (float)random.NextDouble();
	}

	public float Range(float min, float max)
	{
		return ((max - min) * Value()) + min;
	}

	public float Value(float max)
	{
		return Value() * max;
	}

	public bool Bool()
	{
		return Value() > 0.5f;
	}
}
