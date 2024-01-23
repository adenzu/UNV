using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField] private bool _setHere = false;
    [SerializeField] private BoatController _boat;

    private void Update()
    {
        if (_setHere)
        {
            _boat.SetDestination(transform.position);
            _setHere = false;
        }
    }
}
