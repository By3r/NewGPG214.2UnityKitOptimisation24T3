using Gamekit3D;
using UnityEngine;

namespace Dana
{
    /// <summary>
    /// Responsible for checking whether the player can pick-up  the staff or not.
    /// </summary>
    
    public class MeleeUnlockable : MonoBehaviour
    {
        #region Variables
        public GameObject staffPickup;
        [SerializeField] private int interactedWithWeapon = 0;
        #endregion

        private void Start()
        {
            TogglePickupScript(false);
            if (PlayerDataManager.instance == null)
            {
                return;
            }

            PlayerDataManager.instance.UpdateWeaponPickupState();
            PlayerDataManager.instance.RegisterWeaponPickup(this);

        }

        public void TogglePickupScript(bool isEnabled)
        {
            if (staffPickup != null)
            {
                staffPickup.SetActive(isEnabled);
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Player") && interactedWithWeapon == 0)
            {
                PlayerController playerController = collider.GetComponent<PlayerController>();
                if (staffPickup.activeSelf == true)
                {
                    playerController.SetCanAttack(true);
                    interactedWithWeapon = 5;

                    PlayerDataManager.instance.RecordSwordUnlockEvent();
                }
            }
        }

    }
}