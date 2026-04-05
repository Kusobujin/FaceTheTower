using UnityEngine;
using TMPro;

public class battleUI : MonoBehaviour
{
    public TextMeshProUGUI mageText;
    public TextMeshProUGUI summonText;
    public TextMeshProUGUI summonHPText;

    public TextMeshProUGUI enemy1Text;
    public TextMeshProUGUI enemy2Text;

    public TextMeshProUGUI battleLogText;

    public TextMeshProUGUI floorText;

    public battleSystem system;

    void Start()
    {
        mageText.text = "Mage: " + GameData.Instance.selectedMage;
        summonText.text = "Summon:\n" + GameData.Instance.selectedSummon;

        if (GameData.Instance.isBossBattle)
        {
            floorText.text = GameData.Instance.currentFloor + "F-BOSS";
        }
        else
        {
            floorText.text = GameData.Instance.currentFloor + "F-" + GameData.Instance.currentBattleInFloor;
        }

        RefreshUI();

        if (GameData.Instance.isBossBattle)
        {
            battleLogText.text = "A boss blocks your path!";
        }
        else
        {
            battleLogText.text = "A battle has started!";
        }
    }

    public void RefreshUI()
    {
        summonHPText.text = "HP: " + system.summonHP + "/" + system.summonMaxHP;

        if (GameData.Instance.isBossBattle)
        {
            enemy1Text.text = "BOSS\n" + system.enemy1Data.enemyName + "\nHP: " + system.enemy1HP;

            if (system.enemy1ThornsActive)
                enemy1Text.text += "\n[THORNS]";
        }
        else
        {
            enemy1Text.text = system.enemy1Data.enemyName + "\nHP: " + system.enemy1HP;

            if (system.enemy1ThornsActive)
                enemy1Text.text += "\n[THORNS]";
        }

        if (system.enemyCount == 2 && system.enemy2Data != null)
        {
            enemy2Text.text = system.enemy2Data.enemyName + "\nHP: " + system.enemy2HP;

            if (system.enemy2ThornsActive)
                enemy2Text.text += "\n[THORNS]";
        }
        else
        {
            enemy2Text.text = "";
        }
    }

    public void Attack()
    {
        if (system.battleEnded)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("lobby");
            return;
        }

        system.choosingTarget = true;
        battleLogText.text = "Choose a target!";
    }

    public void TargetEnemy1()
    {
        if (!system.choosingTarget) return;
        if (system.enemy1HP <= 0) return;

        system.enemy1HP -= system.playerDamage;
        if (system.enemy1HP < 0) system.enemy1HP = 0;

        string log = "You dealt " + system.playerDamage + " damage to " + system.enemy1Data.enemyName + "!";

        if (system.enemy1ThornsActive)
        {
            system.summonHP -= system.enemy1Data.skillPower;
            if (system.summonHP < 0) system.summonHP = 0;

            log += "\nThorns reflected " + system.enemy1Data.skillPower + " damage!";
        }

        RefreshUI();
        battleLogText.text = log;
        system.choosingTarget = false;

        CheckVictoryOrEnemyTurn();
    }

    public void TargetEnemy2()
    {
        if (!system.choosingTarget) return;
        if (system.enemyCount < 2) return;
        if (system.enemy2HP <= 0) return;

        system.enemy2HP -= system.playerDamage;
        if (system.enemy2HP < 0) system.enemy2HP = 0;

        string log = "You dealt " + system.playerDamage + " damage to " + system.enemy2Data.enemyName + "!";

        if (system.enemy2ThornsActive)
        {
            system.summonHP -= system.enemy2Data.skillPower;
            if (system.summonHP < 0) system.summonHP = 0;

            log += "\nThorns reflected " + system.enemy2Data.skillPower + " damage!";
        }

        RefreshUI();
        battleLogText.text = log;
        system.choosingTarget = false;

        CheckVictoryOrEnemyTurn();
    }

    void CheckVictoryOrEnemyTurn()
    {
        bool enemy1Dead = system.enemy1HP <= 0;
        bool enemy2Dead = system.enemyCount == 1 || system.enemy2HP <= 0;

        if (system.summonHP <= 0)
        {
            system.battleEnded = true;
            RefreshUI();
            battleLogText.text = "Your summon was defeated... Press ATTACK to return.";
            return;
        }

        if (enemy1Dead && enemy2Dead)
        {
            system.battleEnded = true;
            AdvanceTowerProgress();
            RefreshUI();
            battleLogText.text = "Victory! All enemies were defeated.";
            return;
        }

        EnemyTurn();
    }

    void EnemyTurn()
    {
        int totalEnemyDamage = 0;
        string log = "";

        // ENEMY 1
        if (system.enemy1Data != null && system.enemy1HP > 0)
        {
            bool usedSkill = system.TryUseEnemySkill(1);

            if (usedSkill)
            {
                switch (system.enemy1Data.skillType)
                {
                    case EnemySkillType.Heal:
                        log += system.enemy1Data.enemyName + " used Heal!\n";
                        break;

                    case EnemySkillType.Thorns:
                        log += system.enemy1Data.enemyName + " activated Thorns!\n";
                        break;

                    case EnemySkillType.Summon:
                        log += system.enemy1Data.enemyName + " summoned an ally!\n";
                        break;
                }
            }
            else
            {
                totalEnemyDamage += system.enemy1Data.damage;
                log += system.enemy1Data.enemyName + " attacked for " + system.enemy1Data.damage + " damage!\n";
            }
        }

        // ENEMY 2
        if (system.enemyCount == 2 && system.enemy2Data != null && system.enemy2HP > 0)
        {
            bool usedSkill = system.TryUseEnemySkill(2);

            if (usedSkill)
            {
                switch (system.enemy2Data.skillType)
                {
                    case EnemySkillType.Heal:
                        log += system.enemy2Data.enemyName + " used Heal!\n";
                        break;

                    case EnemySkillType.Thorns:
                        log += system.enemy2Data.enemyName + " activated Thorns!\n";
                        break;

                    case EnemySkillType.Summon:
                        log += system.enemy2Data.enemyName + " summoned an ally!\n";
                        break;
                }
            }
            else
            {
                totalEnemyDamage += system.enemy2Data.damage;
                log += system.enemy2Data.enemyName + " attacked for " + system.enemy2Data.damage + " damage!\n";
            }
        }

        system.summonHP -= totalEnemyDamage;
        if (system.summonHP < 0) system.summonHP = 0;

        system.EndEnemyTurnUpdates();

        RefreshUI();

        if (system.summonHP <= 0)
        {
            system.battleEnded = true;

            if (log == "")
                log = "Your summon was defeated... Press ATTACK to return.";
            else
                log += "\nYour summon was defeated... Press ATTACK to return.";

            battleLogText.text = log.Trim();
        }
        else
        {
            if (totalEnemyDamage > 0)
                log += "\nYour summon took " + totalEnemyDamage + " total damage.";

            battleLogText.text = log.Trim();
        }
    }

    void AdvanceTowerProgress()
    {
        if (!GameData.Instance.isBossBattle)
        {
            if (GameData.Instance.currentFloor % 3 == 0 && GameData.Instance.currentBattleInFloor == 3)
            {
                GameData.Instance.isBossBattle = true;
            }
            else
            {
                GameData.Instance.currentBattleInFloor++;

                if (GameData.Instance.currentBattleInFloor > 3)
                {
                    GameData.Instance.currentBattleInFloor = 1;
                    GameData.Instance.currentFloor++;
                }
            }
        }
        else
        {
            GameData.Instance.isBossBattle = false;
            GameData.Instance.currentFloor++;
            GameData.Instance.currentBattleInFloor = 1;
        }
    }
}