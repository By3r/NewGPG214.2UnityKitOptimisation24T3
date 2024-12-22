using UnityEngine;

namespace Dana
{
    /// <summary>
    /// Handles Buttons that switch between panels or quit application in main menu.
    /// Does not handle any complex save/load functions.
    /// </summary>

    public class MainMenuButtonsHandler : MonoBehaviour
    {
        #region Variables
        [Tooltip("Assign the panel/gameobject responsible for handling new sign ups in this slot.")]
        [SerializeField] private GameObject newUserPanel;
        [Tooltip("Assign the panel/gameobject responsible for handling existing users in this slot.")]
        [SerializeField] private GameObject existingUserPanel;
        #endregion

        #region Public Functions.
        public void ClearPanelsAndShowMainMenuScreen()
        {
            newUserPanel.SetActive(false);
            existingUserPanel.SetActive(false);
        }

        public void ShowNewUserPanel()
        {
            newUserPanel.SetActive(true);
            existingUserPanel.SetActive(false);
        }

        public void ShowExistingUserPanel()
        {
            existingUserPanel.SetActive(true);
            newUserPanel.SetActive(false);
        }

        public void ToggleBetwwenNewUserAndExistingUserPanel()
        {
            newUserPanel.SetActive(!newUserPanel.activeSelf);
            existingUserPanel.SetActive(!existingUserPanel.activeSelf);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
        #endregion
    }
}
