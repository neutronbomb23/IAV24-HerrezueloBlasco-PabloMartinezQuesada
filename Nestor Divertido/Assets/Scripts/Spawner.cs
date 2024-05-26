using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public Wave[] waves;
    public Enemy enemy;
    public Color initialTileColor = Color.white;
    private int enemySpawnedCount = 0;

    LivingEntity playerEntity;
    Transform playerT;

    public HealthPack hp;
    public float hpSpawnTime = 3;
    public int hpCapAmmount = 3;

    private int hpAccumulated = 0;
    private float hpNextSpawnTime;

    public GunPickup gunPickupToRespawn;
    public Vector2 gunsSpawnTime = new Vector2(3f, 6f);
    public int gunsCapAmmount = 3;

    private int gunsAccumulated = 0;
    private float gunsNextSpawnTime;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;
    int enemiesRemainingAlive;
    float enemyNextSpawnTime;

    MapGenerator map;

    public event System.Action<int> OnNewWave;

    private void Start() {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    void Update() {
        // Genera enemigos según el tiempo especificado
        if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > enemyNextSpawnTime) {
            enemiesRemainingToSpawn--;
            enemySpawnedCount++;
            enemyNextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
            SpawnEnemy();
        }

        // Genera botiquines si hay menos del límite especificado
        if ((hpAccumulated < hpCapAmmount) && (hpNextSpawnTime < Time.time)) {
            hpAccumulated++;
            hpNextSpawnTime = Time.time + hpSpawnTime;
            SpawnHP();
        }

        // Genera armas si hay menos del límite especificado
        if ((gunsAccumulated < gunsCapAmmount) && (gunsNextSpawnTime < Time.time)) {
            gunsAccumulated++;
            gunsNextSpawnTime = Time.time + Random.Range(gunsSpawnTime.x, gunsSpawnTime.y);
            SpawnRandomGun();
        }
    }

    // Genera un arma en una posición aleatoria
    void SpawnRandomGun() {
        Transform spawnTile = map.GetRandomOpenTile();
        GunPickup spawnedGun = Instantiate(gunPickupToRespawn, spawnTile.position + Vector3.up, Quaternion.identity) as GunPickup;
        spawnedGun.OnCollected += this.OnGunCollected;
    }

    // Genera un botiquín en una posición aleatoria
    void SpawnHP() {
        Transform spawnTile = map.GetRandomOpenTile();
        HealthPack spawnedHP = Instantiate(hp, spawnTile.position + Vector3.up, Quaternion.identity) as HealthPack;
        spawnedHP.OnCollected += this.OnHPCollected;
    }

    // Genera un enemigo con un retraso de aparición
    void SpawnEnemy() {
        float spawnDelay = 1;
        Invoke("InstantiateEnemy", spawnDelay);
    }

    // Instancia al enemigo en el tile seleccionado
    void InstantiateEnemy() {
        Transform spawnTile = map.GetRandomOpenTile();
        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.name = "Fede_" + enemySpawnedCount.ToString();
        spawnedEnemy.OnDeath += OnEnemyDeath;
        spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);
    }

    // Maneja la recolección de armas
    void OnGunCollected() {
        if (gunsAccumulated > 0) {
            gunsAccumulated--;
        }
    }

    // Maneja la recolección de botiquines
    void OnHPCollected() {
        if (hpAccumulated > 0) {
            hpAccumulated--;
        }
    }

    // Maneja el evento de muerte del enemigo
    void OnEnemyDeath() {
        if (--enemiesRemainingAlive == 0) {
            NextWave();
        }
    }

    // Reinicia la posición del jugador
    void ResetPlayerPosition() {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }

    // Inicia la siguiente oleada de enemigos
    void NextWave() {
        currentWaveNumber += 1;
        if (currentWaveNumber - 1 < waves.Length) {
            //print("Oliada " + currentWaveNumber);
            currentWave = waves[currentWaveNumber - 1];
            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;

            if (OnNewWave != null) {
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();
        }
    }

    [System.Serializable]
    public class Wave {
        public bool infinite;
        public int enemyCount;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColor;
    }
}