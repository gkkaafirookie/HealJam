using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EnemyBase : MonoBehaviour
{

    public string enemyName;
    public GameObject Marker;

    [SerializeField]
    Renderer rend;

    Camera main;


    public Animator animator;
    public CombatHandler playerCombat;
    public EnemyGroupHandler enemyManager;
    public EnemyDetector enemyDetection;
    public CharacterController characterController;

    [Header("Stats")]
    public int health = 3;
    public int damageFactor;
    public int hitFactor;
    public float moveSpeed = 1;
    public Vector3 moveDirection;

    [Header("States")]
    [SerializeField] private bool isPreparingAttack;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isRetreating;
    [SerializeField] private bool isLockedTarget;
    [SerializeField] private bool isStunned;
    [SerializeField] private bool isWaiting = true;

    [Header("Polish")]
    [SerializeField] private ParticleSystem counterParticle;

    public Coroutine PrepareAttackCoroutine;
    public Coroutine RetreatCoroutine;
    public Coroutine DamageCoroutine;
    public Coroutine MovementCoroutine;

    //Events
    public UnityEvent<EnemyBase> OnDamage;
    public UnityEvent<EnemyBase> OnStopMoving;
    public UnityEvent<EnemyBase> OnRetreat;

    public bool IsPreparingAttack { get => isPreparingAttack; set => isPreparingAttack = value; }
    public bool IsMoving { get => isMoving; set => isMoving = value; }
    public bool IsRetreating { get => isRetreating; set => isRetreating = value; }
    public bool IsLockedTarget { get => isLockedTarget; set => isLockedTarget = value; }
    public bool IsStunned { get => isStunned; set => isStunned = value; }
    public bool IsWaiting { get => isWaiting; set => isWaiting = value; }

    // Start is called before the first frame update
    public virtual void Start()
    {
        main = Camera.main;


        playerCombat = FindObjectOfType<CombatHandler>();
        enemyDetection = FindObjectOfType<EnemyDetector>();

        playerCombat.OnHit.AddListener((x) => OnPlayerHit(x));
        playerCombat.OnCounterAttack.AddListener((x) => OnPlayerCounter(x));
        playerCombat.OnTrajectory.AddListener((x) => OnPlayerTrajectory(x));

        DoMovement();

        enemyName = GKUtils.GetRandomName();
    }

    public void DoMovement()
    {

        MovementCoroutine = StartCoroutine(EnemyMovement());
    }

    public void SetRender()
    {
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
   
        Marker.transform.LookAt(main.transform.position, -Vector3.up);

        Rotate();
        MoveEnemy(moveDirection);
    }

    public void SelectEnemy(bool marker)
    {
        Marker.SetActive(marker);
        if(rend == null)
            rend = GetComponentInChildren<SkinnedMeshRenderer>();
        if (rend == null)
            return;
        if (marker)
        {
            rend.material.SetColor("_RimLightColor", Color.red);
        }
        else
        {
            rend.material.SetColor("_RimLightColor", Color.blue);
        }
    }

    public abstract IEnumerator EnemyMovement();

    public abstract void StopMoving();

    public abstract void Rotate();

    public abstract void MoveEnemy(Vector3 direction);

    public abstract void Attack();

    public abstract void SetAttack();

    public abstract void SetRetreat();

    public abstract void ApplyGravity();

    public virtual void OnPlayerHit(EnemyBase target)
    {
        if (target == this)
        {
            StopEnemyCoroutines();
            DamageCoroutine = StartCoroutine(HitCoroutine());

            enemyDetection.SetCurrentTarget(null);
            isLockedTarget = false;
            OnDamage.Invoke(this);

            health -= hitFactor;

            if (health <= 0)
            {
                Death();
                return;
            }

            animator.SetTrigger("Hit");
            transform.DOMove(transform.position - (transform.forward / 2), .3f).SetDelay(.1f);

            StopMoving();
        }

        IEnumerator HitCoroutine()
        {
            isStunned = true;
            yield return new WaitForSeconds(.5f);
            isStunned = false;
        }
    }

    public virtual void OnPlayerCounter(EnemyBase target)
    {
        if (target == this)
        {
            PrepareAttack(false);
        }
    }

    public virtual void OnPlayerTrajectory(EnemyBase target)
    {
        if (target == this)
        {
            StopEnemyCoroutines();
            isLockedTarget = true;
            PrepareAttack(false);
            StopMoving();
        }
    }

    public virtual void Death()
    {
        StopEnemyCoroutines();

        this.enabled = false;
        characterController.enabled = false;
        animator.SetTrigger("Death");
        enemyManager.SetEnemyAvailiability(this, false);
        SelectEnemy(false);
    }


    public virtual void StopEnemyCoroutines()
    {
        PrepareAttack(false);

        if (IsRetreating)
        {
            if (RetreatCoroutine != null)
                StopCoroutine(RetreatCoroutine);
        }

        if (PrepareAttackCoroutine != null)
            StopCoroutine(PrepareAttackCoroutine);

        if (DamageCoroutine != null)
            StopCoroutine(DamageCoroutine);

        if (MovementCoroutine != null)
            StopCoroutine(MovementCoroutine);
    }


    public virtual void HitEvent()
    {
        if (!playerCombat.isCountering && !playerCombat.isAttackingEnemy)
            playerCombat.DamageEvent(damageFactor);

        PrepareAttack(false);
    }

    public virtual void PrepareAttack(bool active)
    {
        IsPreparingAttack = active;

        if (active)
        {
            counterParticle.Play();
        }
        else
        {
            StopMoving();
            counterParticle.Clear();
            counterParticle.Stop();
        }
    }

    
    public bool IsAttackable()
    {
        return health > 0;
    }


}
