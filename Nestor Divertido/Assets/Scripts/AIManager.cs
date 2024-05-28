using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public Player player;
    private PlayerController playerController;
    public Context context;
    public Scanner scanner;

    public MoveToPickup moveToPickup;
    public MoveToBestPosition moveToBestPosition;

    public UseLife useHealth;
    public ReloadGun reloadGun;
    public SetBestAttackTarget setBestAttackTarget;

    [Header("Tweaks")]
    [Range(0f, 3f)]
    public float coolDownToShoot = 1.5f;

    [Header("Scanner")]
    public float scanTimeIntervalInSecs = 1f;
    [Range(0f, 30f)]
    public float enemyScanRange = 10f;
    [Range(0f, 30f)]
    public float pickupScanRange = 10f;
    [Range(0.5f, 3f)]
    public float samplingDensity = 1.5f;
    [Range(3f, 30f)]
    public float samplingRange = 12f;

    private enum AIBehavior
    {
        ProximityAttack,
        LowHealthPriority
    }

    private AIBehavior currentBehavior = AIBehavior.ProximityAttack;

    void Awake()
    {
        this.player = this.gameObject.GetComponent<Player>();
        this.context = new Context(player);
        this.scanner = this.gameObject.AddComponent<Scanner>();
        this.playerController = player.GetComponent<PlayerController>();
        this.moveToPickup = new MoveToPickup();
        this.moveToBestPosition = new MoveToBestPosition();
        this.useHealth = new UseLife();
        this.reloadGun = new ReloadGun();
        this.setBestAttackTarget = new SetBestAttackTarget();
    }

    void Start()
    {
        InvokeRepeating("Scan", scanTimeIntervalInSecs, scanTimeIntervalInSecs);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleBehavior();
        }
    }

    private void ToggleBehavior()
    {
        if (currentBehavior == AIBehavior.ProximityAttack)
        {
            currentBehavior = AIBehavior.LowHealthPriority;
        }
        else
        {
            currentBehavior = AIBehavior.ProximityAttack;
        }
        Debug.Log("Behavior changed to: " + currentBehavior);
    }

    private void Scan()
    {
        scanner.ScanForEnemies(this.context, enemyScanRange);
        scanner.ScanForPickups(this.context, pickupScanRange);
        scanner.ScanForPositions(this.context, samplingRange, samplingDensity);

        TacticalMovement();
        PlayerAction();
    }

    private void TacticalMovement()
    {
        if (HasEnemies())
        {
            if (moveToPickup.Run(context) > 0 && context.nearestPickup)
            {
                Vector3 desiredPosition = context.nearestPickup.transform.position;
                playerController.desiredPositionByAI = desiredPosition;
            }

            if (reloadGun.Run(context) > 0)
            {
                player.gunController.Reload();
            }

            playerController.desiredPositionByAI = moveToBestPosition.GetBest(context);
        }
    }

    private void PlayerAction()
    {
        if (HasEnemies())
        {
            if (useHealth.Run(context) > 0)
            {
                player.UseLifePack();
            }

            if (currentBehavior == AIBehavior.ProximityAttack)
            {
                ProximityAttack();
            }
            else if (currentBehavior == AIBehavior.LowHealthPriority)
            {
                LowHealthPriorityAttack();
            }
        }
    }

    private void ProximityAttack()
    {
        float bestTargetScore = setBestAttackTarget.Run(context);
        if (bestTargetScore > 0)
        {
            player.targetEntity = context.nearestEnemy;
            player.AimAndShoot(coolDownToShoot);
        }
    }

    private void LowHealthPriorityAttack()
    {
        if (context.enemies.Count > 0)
        {
            LivingEntity target = null;
            float lowestHealth = float.MaxValue;

            foreach (var enemy in context.enemies)
            {
                if (enemy.life < lowestHealth)
                {
                    lowestHealth = enemy.life;
                    target = enemy;
                }
            }

            if (target != null)
            {
                player.targetEntity = target;
                player.AimAndShoot(coolDownToShoot);
            }
        }
    }

    public bool HasEnemies()
    {
        return (context.enemies.Count != 0);
    }
}
