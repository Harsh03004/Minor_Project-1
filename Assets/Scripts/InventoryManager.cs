using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    public bool menuActived;

    [SerializeField]
    private List<ItemSlot> itemSlots = new List<ItemSlot>(); // Dynamic list of item slots

    public GameObject itemSlotPrefab; // Prefab for creating new slots
    public Transform inventoryParent; // Parent object for arranging slots

    public ItemSO[] itemSOs;

    void Start()
    {
        // Inventory starts empty, no predefined slots
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && menuActived)
        {
            Time.timeScale = 1.0f;
            InventoryMenu.SetActive(false);
            menuActived = false;
            Debug.Log("Menu not active");
        }
        else if (Input.GetKeyDown(KeyCode.I) && !menuActived)
        {
            Time.timeScale = 0.0f;
            InventoryMenu.SetActive(true);
            menuActived = true;
            Debug.Log("Menu is active");
        }
    }

    public bool UseItem(string itemName)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.itemName == itemName && slot.thisItemSelected)
            {
                Debug.Log($"Using item: {itemName} from slot");
                return slot.UseItem(); // Ensure this is called only once
            }
        }
        return false;
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        // Find the ItemSO for the item being added
        ItemSO correspondingItemSO = itemSOs.FirstOrDefault(item => item.itemName == itemName);

        foreach (var slot in itemSlots)
        {
            if (slot.itemName == itemName && !slot.isFull)
            {
                int leftover = slot.AddItem(itemName, quantity, itemSprite, itemDescription);
                if (leftover > 0)
                {
                    return AddItem(itemName, leftover, itemSprite, itemDescription);
                }
                return 0; // Successfully added
            }
        }

        // Create a new slot if no suitable slot exists
        GameObject newSlot = Instantiate(itemSlotPrefab, inventoryParent);
        ItemSlot slotScript = newSlot.GetComponent<ItemSlot>();

        slotScript.itemName = itemName;
        slotScript.itemSprite = itemSprite;
        slotScript.itemDescription = itemDescription;

        int leftovers = slotScript.AddItem(itemName, quantity, itemSprite, itemDescription);
        itemSlots.Add(slotScript);

        if (leftovers > 0)
        {
            return AddItem(itemName, leftovers, itemSprite, itemDescription);
        }
        return 0;
    }

    public void RemoveSlot(ItemSlot slotToRemove)
    {
        // Remove the slot from the list
        itemSlots.Remove(slotToRemove);

        // Destroy the slot GameObject
        Destroy(slotToRemove.gameObject);

        Debug.Log("Slot removed from inventory.");
    }

    public void DeselectAllSlots()
    {
        foreach (var slot in itemSlots)
        {
            slot.selectedShader.SetActive(false);
            slot.thisItemSelected = false;
        }
    }
}
