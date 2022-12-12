using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
public class PlayerSpawner : MonoBehaviour
{
    NetworkManager m_NetworkManager;

    int m_RoundRobinIndex = 0;

    [SerializeField]
    List<Vector3> m_SpawnPositions = new List<Vector3>() {
        new Vector3(x: 1.5f, y: 4f, z: 0),
        // new Vector3(x: 8.5f, y: 6f, z: 0)
    };

    public Vector3 GetNextSpawnPosition()
    {
        m_RoundRobinIndex = (m_RoundRobinIndex + 1) % m_SpawnPositions.Count;
        return m_SpawnPositions[m_RoundRobinIndex];
    }

    private void Awake()
    {
        var networkManager = gameObject.GetComponent<NetworkManager>();
        networkManager.ConnectionApprovalCallback += ConnectionApprovalWithSpawnPos;
    }

    void ConnectionApprovalWithSpawnPos(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // Here we are only using ConnectionApproval to set the player's spawn position. Connections are always approved.
        response.CreatePlayerObject = true;
        response.Position = GetNextSpawnPosition();
        response.Rotation = Quaternion.identity;
        response.Approved = true;
    }
}
