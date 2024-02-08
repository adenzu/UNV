using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSquareDirectionTest : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + Util.UnitSquare(transform.forward.XZ()).XZ(), 1f);
    }
}
