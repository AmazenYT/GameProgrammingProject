using UnityEngine;
using TMPro;

public class LivesUI : MonoBehaviour
{
    public PlayerLivesSO playerLives;
    public TextMeshProUGUI livesText;

    void Update()
    {
      livesText.text = "Lives: " + playerLives.currentLives;
    }
}