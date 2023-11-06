using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNV.Path2D
{
    [RequireComponent(typeof(Rigidbody))]
    public class PathFollower : MonoBehaviour
    {
        public PathCreator pathCreator;

        [SerializeField] private bool _startOnAwake = true;

        [SerializeField, Min(0)] private float _force = 1f;
        [SerializeField, Min(0)] private float _speed = 5f;

        [SerializeField] private bool _loop = false;

        [SerializeField] private bool _lookForward = true;

        [SerializeField, Min(0)] private float _deadZone = 0.1f;

        private Vector3[] _pathPoints;

        private int _targetPointIndex = 0;
        private bool _isMoving = false;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            UpdatePath();
            if (_startOnAwake)
            {
                PlayMotion();
            }
        }

        private void FixedUpdate()
        {
            if (_isMoving)
            {
                Vector3 targetPosition = _pathPoints[_targetPointIndex];
                Vector3 direction = (targetPosition - transform.position).normalized;
                PhysicsHelper.ApplyForceToReachVelocity(_rigidbody, direction * _speed, _force);

                if (_lookForward)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.1f);
                }

                if (Vector2.Distance(transform.position.XZ(), targetPosition.XZ()) < _deadZone)
                {
                    _targetPointIndex++;
                    if (_targetPointIndex >= _pathPoints.Length)
                    {
                        if (_loop)
                        {
                            _targetPointIndex = 0;
                        }
                        else
                        {
                            PauseMotion();
                        }
                    }
                }
            }
        }

        public void UpdatePath()
        {
            SetPath(pathCreator.GetPathPoints());
        }

        public void SetPath(Vector3[] pathPoints)
        {
            _pathPoints = pathPoints;
            _targetPointIndex = 0;
            transform.position = _pathPoints[0];
        }

        public void PlayMotion()
        {
            _isMoving = true;
        }

        public void PauseMotion()
        {
            _isMoving = false;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3[] pathPoints = pathCreator.GetPathPoints();
            if (pathPoints != null)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < pathPoints.Length; i++)
                {
                    Gizmos.DrawSphere(pathPoints[i], _deadZone);
                }
            }
        }
    }
}

