using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : Pickup {
    public Gun gunToEquip;
    public override event System.Action OnCollected;
    
    public override void Awake()
    {
        base.Awake();
    }

    void OnTriggerStay(Collider other) {
        if (other.tag == "Player") {
            Player playerScript = other.GetComponent<Player>();
            if (playerScript) {
                playerScript.gunController.EquipGun(gunToEquip);
            }

            if (OnCollected != null)
            {
                OnCollected();
            }
            Destroy(this.gameObject);
        }
    }
}
