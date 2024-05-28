using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : Pickup {
    public override event System.Action OnCollected;
    
    void OnTriggerStay(Collider other) {
		if (other.tag == "Player") {
            Player player = other.GetComponent<Player>();
            player.AddLifePack();
           
            if (OnCollected != null) {           
				OnCollected();
			}
			Destroy(this.gameObject);
		}
	}
}
