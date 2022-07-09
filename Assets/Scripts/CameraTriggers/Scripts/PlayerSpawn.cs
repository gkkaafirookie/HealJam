using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : GkMono
{
    public string Camera;

    public List<GameObject> DeActiveObjects = new List<GameObject>();
    public List<GameObject> ActivateObjects = new List<GameObject>();


    public void SetSpawn(GameObject Player)
    {
        SetList(DeActiveObjects, false);
        SetList(ActivateObjects, true);
        CameraMan.ChangeCamera(Camera);
        Player.transform.SetPositionAndRotation(transform.position, transform.rotation);
        Player.SetActive(true);
        
    }
}
