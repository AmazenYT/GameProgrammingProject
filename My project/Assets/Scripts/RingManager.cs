using UnityEngine;
using TMPro;

public class RingManager : MonoBehaviour
{
    public MB_GameManager gameManager;
    public TextMeshProUGUI ringText;

    void Update()
    {
        if (gameManager == null || ringText == null) return;

        ringText.text = "Rings: " + gameManager.gameStatus.ringsCollected;
    }
}