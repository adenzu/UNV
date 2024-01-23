using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDistanceTest : MonoBehaviour
{
    [SerializeField] private Transform _from1;
    [SerializeField] private Transform _to1;
    [SerializeField] private Transform _from2;
    [SerializeField] private Transform _to2;

    private void Update()
    {
        Debug.Log(Util.GetLinesDistance(_from1.position.XZ(), _to1.position.XZ(), _from2.position.XZ(), _to2.position.XZ()));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_from1.position, _to1.position);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_from2.position, _to2.position);
    }
}
