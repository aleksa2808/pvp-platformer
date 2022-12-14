using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum GameState
{
    NoAdvantage,
    BottomPlayerAdvantage,
    TopPlayerAdvantage,
};

public class GameController : NetworkBehaviour
{
    public GameState gameState = GameState.NoAdvantage;

    public GameObject bottomPlayer;
    public GameObject topPlayer;
    private PlayerController bottomPlayerController;
    private PlayerController topPlayerController;

    public CannonController cannonController;

    public GameObject bottomPowerPad;
    public GameObject topPowerPad;

    private List<Vector3> bottomPowerPadPositions = new List<Vector3>() {
        new Vector3(x: 1.5f, y: 2.95f, z: 0f),
        new Vector3(x: 8.5f, y: 2.95f, z: 0f),
    };
    private int bottomPowerPadPositionIndex = 1;

    private List<Vector3> topPowerPadPositions = new List<Vector3>() {
        new Vector3(x: 1.5f, y: 7.05f, z: 0f),
        new Vector3(x: 8.5f, y: 7.05f, z: 0f),
    };
    private int topPowerPadPositionIndex = 0;

    private ulong bottomPlayerClientId;
    private ulong topPlayerClientId;

    private float lastMoveInput = 0f;
    private bool lastActionInput = false;

    public void mapClientIDToPlayer(ulong clientId, PlayerType playerType)
    {
        switch (playerType)
        {
            case PlayerType.BottomPlayer:
                bottomPlayerClientId = clientId;
                break;
            case PlayerType.TopPlayer:
                topPlayerClientId = clientId;
                break;
        }
    }

    void UpdatePowerPadPositions()
    {
        bottomPowerPad.transform.localPosition = bottomPowerPadPositions[bottomPowerPadPositionIndex];
        topPowerPad.transform.localPosition = topPowerPadPositions[topPowerPadPositionIndex];
    }

    void Start()
    {
        bottomPlayerController = bottomPlayer.GetComponent<PlayerController>();
        topPlayerController = topPlayer.GetComponent<PlayerController>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UpdatePowerPadPositions();
        }
    }

    void Update()
    {
        if (IsClient)
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            bool actionInput = Input.GetKey(KeyCode.Space);

            if (moveInput != lastMoveInput || actionInput != lastActionInput)
            {
                MoveServerRpc(moveInput, actionInput);
                lastMoveInput = moveInput;
                lastActionInput = actionInput;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void MoveServerRpc(float clientMoveInput, bool clientActionInput, ServerRpcParams serverRpcParams = default)
    {
        if (serverRpcParams.Receive.SenderClientId == bottomPlayerClientId)
        {
            if (gameState == GameState.BottomPlayerAdvantage)
            {
                cannonController.Control(clientMoveInput, clientActionInput);
            }
            else
            {
                bottomPlayerController.Control(clientMoveInput, clientActionInput);
            }
        }
        else if (serverRpcParams.Receive.SenderClientId == topPlayerClientId)
        {
            if (gameState == GameState.TopPlayerAdvantage)
            {
                cannonController.Control(-clientMoveInput, clientActionInput);
            }
            else
            {
                topPlayerController.Control(clientMoveInput, clientActionInput);
            }
        }
    }

    public void PowerPadActive(PowerPadLocation powerPadLocation)
    {
        if (!IsServer) return;

        cannonController.Control(0, false);

        switch (powerPadLocation)
        {
            case PowerPadLocation.Bottom:
                gameState = GameState.BottomPlayerAdvantage;
                bottomPlayerController.Control(0, false);

                topPowerPadPositionIndex = topPlayer.transform.position.x >= 5f ? 0 : 1;

                break;
            case PowerPadLocation.Top:
                gameState = GameState.TopPlayerAdvantage;
                topPlayerController.Control(0, false);

                bottomPowerPadPositionIndex = bottomPlayer.transform.position.x >= 5f ? 0 : 1;

                break;
        };
        Debug.Log(gameState);
        UpdatePowerPadPositions();
        UpdatePowerPadPositionsClientRpc(bottomPowerPadPositionIndex, topPowerPadPositionIndex);
    }

    [ClientRpc]
    void UpdatePowerPadPositionsClientRpc(int bottomPowerPadPositionIndex, int topPowerPadPositionIndex)
    {
        this.bottomPowerPadPositionIndex = bottomPowerPadPositionIndex;
        this.topPowerPadPositionIndex = topPowerPadPositionIndex;
        UpdatePowerPadPositions();
    }

    public void PlayerDamaged(PlayerType playerType)
    {
        if (!IsServer) return;

        // TODO: restart match
    }
}
