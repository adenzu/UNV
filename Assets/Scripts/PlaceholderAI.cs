using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlaceholderAI
{

    public static Outputs Predict(Inputs inputs)
    {
        float distance = (inputs.target - inputs.position).magnitude;
        float outputFactor = distance / inputs.targetDeadZone;

        Outputs outputs = new Outputs
        {
            thrustJerk = distance < inputs.targetDeadZone ? (inputs.thrust * inputs.thrust > 0.01f ? -Mathf.Sign(inputs.thrust) : 0) : 1,
            steerJerk = (Vector3.Angle(inputs.right, inputs.target - inputs.position) > 90 ? -1 : 1) * Mathf.Clamp01(outputFactor)
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
        public float thrustJerk;
        public float steerJerk;
    }
}
