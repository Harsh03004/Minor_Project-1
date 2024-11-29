    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu]
    public class ItemSO : ScriptableObject
    {
        public string itemName;
        public StatToChange statToChange = new StatToChange();
        public int amountToChangeStat;

        public AttributeToChange attributeToChange = new AttributeToChange();
        public int amountToChangeAttribute;

        public bool UseItem()
    {
        Debug.Log($"Using ItemSO: {itemName}");
        if (statToChange == StatToChange.health)
        {
            Playerhealth playerHealth = GameObject.FindObjectOfType<Playerhealth>();
            if (playerHealth != null)
            {
                if (playerHealth.IsAtMaxHealth())
                {
                    Debug.Log($"Cannot use {itemName}. Health is at maximum.");
                    return false;
                }
                else
                {
                    playerHealth.ModifyHealth(amountToChangeStat);
                    Debug.Log($"{itemName} used. Health increased by {amountToChangeStat}.");
                    return true;
                }
            }
        }
        if (statToChange == StatToChange.EXP)
            {
                playerattack playerAttack = GameObject.FindObjectOfType<playerattack>();

                if (playerAttack != null)
                {
                    Debug.Log($"Adding {amountToChangeStat} experience...");
                    playerAttack.ModifyExperience(amountToChangeStat);
                    Debug.Log($"{itemName} used. Experience increased by {amountToChangeStat}.");
                    return true;
                }
                else
                {
                    Debug.LogWarning("playerattack script not found in the scene.");
                }
            }

            if(statToChange==StatToChange.Attack)
            {
                playerattack specialAttack=GameObject.FindObjectOfType<playerattack>();

                if (specialAttack != null)
                {
                    Debug.Log($"Damage {amountToChangeStat} increase for special attack");
                    specialAttack.specialattack = amountToChangeStat;
                    Debug.Log($"{itemName} used.Special Attack Power Increased by {amountToChangeStat}.");
                    return true;
                }

                else
                {
                    Debug.LogWarning("playerattack script not found in the scene.");
                }
            }

            return false;
        }


        public enum StatToChange
        {
            none,
            health,
            EXP,
            Attack
        };

        public enum AttributeToChange
        {
            none,
            strength,
            defense,
            intelligence,
            agility
        };
    }
