using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

[Serializable]
public class Spark
{
    public SparkModel.SparkType type;
    public float life;
    public float scale;
    public int points;
    public Spark(SparkModel.SparkType type, int points, float life, float scale)
    {
        this.type = type;
        this.points = points;
        this.life = life;
        this.scale = scale;
    }
}
