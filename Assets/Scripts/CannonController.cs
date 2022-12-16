using Unity.Netcode;
using UnityEngine;

public class CannonController : NetworkBehaviour
{
    public float moveSpeed;
    public GameController gameController;
    public GameObject projectilePrefab;

    private float moveInput = 0f;
    private bool shootInput;

    private float shootCooldown = 0.7f;
    private float shootAvailableAt = 0f;


    public void Control(float moveInput, bool shootInput)
    {
        this.moveInput = moveInput;
        this.shootInput = shootInput;
    }

    private void Update()
    {
        if (IsServer)
        {
            if (shootInput)
            {
                var t = Time.time;
                if (t >= shootAvailableAt)
                {
                    shootAvailableAt = t + shootCooldown;

                    var prefabPosition = projectilePrefab.transform.position;
                    GameObject go = Instantiate(projectilePrefab, new Vector3(transform.position.x, transform.position.y, prefabPosition.z), Quaternion.identity);
                    go.GetComponent<ProjectileMovement>().moveDirection = gameController.gameState == GameState.BottomPlayerAdvantage ? 1 : -1;
                    go.GetComponent<NetworkObject>().Spawn();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            transform.position = transform.position + moveInput * moveSpeed * Vector3.right * Time.deltaTime;

            if (transform.position.x > 10f)
            {
                transform.position = new Vector3(10f, transform.position.y, transform.position.z);
            }
            else if (transform.position.x < 0f)
            {
                transform.position = new Vector3(0f, transform.position.y, transform.position.z);
            }
        }
    }
}
