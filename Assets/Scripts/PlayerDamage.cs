using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerDamage : NetworkBehaviour
{
    public PlayerType playerType;

    public GameController gameController;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (IsServer && col.gameObject.tag == "Damage")
        {
            Debug.Log(gameObject.name + " damaged by " + col.gameObject.name + " at " + Time.time + "s");
            gameController.PlayerDamaged(playerType);
        }
    }
}
