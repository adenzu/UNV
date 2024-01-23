using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoatController))]
public class ForwardTargeter : MonoBehaviour
{
    [SerializeField] private float _distance = 100f;

    private void Start()
    {
        BoatController boatController = GetComponent<BoatController>();
        boatController.SetDestination(transform.position + transform.forward * _distance);
    }
}
