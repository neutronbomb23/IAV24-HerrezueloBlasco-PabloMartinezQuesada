using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class Player : LivingEntity {
    public float moveSpeed = 5f;
    PlayerController controller;
    public GunController gunController;
    public int startingLifePacks = 2;
    public int currentLifePacks { get; private set; }
    public Vector3 spawnPoint;
    public float thresholdCursorDistanceSquared = 1f;

    public bool isInvincible = false;
    [Range(0f, 5f)]
    public float invincibilityCoolDown = 1.5f;

    public event System.Action<int> OnChangeLPValue;

    private NavMeshAgent agent;

    public override void Start() {
        base.Start();
        if (OnChangeLPValue != null) { OnChangeLPValue(currentLifePacks); }
    }

    void Awake() {
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>() as GunController;
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
        currentLifePacks = startingLifePacks;
        spawnPoint = this.transform.position;
        agent = this.GetComponent<NavMeshAgent>();
        agent.speed = this.moveSpeed;
    }

    public override void TakeDamage(float damage) {
        //Debug.Log(life);
        if (!isInvincible) {
            life -= damage;
            if (life >= 0 && !dead) {
                InvincibilityTimer(invincibilityCoolDown);
            }
        }

        if (life <= 0 && !dead) {
            Die();
        }
    }

    void InvincibilityTimer(float coolDownInSecs) {
        isInvincible = true;
        Invoke("EndInvincibility", coolDownInSecs);
    }

    void EndInvincibility() {
        isInvincible = false;
    }

    public void AddLifePack() {
        this.currentLifePacks++;
        if (OnChangeLPValue != null) { OnChangeLPValue(currentLifePacks); }
    }

    public void UseLifePack() {
        if (currentLifePacks > 0) {
            if (startingLife != life) {
                currentLifePacks--;
                if (OnChangeLPValue != null) { OnChangeLPValue(currentLifePacks); }
                RefillLife();
            }
        }
    }

    void OnNewWave(int waveNumber) {
        life = startingLife;
        gunController.EquipGun(0);
    }

    void Update()
    {
        controller.MoveAgent();
        if (this.targetEntity)
        {
            controller.LookAt(this.targetEntity.transform.position);
        }
    }

    public override void Die()
    {
        base.Die();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool HasAnyBulletInCharger()
    {
        return this.gunController ? gunController.HasAnyBulletInCharger() : false;
    }

    public bool HasAnyChargers() {
        return this.gunController ? gunController.HasAnyChargers() : false;
    }

    public void Shoot() {
        this.gunController.OnTriggerHold();
        this.gunController.OnTriggerRelease();
    }

    public void AimAndShoot(float coolDownToShoot) {
        Invoke("Shoot", coolDownToShoot);
    }
}