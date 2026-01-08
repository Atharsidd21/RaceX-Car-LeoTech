using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerProfileView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private Image avatarImage;

    public void Refresh(
        string playerName,
        int coins,
        Sprite avatarSprite
    )
    {
        if (nameText != null)
            nameText.text = playerName;

        if (coinsText != null)
            coinsText.text = coins.ToString();

        if (avatarImage != null)
            avatarImage.sprite = avatarSprite;
    }
}
