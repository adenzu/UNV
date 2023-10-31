using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace UNV.Path
{
    public class PathFollower : MonoBehaviour
    {
        public PathCreator pathCreator;

        private Vector3[] _pathPoints;
        private float _pathLength;

        [SerializeField] private bool _loop = false;

        [SerializeField] private float _duration = 5f;

        private void Awake()
        {
            UpdatePath();
            StartMotion();
        }

        public void UpdatePath()
        {
            SetPath(pathCreator.GetPathPoints());
        }

        public void SetPath(Vector3[] pathPoints)
        {
            _pathPoints = pathPoints;
            transform.position = _pathPoints[0];
            UpdatePathLength();
        }

        public void StartMotion()
        {
            UpdatePath();
            transform.DOKill();
            transform.DOPath(_pathPoints, _duration, PathType.CatmullRom)
            .SetOptions(_loop)
            .SetLoops(_loop ? -1 : 0)
            .SetLookAt(0.01f)
            .SetEase(Ease.Linear);
        }

        public void PauseMotion()
        {
            transform.DOPause();
        }

        public void ResumeMotion()
        {
            transform.DOPlay();
        }

        private void UpdatePathLength()
        {
            _pathLength = 0;
            for (int i = 1; i < _pathPoints.Length; i++)
            {
                _pathLength += Vector3.Distance(_pathPoints[i], _pathPoints[i - 1]);
            }
        }
    }
}

