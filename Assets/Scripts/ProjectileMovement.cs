using Unity.Netcode;
using UnityEngine;

public class ProjectileMovement : NetworkBehaviour
{
    public float moveSpeed;
    public float moveDirection;

    private void FixedUpdate()
    {
        if (IsServer)
        {
            transform.position = transform.position + moveDirection * moveSpeed * Vector3.up * Time.deltaTime;

            if (transform.position.y >= 10f || transform.position.y <= 0f)
            {
                GetComponent<NetworkObject>().Despawn();
            }
        }
    }

    void OnTriggerEnter2D()
    {
        if (IsServer)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
