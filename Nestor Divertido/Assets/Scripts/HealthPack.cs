using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : Pickup {
    public override event System.Action OnCollected;
    
    void OnTriggerStay(Collider other) {
		if (other.tag == "Player") {
            Player player = other.GetComponent<Player>();
            player.AddLifePack();
            Destroy(Instantiate(pickupEffect.gameObject, this.transform.position, Quaternion.Euler(new Vector3(90, 0, 0))), pickupEffect.main.duration);

            if (OnCollected != null) {           
				OnCollected();
			}
			Destroy(this.gameObject);
		}
	}
}
