using UnityEngine;

// Handles weapon slots: 1 is rifle and 2 is pistol.
public class WeaponSwitcher : MonoBehaviour
{
    [Header("Weapon Slots")]
    public GameObject rifle;
    public GameObject pistol;
    public WeaponHUD hud;

    [Header("Held Rifle Pose")]
    public Vector3 rifleHoldPosition = new Vector3(-0.35f, -0.25f, 0.7f);
    public Vector3 rifleHoldRotation = Vector3.zero;

    public int SelectedSlot { get; private set; } = 2;
    bool rifleCollected;

    void Start()
    {
        if (hud == null)
            hud = GetComponent<WeaponHUD>();
        SelectPistol();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && rifleCollected)
            SelectRifle();
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectPistol();
    }

    public void CollectRifle()
    {
        rifleCollected = true;
        if (rifle != null)
        {
            rifle.SetActive(true);
            rifle.transform.SetParent(pistol != null ? pistol.transform.parent : transform, false);
            rifle.transform.localPosition = rifleHoldPosition;
            rifle.transform.localRotation = Quaternion.Euler(rifleHoldRotation);
            Rifle rifleScript = rifle.GetComponent<Rifle>();
            if (rifleScript != null)
                rifleScript.enabled = true;

            foreach (Collider rifleCollider in rifle.GetComponentsInChildren<Collider>())
                rifleCollider.enabled = false;
        }
        SelectRifle();
    }

    public void SelectRifle()
    {
        if (!rifleCollected || rifle == null)
            return;
        SelectedSlot = 1;
        rifle.SetActive(true);
        rifle.transform.localPosition = rifleHoldPosition;
        rifle.transform.localRotation = Quaternion.Euler(rifleHoldRotation);
        Rifle rifleScript = rifle.GetComponent<Rifle>();
        if (rifleScript != null)
            rifleScript.enabled = true;
        if (pistol != null) pistol.SetActive(false);
        hud?.SetSelected(1);
    }

    public void SelectPistol()
    {
        SelectedSlot = 2;
        if (rifle != null && rifleCollected)
            rifle.SetActive(false);
        if (pistol != null) pistol.SetActive(true);
        hud?.SetSelected(2);
    }
}
