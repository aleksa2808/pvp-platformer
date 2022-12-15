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
        // TODO: this might need to happen only on the server since currently this can trigger on the server but not on a client leading to desyncs
        if (col.gameObject.tag == "Player")
        {
            Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
            gameController.PowerPadActive(powerPadLocation);
        }
    }
}
