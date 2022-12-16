using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
public class ConnectionHandler : MonoBehaviour
{
    NetworkManager networkManager;

    public GameObject normalCameraPrefab;
    public GameObject invertedCameraPrefab;

    public GameController gameController;

    private void Awake()
    {
        networkManager = gameObject.GetComponent<NetworkManager>();
        networkManager.ConnectionApprovalCallback += ConnectionHandlerCallback;
    }

    void ConnectionHandlerCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        switch (networkManager.ConnectedClients.Count)
        {
            case 0:
                {
                    response.CreatePlayerObject = false;
                    response.Approved = true;
                    Debug.Log("Bottom Player connected. ID: " + request.ClientNetworkId);

                    gameController.mapClientIDToPlayer(request.ClientNetworkId, PlayerType.BottomPlayer);

                    break;
                }
            case 1:
                {
                    response.CreatePlayerObject = false;
                    response.Approved = true;
                    Debug.Log("Top Player connected ID: " + request.ClientNetworkId);

                    gameController.mapClientIDToPlayer(request.ClientNetworkId, PlayerType.TopPlayer);

                    break;
                }
            default:
                response.CreatePlayerObject = false;
                response.Approved = false;
                Debug.Log("All players already connected.");
                break;
        }
    }
}
