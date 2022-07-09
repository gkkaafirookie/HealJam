using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraSwitchTrigger : GkMono
{
    
    public CinemachineVirtualCameraBase FromCam;
    public CinemachineVirtualCameraBase ToCam;

    public List<GameObject> DeActiveObjects = new List<GameObject>();
    public List<GameObject> ActivateObjects = new List<GameObject>();

    public TriggerType TriggerType;

    public bool isTriggered;
    public bool twoWay;

    private bool busy;

    public bool isDual;
    public GameObject InTrigger, OutTrigger;

    private void Start()
    {
        if(isDual)
        {
            var pos = InTrigger.transform.localPosition;
            var rot = InTrigger.transform.localRotation;
            var scale = InTrigger.transform.localScale;
            SetPosRot(pos, rot, scale);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (TriggerType == TriggerType.OnExit)
        {
            StartCoroutine(ShiftCamCo());
        }
    }

    private IEnumerator ShiftCamCo()
    {
        if (busy)
            yield break;
        busy = true;
        ShiftCam();
        isTriggered = !isTriggered;
        yield return new WaitForSeconds(2f);
        
        if(isTriggered)
        {
            var pos = OutTrigger.transform.localPosition;
            var rot = OutTrigger.transform.localRotation;
            var scale = OutTrigger.transform.localScale;
            SetPosRot(pos, rot, scale);
        }
        else
        {
            var pos = InTrigger.transform.localPosition;
            var rot = InTrigger.transform.localRotation;
            var scale = InTrigger.transform.localScale;
            SetPosRot(pos, rot, scale);
        }

        busy = false;
    }

    private void SetPosRot(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        transform.localPosition = pos;
        transform.localRotation = rot;
        transform.localScale = scale;
    }

    public void ShiftCam()
    {
        if (!twoWay && !isTriggered)
        {
            ShiftToCam();
            return;
        }
        else if (!twoWay && isTriggered)
        {
            return;
        }
        if (isTriggered)
        {
            ShiftFromCam();
        }
        else
        {
            ShiftToCam();
        }
    }

    private void ShiftToCam()
    {
        Debug.Log("Shift To Cam");

        SetList(DeActiveObjects, false);
        SetList(ActivateObjects, true);
        if (FromCam != null)
        {
            FromCam.Priority = 0;
        }
        if (ToCam != null)
        {
            ToCam.Priority = 1;
        }
       
    }

    private void ShiftFromCam()
    {
        Debug.Log("Shift From Cam");

        SetList(DeActiveObjects, true);
        SetList(ActivateObjects, false);
        if (FromCam != null)
        {
            FromCam.Priority = 1;
        }
        if (ToCam != null)
        {
            ToCam.Priority = 0;
        }
     

    }

    private void OnTriggerEnter(Collider other)
    {
        if (TriggerType == TriggerType.OnEnter)
        {
            StartCoroutine(ShiftCamCo());
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (FromCam != null)
        {
            Vector3 position = FromCam.transform.position;
         
            DrawGizmo(position, FromCam.Name, Colors.LightSalmon);
        }
        if (ToCam != null)
        {
            Vector3 position = ToCam.transform.position;
            var color =  Colors.Green;
           
            DrawGizmo(position, ToCam.Name, color);
        }

        foreach(var item in ActivateObjects)
        {
            DrawLine(item.transform.position, color: Colors.LightGreen);
        }
        foreach (var item in DeActiveObjects)
        {
            DrawLine(item.transform.position, color: Colors.Orangered);
        }
    }

    private void DrawGizmo(Vector3 position, string message, Color lineColor , Vector3 LablePos =default)
    {
        LablePos = position - ((position - transform.position).normalized * 0.5f) ;
        DrawGizmoLables(message, LablePos, lineColor);
        DrawLine(position, lineColor, 10);
    }
}

public enum TriggerType
{
    OnExit,
    OnEnter
}