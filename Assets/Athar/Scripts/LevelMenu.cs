using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    [System.Serializable]
    public class LevelItem
    {
        public int levelBuildIndex;
        public Button levelButton;
        public Button lockButton;
    }

    public LevelItem[] levels;

    private void Start()
    {
        SetupLevels();
    }

    void SetupLevels()
    {
        foreach (LevelItem item in levels)
        {
            bool canUnlock = PlayerPrefs.GetInt($"LevelUnlocked_{item.levelBuildIndex}", 0) == 1;
            bool isOpened = PlayerPrefs.GetInt($"LevelOpened_{item.levelBuildIndex}", 0) == 1;

            // Remove old listeners
            item.levelButton.onClick.RemoveAllListeners();
            item.lockButton.onClick.RemoveAllListeners();

            // DEFAULT STATE
            item.levelButton.interactable = false;
            item.lockButton.interactable = false;

            if (!isOpened)
            {
                // 🔒 Still locked
                item.lockButton.gameObject.SetActive(true);

                if (canUnlock)
                {
                    // 🔓 Lock button is now clickable
                    item.lockButton.interactable = true;

                    int indexCopy = item.levelBuildIndex;
                    item.lockButton.onClick.AddListener(() => UnlockLevel(indexCopy));
                }
            }
            else
            {
                // ✅ Level already opened
                item.lockButton.gameObject.SetActive(false);
                item.levelButton.interactable = true;

                int indexCopy = item.levelBuildIndex;
                item.levelButton.onClick.AddListener(() => OpenLevel(indexCopy));
            }
        }
    }

    void UnlockLevel(int levelIndex)
    {
        // Mark level as opened
        PlayerPrefs.SetInt($"LevelOpened_{levelIndex}", 1);
        PlayerPrefs.Save();

        // Refresh UI
        SetupLevels();
    }

    public void OpenLevel(int levelIndex)
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.FadeOutAndStop();

        SceneManager.LoadScene(levelIndex);
    }
}
