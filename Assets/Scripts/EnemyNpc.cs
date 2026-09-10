using UnityEngine;

// A wandering enemy that faces the player and periodically throws a banana.
public class EnemyNpc : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;

    [Header("Throwing")]
    public GameObject bananaPrefab;
    public Transform throwPoint;
    public float throwInterval = 3f;
    public float bananaSpeed = 20f;
    public AudioSource audioSource;
    public AudioClip throwSound;

    [Header("Movement")]
    public bool moveAround = true;
    public float moveSpeed = 2f;
    public float movementRadius = 5f;
    public float pauseAtDestination = 1f;

    EnemyHealthBar healthBar;

    float health;
    float nextThrowTime;
    Transform player;
    PlayerHealth playerHealth;
    Vector3 homePosition;
    Vector3 movementTarget;
    float nextMovementTime;

    void Start()
    {
        health = maxHealth;
        healthBar = GetComponent<EnemyHealthBar>();
        if (healthBar == null)
            healthBar = gameObject.AddComponent<EnemyHealthBar>();
        healthBar.SetHealth(health, maxHealth);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
            player = playerHealth.transform;
        else
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }

        if (playerHealth == null)
            Debug.LogWarning("EnemyNpc could not find a PlayerHealth component.");

        homePosition = transform.position;
        ChooseMovementTarget();

        GameManager.Instance?.RegisterNpc(this);
        nextThrowTime = Time.time + throwInterval;
    }

    void Update()
    {
        MoveAround();

        if (player != null)
        {
            Vector3 lookDirection = player.position - transform.position;
            lookDirection.y = 0f;
            if (lookDirection.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        if (player != null && Time.time >= nextThrowTime)
        {
            ThrowBanana();
            nextThrowTime = Time.time + throwInterval;
        }
    }

    void MoveAround()
    {
        if (!moveAround)
            return;

        Vector3 flatTarget = new Vector3(movementTarget.x, transform.position.y, movementTarget.z);
        transform.position = Vector3.MoveTowards(transform.position, flatTarget, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, flatTarget) < 0.05f)
        {
            if (Time.time >= nextMovementTime)
                ChooseMovementTarget();
        }
    }

    void ChooseMovementTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * movementRadius;
        movementTarget = homePosition + new Vector3(randomOffset.x, 0f, randomOffset.y);
        nextMovementTime = Time.time + pauseAtDestination;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar?.SetHealth(health, maxHealth);
        Debug.Log($"NPC hit for {damage} damage. Health: {Mathf.Max(health, 0f)}/{maxHealth}");

        if (health <= 0f)
        {
            GameManager.Instance?.NpcKilled(this);
            Destroy(gameObject);
        }
    }

    void ThrowBanana()
    {
        if (bananaPrefab == null)
            return;

        Transform origin = throwPoint != null ? throwPoint : transform;
        GameObject banana = Instantiate(bananaPrefab, origin.position, Quaternion.identity);
        Rigidbody body = banana.GetComponent<Rigidbody>();
        if (body == null)
            body = banana.AddComponent<Rigidbody>();

        BananaProjectile projectile = banana.GetComponent<BananaProjectile>();
        if (projectile == null)
            projectile = banana.AddComponent<BananaProjectile>();
        projectile.Configure(projectile.damage, true, false);
        projectile.SetPlayerTarget(playerHealth);
        if (audioSource != null && throwSound != null)
            audioSource.PlayOneShot(throwSound);

        Vector3 target = player.position + Vector3.up * 0.9f;
        Vector3 direction = (target - origin.position).normalized;
        body.useGravity = false;
        body.isKinematic = false;
        body.detectCollisions = true;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        body.linearVelocity = direction * bananaSpeed;

        foreach (Collider bananaCollider in banana.GetComponentsInChildren<Collider>())
        {
            foreach (Collider enemyCollider in GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(bananaCollider, enemyCollider);
        }
    }
}
