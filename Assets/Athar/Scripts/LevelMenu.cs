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
            Debug.Log($"Level {item.levelBuildIndex} Opened = " +
          PlayerPrefs.GetInt($"LevelOpened_{item.levelBuildIndex}", 0));

            bool isOpened = PlayerPrefs.GetInt($"LevelOpened_{item.levelBuildIndex}", 0) == 1;

            // Clear old listeners
            item.levelButton.onClick.RemoveAllListeners();

            // DEFAULT STATE
            item.levelButton.interactable = false;

            if (!isOpened)
            {
                // 🔒 Still locked → show lock
                if (item.lockButton != null)
                    item.lockButton.gameObject.SetActive(true);
            }
            else
            {
                // ✅ Level unlocked → remove lock & enable level
                if (item.lockButton != null)
                    Destroy(item.lockButton.gameObject);

                item.levelButton.interactable = true;

                int indexCopy = item.levelBuildIndex;
                item.levelButton.onClick.AddListener(() => OpenLevel(indexCopy));
            }
        }
    }


    /*  void UnlockLevel(int levelIndex)
      {
          // Mark level as opened
          PlayerPrefs.SetInt($"LevelOpened_{levelIndex}", 1);
          PlayerPrefs.Save();

          // Refresh UI
          SetupLevels();
      }*/

    public void OpenLevel(int levelIndex)
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.FadeOutAndStop();

        SceneManager.LoadScene(levelIndex);
    }
}