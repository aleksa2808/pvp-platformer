using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    enum GameState
    {
        NoAdvantage,
        Player1Advantage,
        Player2Advantage,
    };

    List<Vector3> bottomPowerPadPositions = new List<Vector3>() {
        new Vector3(x: 8.5f, y: 2.95f, z: 0f),
        new Vector3(x: 1.5f, y: 2.95f, z: 0f),
    };
    int bottomPowerPadPositionIndex = 0;

    List<Vector3> topPowerPadPositions = new List<Vector3>() {
        new Vector3(x: 1.5f, y: 7.05f, z: 0f),
        new Vector3(x: 8.5f, y: 7.05f, z: 0f),
    };
    int topPowerPadPositionIndex = 0;

    public GameObject bottomPowerPad;
    public GameObject topPowerPad;

    void UpdatePowerPadPositions()
    {
        bottomPowerPad.transform.localPosition = bottomPowerPadPositions[bottomPowerPadPositionIndex];
        topPowerPad.transform.localPosition = topPowerPadPositions[topPowerPadPositionIndex];
    }

    public override void OnNetworkSpawn()
    {
        UpdatePowerPadPositions();
    }

    public void PowerPadActive(PowerPadLocation powerPadLocation)
    {
        switch (powerPadLocation)
        {
            case PowerPadLocation.Bottom:
                bottomPowerPadPositionIndex = (bottomPowerPadPositionIndex + 1) % bottomPowerPadPositions.Count;
                break;
            case PowerPadLocation.Top:
                topPowerPadPositionIndex = (topPowerPadPositionIndex + 1) % topPowerPadPositions.Count;
                break;
        };
        UpdatePowerPadPositions();
        // UpdatePowerPadPositionsClientRpc();
    }

    // [ClientRpc]
    // void UpdatePowerPadPositionsClientRpc()
    // {
    //     UpdatePowerPadPositions();
    // }
}
