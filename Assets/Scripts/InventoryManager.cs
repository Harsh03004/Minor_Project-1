using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    public bool menuActived;

    [SerializeField]
    public ItemSlot [] itemSlot;

    public ItemSO[] itemSOs;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && menuActived)
        {
            //Time.timeScale = 1.0f;
            InventoryMenu.SetActive(false);
            menuActived = false;
            Debug.Log("Menu not active");
        }

        else if (Input.GetKeyDown(KeyCode.I) && !menuActived)
        {
            //Time.timeScale = 0.0f;
            InventoryMenu.SetActive(true);
            menuActived = true;
            Debug.Log("Menu is active");
        }
    }

    public bool UseItem(string itemName)
    {
        for (int i=0; i < itemSOs.Length; i++){
            if (itemSOs[i].itemName == itemName)
            {
               bool usable= itemSOs[i].UseItem();
                return usable;
            }
        }
        return false;
    }

    //Adding an item to inventory
    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            // If the slot already has the same item and isn't full, add to it
            if (itemSlot[i].itemName == itemName && !itemSlot[i].isFull)
            {
                int leftOverItems = itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription);

                // If there are leftovers, add them to the next slot recursively
                if (leftOverItems > 0)
                {
                    return AddItem(itemName, leftOverItems, itemSprite, itemDescription);
                }
                return 0; // Successfully added all items
            }
        }

        // If no matching slot was found, add to the next empty slot
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (!itemSlot[i].isFull && itemSlot[i].quantity == 0)
            {
                int leftOverItems = itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription);
                if (leftOverItems > 0)
                {
                    return AddItem(itemName, leftOverItems, itemSprite, itemDescription);
                }
                return 0; // All items added successfully
            }
        }

        // If inventory is full and couldn't add items, return the remaining quantity
        return quantity;
    }


    //Deselect the current slot
    public void DeselectAllSlots()
    {
        for(int i=0;i<itemSlot.Length; i++)
        {
            itemSlot[i].selectedShader.SetActive(false);
            itemSlot[i].thisItemSelected = false;
        }
    }
}
