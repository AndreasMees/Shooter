using UnityEngine;

// Fires a banana projectile from the camera centre.
public class Pistol : MonoBehaviour
{
    [Header("References")]
    public Camera aimCamera;
    public ParticleSystem muzzleFlash;
    public GameObject bananaPrefab;
    public AudioSource audioSource;
    public AudioClip fireSound;

    [Header("Weapon")]
    public int damage = 25;
    public float shotsPerSecond = 4f;
    public float range = 100f;
    public float bananaSpeed = 30f;
    public int magazineSize = 8;
    public float reloadTime = 1.2f;

    int ammo;
    float nextShotTime;
    bool isReloading;

    void Awake()
    {
        if (aimCamera == null)
            aimCamera = Camera.main;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        ammo = magazineSize;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            StartReload();

        if (Input.GetMouseButton(0) && Time.time >= nextShotTime && !isReloading)
            Fire();
    }

    void Fire()
    {
        if (ammo <= 0)
        {
            StartReload();
            return;
        }

        ammo--;
        nextShotTime = Time.time + 1f / Mathf.Max(shotsPerSecond, 0.01f);
        if (muzzleFlash != null)
            muzzleFlash.Play();
        if (audioSource != null && fireSound != null)
            audioSource.PlayOneShot(fireSound);
        Debug.Log($"Pistol fired. Ammo: {ammo}/{magazineSize}");

        if (aimCamera == null || bananaPrefab == null)
        {
            Debug.LogWarning("Pistol needs both Aim Camera and Banana Prefab references.");
            return;
        }

        Vector3 spawnPosition = aimCamera.transform.position + aimCamera.transform.forward * 0.7f;
        GameObject banana = Instantiate(bananaPrefab, spawnPosition, aimCamera.transform.rotation);
        BananaProjectile projectile = banana.GetComponent<BananaProjectile>();
        if (projectile == null)
            projectile = banana.AddComponent<BananaProjectile>();
        projectile.Configure(damage, false, true);

        Rigidbody body = banana.GetComponent<Rigidbody>();
        if (body == null)
            body = banana.AddComponent<Rigidbody>();

        body.useGravity = false;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        body.linearVelocity = aimCamera.transform.forward * bananaSpeed;

        foreach (Collider bananaCollider in banana.GetComponentsInChildren<Collider>())
        {
            foreach (Collider playerCollider in transform.root.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(bananaCollider, playerCollider);
            }
        }
    }

    void StartReload()
    {
        if (!isReloading && ammo < magazineSize)
            StartCoroutine(Reload());
    }

    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        ammo = magazineSize;
        isReloading = false;
        Debug.Log("Pistol reloaded.");
    }
}
