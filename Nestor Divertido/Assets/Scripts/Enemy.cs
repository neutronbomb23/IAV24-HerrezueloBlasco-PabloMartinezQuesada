using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : LivingEntity
{
    public enum State { Idle, Chasing, Attacking };
    State currentState;

    public ParticleSystem deathEffect;
    public static event System.Action OnDeathStatic;

    NavMeshAgent pathfinder;
    Transform target;
    Material skinMaterial;

    Color originalColor;

    float attackDistanceThreshold = .5f;
    float timeBetweenAttacks = 1f;
    float damage = 1f;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool isAttacking = false;

    float attackPercent;
    Vector3 originalPosition;
    Vector3 attackPosition;
    float attackSpeed = 3f;
    bool hasAppliedDamage = false;

    private void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<MeshRenderer>().material;

        // Busca un jugador en la escena
        target = GameObject.FindGameObjectWithTag("Player").transform;
        targetEntity = target.GetComponent<LivingEntity>();
        myCollisionRadius = this.GetComponent<CapsuleCollider>().radius;
        targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
    }

    public override void Start()
    {
        base.Start();

        // Si tiene un objetivo, cambia el estado a Persiguiendo
        currentState = State.Chasing;
        targetEntity.OnDeath += OnTargetDeath;
    }

    // Establece las características del enemigo
    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor)
    {
        pathfinder.speed = moveSpeed;

        damage = Mathf.Ceil(targetEntity.startingLife / hitsToKillPlayer);

        startingLife = enemyHealth;

        ParticleSystem.MinMaxGradient color = new ParticleSystem.MinMaxGradient();
        color.mode = ParticleSystemGradientMode.Color;
        color.color = new Color(skinColor.r, skinColor.g, skinColor.b, 1f);

        ParticleSystem.MainModule main = deathEffect.main;
        main.startColor = color;

        skinMaterial.color = skinColor;
        originalColor = skinColor;
    }

    // Maneja el daño recibido por el enemigo
    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (damage >= startingLife && !dead)
        {
            if (OnDeathStatic != null)
            {
                OnDeathStatic();
            }
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.main.startLifetime.constant);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    // Maneja la muerte del objetivo
    void OnTargetDeath()
    {
        currentState = State.Idle;
    }

    private void Update()
    {
        // Si tiene un objetivo, actualiza su comportamiento según el estado actual
        switch (currentState)
        {
            case State.Chasing:
                UpdatePath();
                break;
            case State.Attacking:
                PerformAttack();
                break;
        }

        // Inicia el ataque si está dentro del rango y no está atacando
        if (Time.time > nextAttackTime && !isAttacking)
        {
            float sqrDistanceToTarget = (target.position - this.transform.position).sqrMagnitude;
            if (sqrDistanceToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
            {
                nextAttackTime = Time.time + timeBetweenAttacks;
                StartAttack();
            }
        }
    }

    // Actualiza la ruta del enemigo hacia el objetivo
    void UpdatePath()
    {
        if (currentState == State.Chasing && !dead)
        {
            Vector3 dirToTarget = (target.position - this.transform.position).normalized;
            Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
            pathfinder.SetDestination(targetPosition);
        }
    }

    // Inicia el ataque al objetivo
    void StartAttack()
    {
        isAttacking = true;
        currentState = State.Attacking;
        pathfinder.enabled = false;

        originalPosition = this.transform.position;
        Vector3 dirToTarget = (target.position - this.transform.position).normalized;
        attackPosition = target.position - dirToTarget * (myCollisionRadius);

        attackPercent = 0f;
        skinMaterial.color = Color.red;
        hasAppliedDamage = false;
    }

    // Realiza el ataque al objetivo
    void PerformAttack()
    {
        if (isAttacking)
        {
            attackPercent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(attackPercent, 2) + attackPercent) * 4;
            this.transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            if (attackPercent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }

            if (attackPercent >= 1)
            {
                EndAttack();
            }
        }
    }

    // Termina el ataque y vuelve a perseguir al objetivo
    void EndAttack()
    {
        isAttacking = false;
        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }
}