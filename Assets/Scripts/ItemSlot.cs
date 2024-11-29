using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Unity.VisualScripting;
using System.Linq;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    // Item data
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public Sprite emptySprite;
    public bool isFull;
    public string itemDescription;

    // Item Slot UI elements
    [SerializeField]
    private TMP_Text quantityText;

    [SerializeField]
    private Image itemImage;

    public GameObject selectedShader;
    public bool thisItemSelected;

    //Reference to invetory manager script
    private InventoryManager inventoryManager;

    //Item Description
    public Image itemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;

    private bool isBeingUsed = false; // Prevent double execution

    [SerializeField]
    private int maxNumberOfItems;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        if (isFull)
        {
            return quantity;
        }

        // Set item data
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        itemImage.sprite = itemSprite;
        itemImage.enabled = true;
        this.itemDescription = itemDescription;

        // Calculate new quantity
        this.quantity += quantity;
        if (this.quantity >= maxNumberOfItems)
        {
            int extraItems = this.quantity - maxNumberOfItems;
            this.quantity = maxNumberOfItems;
            isFull = true;

            // Update UI display
            quantityText.text = this.quantity.ToString();
            quantityText.enabled = true;

            return extraItems; // Return leftover items
        }

        // Update UI if no overflow
        quantityText.text = this.quantity.ToString();
        quantityText.enabled = true;
        return 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log($"Item clicked: {itemName}");
            OnLeftClick(inventoryManager);
        }
    }

public InventoryManager GetInventoryManager()
    {
        return inventoryManager;
    }

    public void OnLeftClick(InventoryManager inventoryManager)
    {
        if (!thisItemSelected) // Only process the click if the item is not already selected
        {
            inventoryManager.DeselectAllSlots();

            selectedShader.SetActive(true);
            thisItemSelected = true;

            ItemDescriptionNameText.text = itemName;
            ItemDescriptionText.text = itemDescription;
            itemDescriptionImage.sprite = itemSprite;
        }
        else
        {
            // If already selected, use the item
            UseItem(); // This should happen only once
        }
    }


    public void EmptySlot()
    {
        // Clear the UI
        quantityText.enabled = false;
        itemImage.sprite = emptySprite;
        itemImage.enabled = false;

        // Reset the slot's data
        itemName = null;
        quantity = 0;
        isFull = false;
        itemDescription = null;

        // Notify the InventoryManager to remove the slot
        inventoryManager.RemoveSlot(this);
    }




    public bool UseItem()
    {
        if (isBeingUsed) return false; // Skip if already processing
        isBeingUsed = true;

        if (thisItemSelected)
        {
            ItemSO correspondingItemSO = inventoryManager.itemSOs.FirstOrDefault(item => item.itemName == itemName);
            if (correspondingItemSO != null)
            {
                bool usable = correspondingItemSO.UseItem();
                if (usable)
                {
                    quantity -= 1;
                    quantityText.text = quantity > 0 ? quantity.ToString() : "";

                    if (quantity <= 0)
                    {
                        EmptySlot();
                    }
                    isBeingUsed = false; // Reset flag
                    return true;
                }
            }
        }

        isBeingUsed = false; // Reset flag
        return false;
    }


}
