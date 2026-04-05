using Unity.VisualScripting;
using UnityEngine;

public class battleSystem : MonoBehaviour
{
    public int summonHP;
    public int summonMaxHP = 50;

    public int enemyCount;

    public enemyData[] possibleEnemies;

    public enemyData enemy1Data;
    public enemyData enemy2Data;

    public enemyData bossEnemyData;

    public int enemy1HP;
    public int enemy2HP;
    public int bossHP;

    public int enemy1SkillCooldownCurrent;
    public int enemy2SkillCooldownCurrent;

    public bool enemy1ThornsActive;
    public bool enemy2ThornsActive;

    public int enemy1ThornsTurnsLeft;
    public int enemy2ThornsTurnsLeft;

    public int playerDamage = 10;

    public SummonSkillType summonSkillType;
    public int summonSkillPower;
    public int summonSkillCooldownMax;
    public int summonSkillCooldownCurrent;

    public bool usingSummonSkill = false;

    public bool choosingTarget = false;
    public bool battleEnded = false;

    enemyData GetRandomEnemyForCurrentFloor()
    {
        int floor = GameData.Instance.currentFloor;

        var validEnemies = new System.Collections.Generic.List<enemyData>();

        foreach (enemyData enemy in possibleEnemies)
        {
            if (floor >= enemy.minFloor && floor <= enemy.maxFloor)
            {
                validEnemies.Add(enemy);
            }
        }

        if (validEnemies.Count == 0)
        {
            Debug.LogWarning("No valid enemies found for floor " + floor);
            return possibleEnemies[0];
        }

        return validEnemies[Random.Range(0, validEnemies.Count)];
    }

    enemyData GetBossForCurrentFloor()
    {
        int floor = GameData.Instance.currentFloor;

        foreach (enemyData enemy in possibleEnemies)
        {
            if (enemy.enemyName == "Boss")
            {
                return enemy;
            }
        }

        Debug.LogWarning("No boss found! Using first enemy as fallback.");
        return possibleEnemies[0];
    }

        void SetupSummonSkill()
    {
        switch (GameData.Instance.selectedSummon)
        {
            case "Wolf":
                summonSkillType = SummonSkillType.PowerStrike;
                summonSkillPower = 20;
                summonSkillCooldownMax = 2;
                break;

            case "Fairy":
                summonSkillType = SummonSkillType.PowerStrike;
                summonSkillPower = 18;
                summonSkillCooldownMax = 2;
                break;

            case "Golem":
                summonSkillType = SummonSkillType.PowerStrike;
                summonSkillPower = 25;
                summonSkillCooldownMax = 3;
                break;

            default:
                summonSkillType = SummonSkillType.PowerStrike;
                summonSkillPower = 20;
                summonSkillCooldownMax = 2;
                break;
        }

        summonSkillCooldownCurrent = 0;
        usingSummonSkill = false;
    }

    private void Awake()
    {
        summonHP = summonMaxHP;
        SetupSummonSkill();

        enemy1SkillCooldownCurrent = 0;
        enemy2SkillCooldownCurrent = 0;

        enemy1ThornsActive = false;
        enemy2ThornsActive = false;

        enemy1ThornsTurnsLeft = 0;
        enemy2ThornsTurnsLeft = 0;

        if (GameData.Instance.isBossBattle)
        {
            enemyCount = 1;

            bossHP = Random.Range(bossEnemyData.minHP, bossEnemyData.maxHP + 1);

            enemy1Data = bossEnemyData;
            enemy1HP = bossHP;

            enemy2Data = null;
            enemy2HP = 0;
        }
        else
        {
            enemyCount = Random.Range(1, 3);

            enemy1Data = GetRandomEnemyForCurrentFloor();
            enemy1HP = Random.Range(enemy1Data.minHP, enemy1Data.maxHP + 1);

            if (enemyCount == 2)
            {
                enemy2Data = GetRandomEnemyForCurrentFloor();
                enemy2HP = Random.Range(enemy2Data.minHP, enemy2Data.maxHP + 1);
            }
            else
            {
                enemy2Data = null;
                enemy2HP = 0;
            }
        }
    }

    public int getTotalEnemyDamage()
    {
        int totalDamage = 0;

        if (enemy1HP > 0 && enemy1Data != null)
            totalDamage += enemy1Data.damage;

        if (enemyCount == 2 && enemy2HP > 0 && enemy2Data != null)
            totalDamage += enemy2Data.damage;

        return totalDamage;
    }

    public bool TryUseEnemySkill(int enemyIndex)
    {
        enemyData data = null;
        int currentHP = 0;
        int currentCooldown = 0;
        bool thornsActive = false;

        if (enemyIndex == 1)
        {
            data = enemy1Data;
            currentHP = enemy1HP;
            currentCooldown = enemy1SkillCooldownCurrent;
            thornsActive = enemy1ThornsActive;
        }
        else if (enemyIndex == 2)
        {
            data = enemy2Data;
            currentHP = enemy2HP;
            currentCooldown = enemy2SkillCooldownCurrent;
            thornsActive = enemy2ThornsActive;
        }

        if (data == null || currentHP <= 0)
            return false;

        if (data.skillType == EnemySkillType.None)
            return false;

        if (currentCooldown > 0)
            return false;

        int roll = Random.Range(1, 101);
        if (roll > data.skillChance)
            return false;

        switch (data.skillType)
        {
            case EnemySkillType.Heal:
                int maxHP = data.maxHP;
                if (currentHP >= maxHP)
                    return false;

                HealEnemy(enemyIndex, data.skillPower);
                SetEnemySkillCooldown(enemyIndex, data.skillCooldown);
                Debug.Log(data.enemyName + " used Heal!");
                return true;

            case EnemySkillType.Thorns:
                if (thornsActive)
                    return false;

                ActivateThorns(enemyIndex, data.skillDuration);
                SetEnemySkillCooldown(enemyIndex, data.skillCooldown);
                Debug.Log(data.enemyName + " used Thorns!");
                return true;

            case EnemySkillType.Summon:
                if (enemyCount >= 2)
                    return false;

                SummonExtraEnemy();
                SetEnemySkillCooldown(enemyIndex, data.skillCooldown);
                Debug.Log(data.enemyName + " used Summon!");
                return true;
        }

        return false;
    }

    void SetEnemySkillCooldown(int enemyIndex, int value)
    {
        if (enemyIndex == 1)
            enemy1SkillCooldownCurrent = value;
        else if (enemyIndex == 2)
            enemy2SkillCooldownCurrent = value;
    }

    void HealEnemy(int enemyIndex, int amount)
    {
        if (enemyIndex == 1 && enemy1Data != null)
        {
            enemy1HP += amount;
            int max = enemy1Data.maxHP;
            if (enemy1HP > max)
                enemy1HP = max;
        }
        else if (enemyIndex == 2 && enemy2Data != null)
        {
            enemy2HP += amount;
            int max = enemy2Data.maxHP;
            if (enemy2HP > max)
                enemy2HP = max;
        }
    }

    void ActivateThorns(int enemyIndex, int duration)
    {
        if (enemyIndex == 1)
        {
            enemy1ThornsActive = true;
            enemy1ThornsTurnsLeft = duration;
        }
        else if (enemyIndex == 2)
        {
            enemy2ThornsActive = true;
            enemy2ThornsTurnsLeft = duration;
        }
    }

    void SummonExtraEnemy()
    {
        if (enemyCount >= 2)
            return;

        enemy2Data = GetRandomEnemyForCurrentFloor();
        enemy2HP = Random.Range(enemy2Data.minHP, enemy2Data.maxHP + 1);
        enemyCount = 2;

        enemy2SkillCooldownCurrent = 0;
        enemy2ThornsActive = false;
        enemy2ThornsTurnsLeft = 0;
    }

        public void EndPlayerTurnUpdates()
    {
        if (summonSkillCooldownCurrent > 0)
            summonSkillCooldownCurrent--;
    }

    public void EndEnemyTurnUpdates()
    {
        if (enemy1SkillCooldownCurrent > 0)
            enemy1SkillCooldownCurrent--;

        if (enemy2SkillCooldownCurrent > 0)
            enemy2SkillCooldownCurrent--;

        if (enemy1ThornsActive)
        {
            enemy1ThornsTurnsLeft--;

            if (enemy1ThornsTurnsLeft <= 0)
            {
                enemy1ThornsActive = false;
                enemy1ThornsTurnsLeft = 0;
            }
        }

        if (enemy2ThornsActive)
        {
            enemy2ThornsTurnsLeft--;

            if (enemy2ThornsTurnsLeft <= 0)
            {
                enemy2ThornsActive = false;
                enemy2ThornsTurnsLeft = 0;
            }
        }
    }
    
    public enum SummonSkillType
{
    None,
    PowerStrike
}
}