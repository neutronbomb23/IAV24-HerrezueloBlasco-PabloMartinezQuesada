using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour {
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

    void Awake() {
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

    void Start() {
        InvokeRepeating("Scan", scanTimeIntervalInSecs, scanTimeIntervalInSecs);
    }

    private void Scan() {
        scanner.ScanForEnemies(this.context, enemyScanRange);
        scanner.ScanForPickups(this.context, pickupScanRange);
        scanner.ScanForPositions(this.context, samplingRange, samplingDensity);

        TacticalMovement();
        PlayerAction();
    }

    private void TacticalMovement() {
        if (HasEnemies()) {
            if (moveToPickup.Run(context) > 0 && context.nearestPickup) {
                Vector3 desiredPosition = context.nearestPickup.transform.position;
                playerController.desiredPositionByAI = desiredPosition;
            }

            if (reloadGun.Run(context) > 0) {
                player.gunController.Reload();
            }

            playerController.desiredPositionByAI = moveToBestPosition.GetBest(context);
        }
    }

    private void PlayerAction() {
        if (HasEnemies()) { // Usar vida
            if (useHealth.Run(context) > 0) {
                player.UseLifePack();
            }

            float bestTargetScore = setBestAttackTarget.Run(context); // Moverse
            if (bestTargetScore > 0) {
                player.targetEntity = context.nearestEnemy;
                player.AimAndShoot(coolDownToShoot);
            }
        }
    }

    public bool HasEnemies() {
        return (context.enemies.Count != 0);
    }
}