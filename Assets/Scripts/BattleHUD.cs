using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleHUD : MonoBehaviour
{
    [Header("Player UI")]
    public Slider playerHPBar;
    public Image[] skillIcons;
    public Image[] skillCooldownOverlays;
    public TextMeshProUGUI[] skillCooldownTexts;

    [Header("Enemy UI")]
    public Slider enemyHPBar;

    public SkillManager skillManager;
    private PlayerMove player;
    private EnemyAI enemy;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
        skillManager = player.skillManager;
        enemy = GameObject.FindWithTag("Enemy").GetComponent<EnemyAI>();
    }

    void Update()
    {
        UpdatePlayerHP();
        UpdateEnemyHP();
        UpdateSkillCooldowns();
    }

    private void UpdatePlayerHP()
    {
        if (playerHPBar != null)
        {
            playerHPBar.value = player.CurrentHP / player.MaxHP;
        }
    }

    private void UpdateEnemyHP()
    {
        if (enemyHPBar != null && enemy != null)
        {
            enemyHPBar.value = enemy.CurrentHP / enemy.MaxHP;
        }
    }

    private void UpdateSkillCooldowns()
    {
        for (int i = 0; i < skillManager.skills.Length; i++)
        {
            var skill = skillManager.skills[i];
            float remaining = Mathf.Clamp(skill.cooldown - (Time.time - skill.LastUsedTime), 0, skill.cooldown);
            float fillAmount = remaining / skill.cooldown;

            if (skillCooldownOverlays[i] != null)
            {
                skillCooldownOverlays[i].fillAmount = fillAmount;
            }
            if (skillCooldownTexts[i] != null)
            {
                skillCooldownTexts[i].text = remaining > 0f ? Mathf.Ceil(remaining).ToString() : "";
            }
        }
    }
}
