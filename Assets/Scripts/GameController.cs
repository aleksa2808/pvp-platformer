using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    enum GameState
    {
        NoAdvantage,
        BottomPlayerAdvantage,
        TopPlayerAdvantage,
    };

    public GameObject bottomPlayer;
    public GameObject topPlayer;
    private PlayerMovement bottomPlayerMovement;
    private PlayerMovement topPlayerMovement;

    public GameObject cannon;

    public GameObject bottomPowerPad;
    public GameObject topPowerPad;

    public float cannonSpeed;

    private List<Vector3> bottomPowerPadPositions = new List<Vector3>() {
        new Vector3(x: 1.5f, y: 2.95f, z: 0f),
        new Vector3(x: 8.5f, y: 2.95f, z: 0f),
    };
    private int bottomPowerPadPositionIndex = 0;

    private List<Vector3> topPowerPadPositions = new List<Vector3>() {
        new Vector3(x: 1.5f, y: 7.05f, z: 0f),
        new Vector3(x: 8.5f, y: 7.05f, z: 0f),
    };
    private int topPowerPadPositionIndex = 0;

    private ulong bottomPlayerClientId;
    private ulong topPlayerClientId;

    private GameState gameState = GameState.NoAdvantage;

    private float cannonMoveInput = 0f;

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
        bottomPlayerMovement = bottomPlayer.GetComponent<PlayerMovement>();
        topPlayerMovement = topPlayer.GetComponent<PlayerMovement>();
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
        if (IsServer)
        {

        }

        if (IsClient)
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            bool jump = Input.GetKey(KeyCode.Space);
            MoveServerRpc(moveInput, jump);
        }
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            cannon.transform.position = cannon.transform.position + cannonMoveInput * cannonSpeed * Vector3.right * Time.deltaTime;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void MoveServerRpc(float clientMoveInput, bool clientActionInput, ServerRpcParams serverRpcParams = default)
    {
        if (serverRpcParams.Receive.SenderClientId == bottomPlayerClientId)
        {
            if (gameState == GameState.BottomPlayerAdvantage)
            {
                ControlCannon(clientMoveInput, clientActionInput);
            }
            else
            {
                bottomPlayerMovement.Control(clientMoveInput, clientActionInput);
            }
        }
        else if (serverRpcParams.Receive.SenderClientId == topPlayerClientId)
        {
            if (gameState == GameState.TopPlayerAdvantage)
            {
                ControlCannon(-clientMoveInput, clientActionInput);
            }
            else
            {
                topPlayerMovement.Control(clientMoveInput, clientActionInput);
            }
        }
    }

    public void PowerPadActive(PowerPadLocation powerPadLocation)
    {
        if (!IsServer) return;

        cannonMoveInput = 0;

        switch (powerPadLocation)
        {
            case PowerPadLocation.Bottom:
                gameState = GameState.BottomPlayerAdvantage;
                bottomPlayerMovement.Control(0, false);

                // TODO: improve constant
                topPowerPadPositionIndex = topPlayer.transform.position.x >= 5f ? 0 : 1;

                break;
            case PowerPadLocation.Top:
                gameState = GameState.TopPlayerAdvantage;
                topPlayerMovement.Control(0, false);

                // TODO: improve constant
                bottomPowerPadPositionIndex = bottomPlayer.transform.position.x >= 5f ? 0 : 1;

                break;
        };
        Debug.Log(gameState);
        UpdatePowerPadPositions();
        // UpdatePowerPadPositionsClientRpc();
    }

    // [ClientRpc]
    // void UpdatePowerPadPositionsClientRpc()
    // {
    //     UpdatePowerPadPositions();
    // }

    private void ControlCannon(float move, bool shoot)
    {
        cannonMoveInput = move;
    }
}
