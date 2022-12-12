using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    public GameObject cameraPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Instantiate(cameraPrefab, cameraPrefab.transform.position, cameraPrefab.transform.rotation);
            // cameraToActivate.SetActive(true);
        }
    }
}
