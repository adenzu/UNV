using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    [SerializeField] private Transform _followTarget;
    [SerializeField] private Transform _lookTarget;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _followTarget.position, 0.1f);
        Vector3 difference = _lookTarget.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(difference);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.1f);
    }
}
