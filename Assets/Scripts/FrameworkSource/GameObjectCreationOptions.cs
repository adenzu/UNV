using System.Collections;
using System.Collections.Generic;
using UnityEditor;
<<<<<<< Updated upstream
using UnityEngine;
=======
using UnityEditor.SceneManagement;
using UnityEngine;
using UNV.Pathfinding;
>>>>>>> Stashed changes

public static class GameObjectCreationOptions
{

<<<<<<< Updated upstream
    private static string boatPrefabPath = "Boat";
    private static string stationaryObstaclePrefabPath = "StationaryObstacle";
=======
    private static string boatPrefabPath = "Framework/Boat";
    private static string stationaryObstaclePrefabPath = "Framework/StationaryObstacle";
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
=======

    [MenuItem("GameObject/UNV/Create Tiling Manager")]
    private static void CreateTilingManager()
    {
        GameObject tilingManager = new GameObject("Tiling Manager");
        tilingManager.AddComponent<TilingManager>();
    }

    [MenuItem("GameObject/UNV/Create Path Request Manager")]
    private static void CreatePathRequestManager()
    {
        GameObject pathRequestManager = new GameObject("Path Request Manager");
        pathRequestManager.AddComponent<PathRequestManager>();
        pathRequestManager.AddComponent<Pathfinding>();
    }
>>>>>>> Stashed changes
}
