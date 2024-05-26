using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {
	public static int score { get; private set; }
	float muerteDelUltimoEnemigo;
	int racha;
	public float duracionRacha = 1f;

	void Start() {
		Enemy.OnDeathStatic += OnEnemyKilled;
		FindObjectOfType<Player> ().OnDeath += OnPlayerDeath;
	}

	void OnEnemyKilled() {
		if (Time.time < muerteDelUltimoEnemigo + duracionRacha) {
            racha += 1;
		} 
		else {
            racha = 0;
		}
        muerteDelUltimoEnemigo = Time.time;
		score += 1 + 3 * racha;
	}

	void OnPlayerDeath() {
        score = 0;
		Enemy.OnDeathStatic -= OnEnemyKilled;
	}
}
