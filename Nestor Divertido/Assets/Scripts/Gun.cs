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

    public Transform shell;
    public Transform shellEjection;

    MuzzleFlash muzzleFlash;

    float nextShotTime;
    public int currentChargers { get; private set; }

    bool triggerReleasedSinceLastShot;
    int shotsRemainingInBurst;
    public int projectilesRemainingInMag { get; private set; }
    bool isReloading;

    Vector3 recoilSmoothDampVelocity;
    float recoilRotSmoothDampVelocity;
    float recoilAngle;

    private void Start() {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;

        projectilesRemainingInMag = projectilesXCharger;
        currentChargers = initialChargers;
        tryUpdateAmmoUI();
    }

    private void LateUpdate() {
        this.transform.localPosition = Vector3.SmoothDamp(this.transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
        this.transform.localEulerAngles = Vector3.left * recoilAngle;

        if (!isReloading && projectilesRemainingInMag == 0 && !isOutOfBullets()) {
            Reload();
        }
    }

    void Shoot() {
        if (!isReloading && Time.time > nextShotTime && projectilesRemainingInMag > 0 && !isOutOfBullets()) {
            if (!triggerReleasedSinceLastShot) { return; }

            // Spawning shot(s)
            for (int i = 0; i < projectileSpawn.Length; i++) {
                if (projectilesRemainingInMag == 0) { break; }
                projectilesRemainingInMag--;
                tryUpdateAmmoUI();
                nextShotTime = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(projectileVelocity);
            }
            // Shell ejection
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
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

        if (!isReloading && projectilesRemainingInMag != projectilesXCharger) {
            currentChargers--;
            isReloading = true;
            InvokeRepeating("AnimateReloadStep", 0f, 0.02f);  // Llamar a AnimateReloadStep cada 0.02 segundos
            Invoke("FinishReload", reloadTime);  // Completar la recarga despuÃ©s de reloadTime segundos
        }
    }

    private void AnimateReloadStep() {
        float reloadSpeed = 1f / reloadTime;
        float percent = (Time.time % reloadTime) * reloadSpeed;
        Vector3 initialRot = this.transform.localEulerAngles;
        float maxReloadAngle = 30f;
        float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
        float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
        this.transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;
    }

    private void FinishReload() {
        CancelInvoke("AnimateReloadStep");
        isReloading = false;
        projectilesRemainingInMag = projectilesXCharger;
        tryUpdateAmmoUI();
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

    public bool isOutOfBullets() {
        return (currentChargers == 0 && projectilesRemainingInMag == 0);
    }

    public void Aim(Vector3 aimPoint) {
        if (!isReloading) {
            this.transform.LookAt(aimPoint);
        }
    }

    public void OnTriggerHold() {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease() {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }
}