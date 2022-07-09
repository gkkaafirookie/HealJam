using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : EnemyBase
{

    public Vector3 gravity;
    public float gravityFactor;
    public Transform groundDetection;

    public bool isGrounded;
    public float groundRadius;
    public LayerMask groundMask;
    [TagSelect]
    public string[] collideTags;
    public override void ApplyGravity()
    {
        
           
        
    }

    public override void Attack()
    {
        transform.DOMove(transform.position + (transform.forward / 1), .5f);
        animator.SetTrigger("AirPunch");
    }

    public override void Update()
    {
        base.Update();
        isGrounded = Physics.CheckSphere(groundDetection.position, groundRadius, groundMask);
    }

    public override IEnumerator EnemyMovement()
    {
        yield return new WaitUntil(() => IsWaiting == true);

        int randomChance = Random.Range(0, 2);

        if (randomChance == 1)
        {
            int randomDir = Random.Range(0, 2);
            moveDirection = randomDir == 1 ? Vector3.right : Vector3.left;
            IsMoving = true;
        }
        else
        {
            StopMoving();
        }

        yield return new WaitForSeconds(1);

        MovementCoroutine = StartCoroutine(EnemyMovement());
    }

    public override void MoveEnemy(Vector3 direction)
    {
        //Set movespeed based on direction
        moveSpeed = 1;

        if (direction == Vector3.forward)
            moveSpeed = 5;
        if (direction == -Vector3.forward)
            moveSpeed = 2;

        //Set Animator values
        animator.SetFloat("InputMagnitude", (characterController.velocity.normalized.magnitude * direction.z) / (5 / moveSpeed), .2f, Time.deltaTime);
        animator.SetBool("Strafe", (direction == Vector3.right || direction == Vector3.left));
        animator.SetFloat("StrafeDirection", direction.normalized.x, .2f, Time.deltaTime);

        //Don't do anything if isMoving is false
        if (!IsMoving)
            return;

    

      

        Vector3 dir = (playerCombat.transform.position - transform.position).normalized;
        Vector3 pDir = Quaternion.AngleAxis(90, Vector3.up) * dir; //Vector perpendicular to direction
        Vector3 movedir = Vector3.zero;

        Vector3 finalDirection = Vector3.zero;

        if (direction == Vector3.forward)
            finalDirection = dir;
        if (direction == Vector3.right || direction == Vector3.left)
            finalDirection = (pDir * direction.normalized.x);
        if (direction == -Vector3.forward)
            finalDirection = -transform.forward;

        if (direction == Vector3.right || direction == Vector3.left)
            moveSpeed /= 1.5f;

        movedir += finalDirection * moveSpeed * Time.deltaTime;

        if (!isGrounded)
        {
            gravity += Physics.gravity * Time.deltaTime * gravityFactor;
            if (gravity.y > 0)
                gravity += Physics.gravity * Time.deltaTime * gravityFactor;
            movedir += gravity;
        }
        

        characterController.Move(movedir);

        if (!IsPreparingAttack)
            return;

        if (Vector3.Distance(transform.position, playerCombat.transform.position) < 2)
        {
            StopMoving();
            if (!playerCombat.isCountering && !playerCombat.isAttackingEnemy)
                Attack();
            else
                PrepareAttack(false);
        }
    }

    public override void Rotate()
    {
        transform.LookAt(new Vector3(playerCombat.transform.position.x, transform.position.y, playerCombat.transform.position.z));
    }

    public override void SetAttack()
    {
        IsWaiting = false;

        
        PrepareAttackCoroutine = StartCoroutine(PrepAttack());
        
        IEnumerator PrepAttack()
        {
            //moveDirection = (- transform.position + playerCombat.transform.position).normalized;
           // IsMoving = true;
            //yield return new WaitUntil(() => Vector3.Distance(transform.position, playerCombat.transform.position) < 2);

            PrepareAttack(true);
            yield return new WaitForSeconds(.2f);
            moveDirection = Vector3.forward;
            IsMoving = true;
        }
    }

    public override void SetRetreat()
    {
        StopEnemyCoroutines();

        RetreatCoroutine = StartCoroutine(PrepRetreat());

        IEnumerator PrepRetreat()
        {
            yield return new WaitForSeconds(1.4f);
            OnRetreat.Invoke(this);
            IsRetreating = true;
            moveDirection = -Vector3.forward;
            IsMoving = true;
            yield return new WaitForSeconds(1.4f);
            IsRetreating = false;
            StopMoving();

            //Free 
            IsWaiting = true;
            MovementCoroutine = StartCoroutine(EnemyMovement());
        }
    }

    public override void StopMoving()
    {
        IsMoving = false;
        moveDirection = Vector3.zero;
       /* if (characterController.enabled)
            characterController.Move(moveDirection);*/
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(CheckTags(hit))
        {
            int randomChance = Random.Range(0, 2);
            Debug.Log("hit " + hit.collider.gameObject.name) ;
            if (randomChance == 1)
            {
                int randomDir = Random.Range(0, 2);
                moveDirection = -moveDirection;
                IsMoving = true;
            }
            else
            {
                if(IsRetreating)
                {
                    moveDirection = -moveDirection;
                    IsRetreating = false;
                    IsWaiting = true;
                    return;
                }
                StopMoving();
            }
        }
    }

    bool CheckTags(ControllerColliderHit hit)
    {
        foreach(var tag in collideTags)
        {
            if (hit.collider.CompareTag(tag))
                return true;
        }
        return false;
    }
}
