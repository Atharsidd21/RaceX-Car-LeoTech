using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

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
            bool isOpened =
                PlayerPrefs.GetInt($"LevelOpened_{item.levelBuildIndex}", 0) == 1;

            // Clear old listeners
            item.levelButton.onClick.RemoveAllListeners();
            item.levelButton.interactable = false;

            if (!isOpened)
            {
                // 🔒 Locked
                if (item.lockButton != null)
                    item.lockButton.gameObject.SetActive(true);
            }
            else
            {
                // ✅ Unlocked
                if (item.lockButton != null)
                    Destroy(item.lockButton.gameObject);

                item.levelButton.interactable = true;

                int indexCopy = item.levelBuildIndex;
                item.levelButton.onClick.AddListener(() => OpenLevel(indexCopy));
            }
        }
    }

    // ================= LEVEL LOAD =================

    public void OpenLevel(int levelIndex)
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.FadeOutAndStop();

        // ---------- MULTIPLAYER ----------
        if (GameModeManager.IsMultiplayer)
        {
            // ❗ Only MASTER loads scene
       
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.LoadLevel(levelIndex);
                }
            }
            else
            {
                SceneManager.LoadScene(levelIndex);
            }
        }
    }
}
