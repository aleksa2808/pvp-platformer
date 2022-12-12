using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed;
    public float jumpForce;

    private Rigidbody2D rigidBody2d;

    public Transform platformCheck;
    public LayerMask platformLayer;

    NetworkVariable<bool> isGrounded = new NetworkVariable<bool>();
    SpriteRenderer spriteRenderer;

    float moveInput;
    bool jumpInput;

    public GameObject jumpFieldPrefab;
    GameObject jumpField;

    public override void OnNetworkSpawn()
    {
        rigidBody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        jumpField = Instantiate(jumpFieldPrefab, platformCheck.position, Quaternion.identity, transform);

        if (IsServer)
        {
            updateGroundedState();
        }
    }

    void updateGroundedState()
    {
        Vector2 size = new Vector2(jumpField.transform.lossyScale.x, jumpField.transform.lossyScale.y);
        isGrounded.Value = Physics2D.OverlapBox(point: jumpField.transform.position, size: size, angle: 0, layerMask: platformLayer);
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

            if (jumpInput && isGrounded.Value)
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
