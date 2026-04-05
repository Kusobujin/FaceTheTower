using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class lobbyUI : MonoBehaviour
{
    public TextMeshProUGUI mageText;
    public TextMeshProUGUI summonText;

    public TextMeshProUGUI floorText;

    void Start()
    {
        mageText.text = "Chosen Mage: " + GameData.Instance.selectedMage;
        summonText.text = "Chosen Summon: " + GameData.Instance.selectedSummon;
        if (GameData.Instance.isBossBattle)
    {
        floorText.text = "Current Floor: " + GameData.Instance.currentFloor + "F-BOSS";
    }
    else
    {
        floorText.text = "Current Floor: " + GameData.Instance.currentFloor + "F-" + GameData.Instance.currentBattleInFloor;
    }
    }

    public void GoToBattle()
    {
        SceneManager.LoadScene("battle");
    }
}