using UnityEngine;

// Put this on a world rifle pickup with a trigger collider.
public class RiflePickup : MonoBehaviour
{
    public WeaponSwitcher weaponSwitcher;
    public float pickupRadius = 1.5f;

    bool pickedUp;

    void Awake()
    {
        Rifle rifle = GetComponent<Rifle>();
        if (rifle != null)
            rifle.enabled = false;

        Collider pickupCollider = GetComponent<Collider>();
        if (pickupCollider != null)
            pickupCollider.isTrigger = true;
    }

    void Update()
    {
        if (pickedUp)
            return;

        WeaponSwitcher switcher = FindFirstObjectByType<WeaponSwitcher>();
        if (switcher != null && Vector3.Distance(transform.position, switcher.transform.position) <= pickupRadius)
            PickUp(switcher);
    }

    void OnTriggerEnter(Collider other)
    {
        WeaponSwitcher switcher = weaponSwitcher;
        if (switcher == null)
            switcher = other.GetComponentInParent<WeaponSwitcher>();

        if (switcher != null)
            PickUp(switcher);
    }

    void PickUp(WeaponSwitcher switcher)
    {
        if (pickedUp)
            return;

        pickedUp = true;
        switcher.CollectRifle();

        if (switcher.rifle == gameObject)
            Destroy(this);
        else
            Destroy(gameObject);
    }
}
