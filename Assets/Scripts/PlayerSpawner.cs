using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
public class PlayerSpawner : MonoBehaviour
{
    NetworkManager networkManager;

    Vector3 bottomSpawnPosition = new Vector3(x: 1.5f, y: 4f, z: 0);
    Vector3 topSpawnPosition = new Vector3(x: 8.5f, y: 6f, z: 0);

    private void Awake()
    {
        networkManager = gameObject.GetComponent<NetworkManager>();
        networkManager.ConnectionApprovalCallback += ConnectionApprovalWithSpawnPos;
    }

    void ConnectionApprovalWithSpawnPos(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        switch (networkManager.ConnectedClients.Count)
        {
            case 0:
                response.CreatePlayerObject = true;
                response.PlayerPrefabHash = 1504847109;
                // TODO: we should be able to set this in the prefab, but for some reason that is not working
                response.Position = bottomSpawnPosition;
                response.Approved = true;
                Debug.Log("Spawning Player 1");
                break;
            case 1:
                response.CreatePlayerObject = true;
                response.PlayerPrefabHash = 47039940;
                response.Position = topSpawnPosition;
                response.Approved = true;
                Debug.Log("Spawning Player 2");
                break;
            default:
                response.CreatePlayerObject = false;
                response.Approved = false;
                Debug.Log("Maximum player limit reached.");
                break;
        }
    }
}
