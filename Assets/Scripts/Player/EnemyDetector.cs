using Climbing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{

    public CastType castType;
    [SerializeField]
    private InputCharacterController movementInput;
    public LayerMask layerMask;
    public LayerMask groundMask;
    [SerializeField] 
    private EnemyBase currentTarget;

    Vector3 inputDirection;

    public GameObject Cursor;
    public int directionMux;

    public Vector3 offset;

    Camera Camera;

    Vector3 forward;
    Vector3 right;

    public HealthBarUi healthBar;

    bool TargetDetected;
    // Start is called before the first frame update
    void Start()
    {
        Camera = Camera.main;
      
    }

    // Update is called once per frame
    void Update()
    {
        forward = Camera.transform.forward;
        right = Camera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        inputDirection = forward * movementInput.movement.y + right * movementInput.movement.x;
        inputDirection = inputDirection.normalized;

        Cursor.transform.LookAt(Camera.main.transform.position, -Vector3.up);
        RaycastHit infoMouseHit;
        switch (castType)
        {
            case CastType.PlayerPos:


                if (Physics.SphereCast(transform.position, 2f, inputDirection, out infoMouseHit, 10, layerMask))
                {
                    //if (info.collider.transform.GetComponent<EnemyScript>().IsAttackable())
                    currentTarget = infoMouseHit.collider.GetComponent<EnemyBase>();
                }
                break;
            case CastType.Mouse:

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


                if (Physics.Raycast(ray, out infoMouseHit, 100, layerMask))
                {
                    Cursor.SetActive(true);
                    EnemyBase NewEnemy = infoMouseHit.collider.GetComponent<EnemyBase>();

                    if (currentTarget != null)
                    {


                        if (currentTarget != NewEnemy)
                        {
                            currentTarget.SelectEnemy(false);
                            currentTarget = NewEnemy;
                        }
                        currentTarget.SelectEnemy(true);

                    }
                    else
                    {
                        currentTarget = NewEnemy;
                        currentTarget.SelectEnemy(true);
                    }


                    Vector3 position = infoMouseHit.transform.position;
                    Cursor.transform.position = position + Vector3.up;
                }
                else
                {
                    if (currentTarget != null)
                        currentTarget.SelectEnemy(false);
                    Cursor.SetActive(false);
                }
                break;
            case CastType.PlayerMouse:
                Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);


                if (Physics.Raycast(ray2, out infoMouseHit, 100, groundMask))
                {

                    Vector3 pos = infoMouseHit.point;
                    pos.y = transform.position.y;
                    Vector3 dir = (pos -transform.position).normalized;
                    Debug.DrawLine(infoMouseHit.point, pos, Color.blue, 2f);

                    Vector3 direction = dir * directionMux;
                    Ray playerRay = new Ray(transform.position +offset, direction);
                    Debug.DrawRay(transform.position + offset, direction, Color.red, 2f);
                    TargetDetected = Physics.Raycast(playerRay, out RaycastHit infoMouseHit2, 100, layerMask);
                    if (TargetDetected)
                    {
                        Cursor.SetActive(true);
                        EnemyBase NewEnemy = infoMouseHit2.collider.GetComponent<EnemyBase>();

                        if (currentTarget != null)
                        {
                            if (currentTarget != NewEnemy)
                            {
                                currentTarget.SelectEnemy(false);
                                if(NewEnemy.IsAttackable())
                                {
                                    currentTarget = NewEnemy;
                                    healthBar.ShowBar(true);
                                    healthBar.SetName(currentTarget.enemyName);
                                }
                            }
                            if (currentTarget.IsAttackable())
                            {
                                currentTarget.SelectEnemy(true);
                            
                            }
                            healthBar.UpdateUI(currentTarget.health);
                        }
                        else
                        {
                            if (NewEnemy.IsAttackable())
                            {
                                currentTarget = NewEnemy;
                                currentTarget.SelectEnemy(true);
                                healthBar.ShowBar(true);
                          
                                healthBar.SetName(currentTarget.enemyName);

                            }

                            if (currentTarget != null)
                                healthBar.UpdateUI(currentTarget.health);

                        }


                        Vector3 position = infoMouseHit2.transform.position;
                        Cursor.transform.position = position + Vector3.up;
                    }
                    else
                    {
                        if (currentTarget != null)
                            currentTarget.SelectEnemy(false);
                        Cursor.SetActive(false);
                        StartCoroutine(Hide());
                    }
                }
               
                break;
        }


    }

    private IEnumerator Hide()
    {
        yield return new WaitForSeconds(2f);
        if(!TargetDetected)
            healthBar.ShowBar(false);
    }

    internal void SetCurrentTarget(EnemyBase p)
    {
        currentTarget = p;
    }

    public EnemyBase CurrentTarget()
    {
        return currentTarget;
    }

    public float InputMagnitude()
    {
        return inputDirection.magnitude;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, inputDirection);
        Gizmos.DrawWireSphere(transform.position, 1);
        if (CurrentTarget() != null)
            Gizmos.DrawSphere(CurrentTarget().transform.position, .5f);
    }

}


public enum CastType
{
    PlayerPos,
    Mouse,
    PlayerMouse
}