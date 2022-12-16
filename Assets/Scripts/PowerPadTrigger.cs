using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PowerPadTrigger : NetworkBehaviour
{
    public PowerPadLocation powerPadLocation;

    public GameController gameController;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (IsServer && col.gameObject.tag == "Player")
        {
            Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
            gameController.PowerPadActive(powerPadLocation);
        }
    }
}
