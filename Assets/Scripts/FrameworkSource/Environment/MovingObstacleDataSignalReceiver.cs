using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacleDataSignalReceiver : MonoBehaviour
{
    public delegate void OnDataReceivedDelegate(int id, MovingObstacleData movingObstacleData);
    public event OnDataReceivedDelegate OnDataReceived;

    public void ReceiveData(int id, MovingObstacleData movingObstacleData)
    {
        if (OnDataReceived == null)
        {
            return;
        }
        OnDataReceived.Invoke(id, movingObstacleData);
    }
}
