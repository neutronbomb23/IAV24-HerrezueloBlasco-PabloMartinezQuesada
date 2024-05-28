using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : Pickup
{
    public Gun gunToEquip;
    public override event System.Action OnCollected;

    public override void Awake()
    {
        base.Awake();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            Player playerScript = other.GetComponent<Player>();
            if (playerScript)
            {
                Gun gun = playerScript.GetComponentInChildren<Gun>();
                if (gun != null)
                {
                    gun.addCharger();
                }
            }

            if (OnCollected != null)
            {
                OnCollected();
            }
            Destroy(this.gameObject);
        }
    }
}
