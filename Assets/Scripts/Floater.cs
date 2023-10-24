using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Floater : MonoBehaviour
{
    /*
    Source:
    https://youtu.be/v7ag-NeSMSQ
    */
    [SerializeField] private float _depthBeforeSubmersion;
    [SerializeField] private float _displacementAmount;
    [SerializeField] private int _floaters;
    [SerializeField] private float _waterDrag;
    [SerializeField] private float _waterAngularDrag;
    [SerializeField] private WaterSurface _waterSurface;
    [SerializeField] private Rigidbody _rigidbody;
    private WaterSearchParameters searchParameters;
    private WaterSearchResult searchResult;

    private void FixedUpdate()
    {
        _rigidbody.AddForceAtPosition(Physics.gravity / _floaters, transform.position, ForceMode.Acceleration);

        searchParameters.startPosition = transform.position;
        _waterSurface.FindWaterSurfaceHeight(searchParameters, out searchResult);

        if (transform.position.y < searchResult.height)
        {
            float displacementMultiplier = Mathf.Clamp01((searchResult.height - transform.position.y) / _displacementAmount);
            _rigidbody.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), transform.position, ForceMode.Acceleration);
            _rigidbody.AddForce(displacementMultiplier * -_rigidbody.velocity * _waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            _rigidbody.AddTorque(displacementMultiplier * -_rigidbody.angularVelocity * _waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }
}
