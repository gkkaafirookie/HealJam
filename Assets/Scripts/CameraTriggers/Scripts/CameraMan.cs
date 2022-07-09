using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMan : Singleton<CameraMan>
{

    
    private GameObject blah;

    public CustomList<string, CinemachineVirtualCameraBase> cameras;

    public string currentCamera;
   
    public static void ChangeCamera(string to)
    {
        ChangeCamera(Instance.currentCamera, 0);
        ChangeCamera(to, 1);
        Instance.currentCamera = to;
    }

    public static void ChangeCamera(string from, string to)
    {

      
        ChangeCamera(from, 0);
        ChangeCamera(to);
    }

    public static void ChangeCamera(string key, int prioroty)
    {
        var cam = Instance.cameras.Get(key);
        cam.Priority = prioroty;
    }
}
