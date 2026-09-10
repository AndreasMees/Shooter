using UnityEngine;

// Automatic rifle that fires the shared banana projectile at a faster rate.
public class Rifle : MonoBehaviour
{
    [Header("References")]
    public Camera aimCamera;
    public GameObject bananaPrefab;
    public AudioSource audioSource;
    public AudioClip fireSound;
    public ParticleSystem muzzleFlash;

    [Header("Weapon")]
    public int damage = 15;
    public float shotsPerSecond = 10f;
    public float bananaSpeed = 35f;
    public int magazineSize = 30;
    public float reloadTime = 1.5f;

    int ammo;
    float nextShotTime;
    bool isReloading;

    void Awake()
    {
        ResolveReferences();
        ammo = magazineSize;
    }

    void OnEnable()
    {
        ResolveReferences();
    }

    void ResolveReferences()
    {
        if (aimCamera == null)
            aimCamera = Camera.main;

        if (aimCamera == null)
            aimCamera = GetComponentInParent<Camera>();

        if (bananaPrefab == null)
        {
            Pistol pistol = GetComponentInParent<Pistol>();
            if (pistol == null)
                pistol = FindFirstObjectByType<Pistol>();
            if (pistol != null)
                bananaPrefab = pistol.bananaPrefab;
        }

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
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
        ResolveReferences();

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

        if (aimCamera == null)
        {
            Debug.LogWarning("Rifle needs an Aim Camera reference.");
            return;
        }

        if (bananaPrefab == null)
        {
            Debug.LogWarning("Rifle needs a Banana Prefab reference.");
            return;
        }

        GameObject banana = Instantiate(
            bananaPrefab,
            aimCamera.transform.position + aimCamera.transform.forward * 0.7f,
            aimCamera.transform.rotation);

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
                Physics.IgnoreCollision(bananaCollider, playerCollider);
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
    }
}
