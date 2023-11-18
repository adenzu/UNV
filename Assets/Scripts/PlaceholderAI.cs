using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlaceholderAI
{

    public static Outputs Predict(Inputs inputs)
    {
        float distance = (inputs.target - inputs.position).magnitude;
        float angle = Vector3.Angle(inputs.right, inputs.target - inputs.position);

        Outputs outputs = new Outputs
        {
            thrust = distance < inputs.targetDeadZone ? 0 : 1,
            steer = (90f - angle) / 90f
        };

        return outputs;
    }

    public struct Inputs
    {
        public float thrust;
        public float steer;
        public Vector3 right;
        public Vector3 position;
        public Vector3 target;
        public float targetDeadZone;
    }

    public struct Outputs
    {
        public float thrust;
        public float steer;
    }
}
