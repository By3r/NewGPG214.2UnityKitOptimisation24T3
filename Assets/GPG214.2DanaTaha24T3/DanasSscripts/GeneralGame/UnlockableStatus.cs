using UnityEngine;

namespace Dana
{
    [System.Serializable]
    public class UnlockableStatus
    {
        [Tooltip("Do not manipulate")]
        public bool isUnlocked;

        public UnlockableStatus(bool isUnlocked)
        {
            this.isUnlocked = isUnlocked;
        }
    }
}
