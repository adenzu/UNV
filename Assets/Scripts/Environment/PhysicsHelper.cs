using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Source:
https://github.com/ditzel/UnityOceanWavesAndShip
*/

public static class PhysicsHelper
{
    public static void ApplyForceToReachVelocity(Rigidbody rigidbody, Vector3 velocity, float force = 1, ForceMode mode = ForceMode.Force)
    {
        if (force == 0 || velocity.magnitude == 0)
        {
            return;
        }

        velocity += velocity.normalized * 0.2f * rigidbody.drag;

        //force = 1 => need 1 s to reach velocity (if mass is 1) => force can be max 1 / Time.fixedDeltaTime
        force = Mathf.Clamp(force, -rigidbody.mass / Time.fixedDeltaTime, rigidbody.mass / Time.fixedDeltaTime);

        //dot product is a projection from rhs to lhs with a length of result / lhs.magnitude https://www.youtube.com/watch?v=h0NJK4mEIJU
        if (rigidbody.velocity.magnitude == 0)
        {
            rigidbody.AddForce(velocity * force, mode);
        }
        else
        {
            var velocityProjectedToTarget = velocity.normalized * Vector3.Dot(velocity, rigidbody.velocity) / velocity.magnitude;
            rigidbody.AddForce((velocity - velocityProjectedToTarget) * force, mode);
        }
    }

    public static void ApplyAccelerationToReachAngularSpeed(Rigidbody rigidbody, float angularSpeed, float acceleration = 1, ForceMode mode = ForceMode.Acceleration)
    {
        if (acceleration == 0 || angularSpeed == 0)
        {
            return;
        }

        angularSpeed += Mathf.Sign(angularSpeed) * 0.2f * rigidbody.angularDrag;

        //acceleration = 1 => need 1 s to reach angularVelocity (if mass is 1) => acceleration can be max 1 / Time.fixedDeltaTime
        acceleration = Mathf.Clamp(acceleration, -1 / Time.fixedDeltaTime, 1 / Time.fixedDeltaTime);

        //dot product is a projection from rhs to lhs with a length of result / lhs.magnitude https://www.youtube.com/watch?v=h0NJK4mEIJU
        if (rigidbody.angularVelocity.magnitude == 0)
        {
            rigidbody.AddTorque(0, acceleration, 0, mode);
        }
        else
        {
            float angularSpeedDifference = angularSpeed - rigidbody.angularVelocity.y;
            rigidbody.AddTorque(0, angularSpeedDifference * acceleration, 0, mode);
        }
    }
}
