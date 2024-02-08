using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class GameObjectCreationOptions
{

    private static string boatPrefabPath = "Boat";
    private static string stationaryObstaclePrefabPath = "StationaryObstacle";

    [MenuItem("GameObject/UNV/Create Boat")]
    private static void CreateBoat()
    {
        GameObject boatPrefab = Resources.Load<GameObject>(boatPrefabPath);
        Object.Instantiate(boatPrefab).name = "Boat";
    }

    [MenuItem("GameObject/UNV/Create Stationary Obstacle")]
    private static void CreateStationaryObstacle()
    {
        GameObject stationaryObstaclePrefab = Resources.Load<GameObject>(stationaryObstaclePrefabPath);
        Object.Instantiate(stationaryObstaclePrefab).name = "Stationary Obstacle";
    }

    [MenuItem("GameObject/UNV/Create Position Estimator")]
    private static void CreatePositionEstimator()
    {
        GameObject positionEstimator = new GameObject("Position Estimator");
        positionEstimator.AddComponent<MovingObstaclePositionEstimator>();
    }
}
