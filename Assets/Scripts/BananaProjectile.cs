using UnityEngine;

// Shared projectile for enemy and player banana shots.
public class BananaProjectile : MonoBehaviour
{
    public int damage = 20;
    public float lifetime = 5f;
    public float hitRadius = 1f;

    bool damagesPlayer;
    bool damagesEnemy;
    bool hasHit;
    PlayerHealth targetPlayer;

    public void Configure(int projectileDamage, bool shouldDamagePlayer, bool shouldDamageEnemy)
    {
        damage = projectileDamage;
        damagesPlayer = shouldDamagePlayer;
        damagesEnemy = shouldDamageEnemy;
    }

    public void SetPlayerTarget(PlayerHealth player)
    {
        targetPlayer = player;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (damagesPlayer && targetPlayer == null)
            targetPlayer = FindFirstObjectByType<PlayerHealth>();

        if (damagesPlayer && targetPlayer != null && !hasHit)
        {
            if (Vector3.Distance(transform.position, targetPlayer.transform.position) <= hitRadius)
                DamagePlayer(targetPlayer);
        }

        if (damagesPlayer && !hasHit)
        {
            foreach (Collider hitCollider in Physics.OverlapSphere(transform.position, hitRadius))
            {
                PlayerHealth player = hitCollider.GetComponentInParent<PlayerHealth>();
                if (player != null)
                {
                    DamagePlayer(player);
                    break;
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.collider);
    }

    void OnTriggerEnter(Collider other)
    {
        HandleHit(other);
    }

    void HandleHit(Collider other)
    {
        if (hasHit)
            return;

        if (damagesPlayer)
        {
            PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
            if (player != null)
            {
                DamagePlayer(player);
                return;
            }
        }

        if (damagesEnemy)
        {
            EnemyNpc enemy = other.GetComponentInParent<EnemyNpc>();
            if (enemy != null)
            {
                hasHit = true;
                enemy.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }

        // Any other solid object stops the projectile.
        if (!other.isTrigger)
            Destroy(gameObject);
    }

    void DamagePlayer(PlayerHealth player)
    {
        if (hasHit)
            return;

        hasHit = true;
        player.TakeDamage(damage);
        Destroy(gameObject);
    }
}
