using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour{
    private UI UI;

    public Transform[] projectileSpawn;
    public Projectile projectile;
    public float msBetweenShots = 0.0000010f;
    public float projectileVelocity = 35f;
    public int burstCount = 3;
    public int projectilesXCharger;
    public float reloadTime = .3f;
    public int initialChargers = 2;

    public Vector2 kickMinMax = new Vector2(.05f, .2f);
    public Vector2 recoilAngleMinMax = new Vector2(3f, 5f);
    public float recoilMoveSettleTime = .1f;
    public float recoilRotationSettleTime = .1f;

    MuzzleFlash muzzleFlash;

    float nextShotTime;
    public int currentChargers { get; private set; }

    bool triggerReleasedSinceLastShot;
    public int projectilesRemainingInMag { get; private set; }

    Vector3 recoilSmoothDampVelocity;
    float recoilRotSmoothDampVelocity;
    float recoilAngle;

    private void Start() {
        muzzleFlash = GetComponent<MuzzleFlash>();

        projectilesRemainingInMag = projectilesXCharger;
        currentChargers = initialChargers;
        tryUpdateAmmoUI();
    }

    private void LateUpdate() {
        this.transform.localPosition = Vector3.SmoothDamp(this.transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
        this.transform.localEulerAngles = Vector3.left * recoilAngle;

        if  (projectilesRemainingInMag == 0 && !isOutOfBullets()) {
            Reload();
        }
    }

    void Shoot() {
        //Debug.Log("PiumPium");
        //Debug.Log(isOutOfBullets());
        //Debug.Log(nextShotTime);
        //Debug.Log(projectilesRemainingInMag);
        if (Time.time > nextShotTime && projectilesRemainingInMag > 0 && !isOutOfBullets()) {
            if (!triggerReleasedSinceLastShot) { return; }

            // Spawning shot(s)
            for (int i = 0; i < projectileSpawn.Length; i++) {
                if (projectilesRemainingInMag == 0) { break; }
                projectilesRemainingInMag--;
                tryUpdateAmmoUI();
                nextShotTime = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = Instantiate(projectile, this.transform.position, this.transform.rotation) as Projectile;
                newProjectile.SetSpeed(projectileVelocity);
            }
            muzzleFlash.Activate();
            // Recoil
            this.transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);
        }
    }

    public void Reload() {
        if (isOutOfBullets()) {
            return;
        }

        if (projectilesRemainingInMag <= 0 && currentChargers > 0) {
            currentChargers--;
            projectilesRemainingInMag = projectilesXCharger;
            tryUpdateAmmoUI();
        }
    }

    public void registerGameUI(UI UI) {
        this.UI = UI;
    }

    private void tryUpdateAmmoUI() {
        if (UI) {
            UI.ClipCountUI.text = currentChargers.ToString("D2");
            UI.AmmoCountUI.text = projectilesRemainingInMag.ToString("D2");
        }
    }

    public void addCharger()
    {
        currentChargers++;
        tryUpdateAmmoUI();
    }
    public bool isOutOfBullets() {
        return (currentChargers == 0 && projectilesRemainingInMag == 0);
    }

    public void Aim(Vector3 aimPoint) {
       
            this.transform.LookAt(aimPoint);
        
    }

    public void OnTriggerHold() {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease() {
        triggerReleasedSinceLastShot = true;
    }
}