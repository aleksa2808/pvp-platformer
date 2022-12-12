using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed;
    public float jumpForce;

    private Rigidbody2D rigidBody2d;

    public Transform platformCheck;
    public LayerMask platformLayer;

    bool isGrounded;

    float moveInput;
    bool jumpInput;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            rigidBody2d = GetComponent<Rigidbody2D>();
            updateGroundedState();
        }
    }

    void updateGroundedState()
    {
        Vector2 size = new Vector2(platformCheck.lossyScale.x, platformCheck.lossyScale.y);
        isGrounded = Physics2D.OverlapBox(point: platformCheck.position, size: size, angle: 0, layerMask: platformLayer);
    }

    void Update()
    {
        if (IsServer)
        {
            updateGroundedState();
        }

        if (IsOwner)
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
            rigidBody2d.velocity = new Vector2(moveInput * moveSpeed, rigidBody2d.velocity.y);

            if (jumpInput && isGrounded)
            {
                rigidBody2d.velocity = Vector2.up * jumpForce;
            }
        }
    }

    [ServerRpc]
    private void MoveServerRpc(float clientMoveInput, bool clientJumpInput)
    {
        moveInput = clientMoveInput;
        jumpInput = clientJumpInput;
    }
}
