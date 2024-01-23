using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColregTestSetup : MonoBehaviour
{
    [SerializeField] private Transform[] _boats;
    [SerializeField, Min(0f)] private float _distance;
    [SerializeField, Range(0f, 360f)] private float _angle;
    [SerializeField, Range(0f, 360f)] private float _angleOffset;

    private int _arrivedCount;

    private void Start()
    {
        for (int i = 0; i < _boats.Length; i++)
        {
            BoatController boatController = _boats[i].GetComponent<BoatController>();
            boatController.OnArrivalToDestinationHook += AddOne;
        }
        Invoke(nameof(Test), 1f);
    }

    private void Test()
    {
        _arrivedCount = 0;
        for (int i = 0; i < _boats.Length; i++)
        {
            Vector3 target = transform.position + Quaternion.Euler(0f, _angle * i + _angleOffset + 180f, 0f) * Vector3.forward * _distance;
            BoatController boatController = _boats[i].GetComponent<BoatController>();
            boatController.SetDestination(target);
        }
    }

    private void Setup()
    {
        for (int i = 0; i < _boats.Length; i++)
        {
            _boats[i].position = transform.position + Quaternion.Euler(0f, _angle * i + _angleOffset, 0f) * Vector3.forward * _distance;
            _boats[i].rotation = Quaternion.Euler(0f, _angle * i + _angleOffset + 180f, 0f);
        }
    }

    private void AddOne()
    {
        _arrivedCount++;
        if (_arrivedCount == _boats.Length)
        {
            // Setup();
            // Invoke(nameof(Test), 1f);
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            return;
        }
        Setup();
    }
}
