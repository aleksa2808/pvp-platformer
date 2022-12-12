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

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        rigidBody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        jumpField = Instantiate(jumpFieldPrefab, platformCheck.position, Quaternion.identity, transform);

        if (IsServer)
        {
            updateGroundedState();
            changeColor();
        }
    }

    void updateGroundedState()
    {
        // TODO: is there a better way to specify this size? currently it's hardcoded so if the gameobject size changes this needs to be updated as well
        // isGrounded.Value = Physics2D.OverlapBox(point: platformCheck.position, size: new Vector2(0.2f, 0.05f), angle: 0, layerMask: platformLayer);

        Vector2 size = new Vector2(0.99f * jumpField.transform.lossyScale.x, jumpField.transform.lossyScale.y);
        isGrounded.Value = Physics2D.OverlapBox(point: jumpField.transform.position, size: size, angle: 0, layerMask: platformLayer);
    }

    void changeColor()
    {
        if (isGrounded.Value)
        {
            spriteRenderer.color = Color.blue;
        }
        else
        {
            spriteRenderer.color = Color.cyan;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            updateGroundedState();
        }
        changeColor();

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
            // transform.position += new Vector3(moveInput * moveSpeed * Time.deltaTime, 0, 0);
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
