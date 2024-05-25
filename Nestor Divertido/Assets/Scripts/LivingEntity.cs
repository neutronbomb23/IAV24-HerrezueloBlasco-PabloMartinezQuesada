using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour {
    bool isInvulnerable = false;
    public float startingLife;
    public float life { get; protected set; }
    protected bool dead;
    public LivingEntity targetEntity;
    public event System.Action OnDeath;

    public virtual void Start() {
        life = startingLife;
    }

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection) {
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage) {
        life -= damage;

        if (life <= 0 && !dead) {
            Die();
        }
    }

    public virtual void Die() {
        //Debug.Log("Neglo");
        dead = true;
        if (OnDeath != null) {
            OnDeath();
        }
        GameObject.Destroy(this.gameObject);
    }

    public void RefillLife() {
        life = startingLife;
    }

    public bool HasFullLife() {
        return (life == startingLife);
    }

    public float GetCurrentLifePercent() {
        return life / startingLife;
    }

    public bool GetInvulnerablilityState() {
        return isInvulnerable;
    }
}
