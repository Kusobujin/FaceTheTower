using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class summonSelectUI : MonoBehaviour
{
    public TextMeshProUGUI chosenMageText;

    void Start()
    {
        chosenMageText.text = "Chosen Mage: " + GameData.Instance.selectedMage;
    }

    public void GoBack()
    {
        SceneManager.LoadScene("mageSelect");
    }

    public void SelectSummon(string summonName)
    {
        GameData.Instance.selectedSummon = summonName;
        
        if (GameData.Instance.useDebugStartFloor)
    {
        GameData.Instance.currentFloor = GameData.Instance.debugStartFloor;
        GameData.Instance.currentBattleInFloor = GameData.Instance.debugStartBattle;
        GameData.Instance.isBossBattle = GameData.Instance.debugStartBossBattle;
    }
    else
    {
        GameData.Instance.currentFloor = 1;
        GameData.Instance.currentBattleInFloor = 1;
        GameData.Instance.isBossBattle = false;
    }

        SceneManager.LoadScene("lobby");
    }
}