using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager instance { get; private set; }

    [Header("Data")]
    public int coin = 1965487;

    [Header("References")]
    [SerializeField] TextMeshProUGUI XPText; // Reflects skill points
    [SerializeField] TextMeshProUGUI CoinText;

    [SerializeField]
    public TextMeshProUGUI SkillPoints;

    private playerattack playerAttackScript; // Reference to the playerattack script

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of EconomyManager exists
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Get the playerattack script
        playerAttackScript = FindObjectOfType<playerattack>();

        // Initialize with current values
        AddCoin(0); // Update coin display

        if (playerAttackScript != null)
        {
            UpdateXPText(playerAttackScript.skillPoints); // Initialize XPText to show skill points
        }
    }

    private void Update()
    {
        // Continuously update the XPText with the skill points
        if (playerAttackScript != null && XPText != null)
        {
            UpdateXPText(playerAttackScript.skillPoints);
        }
    }

    public void AddCoin(int amount)
    {
        coin += amount;
        if (CoinText != null)
        {
            CoinText.SetText(coin.ToString());
        }
    }

    public void UpdateXPText(int skillPoints)
    {
        // Update the XPText to show the current skill points
        if (XPText != null)
        {
            XPText.SetText(skillPoints.ToString());
            SkillPoints.SetText(skillPoints.ToString());
        }
    }
}
