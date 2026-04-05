using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public string selectedMage;
    public string selectedSummon;

    public int currentFloor = 1;
    public int currentBattleInFloor = 1;

    public bool isBossBattle = false;

    [Header("Debug Start")]
    public bool useDebugStartFloor = false;
    public int debugStartFloor = 1;
    public int debugStartBattle = 1;
    public bool debugStartBossBattle = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}