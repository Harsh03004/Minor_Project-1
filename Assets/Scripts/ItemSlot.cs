using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Unity.VisualScripting;

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
        if(eventData.button==PointerEventData.InputButton.Left)
        {
            OnLeftClick(GetInventoryManager());
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnRightClick()
    {
       //Dropping the item
       GameObject itemToDrop =new GameObject(itemName);
        Item newItem=itemToDrop.AddComponent<Item>();
        newItem.name = itemName;
        newItem.quantity = 1;
        newItem.sprite= itemSprite;
        newItem.itemDescription= itemDescription;

        SpriteRenderer sr=itemToDrop.AddComponent<SpriteRenderer>();
        sr.sprite = itemSprite;
        sr.sortingOrder = 5;
        sr.sortingLayerName = "Ground";

        itemToDrop.AddComponent<BoxCollider2D>();

        itemToDrop.transform.position=GameObject.FindWithTag("Player").transform.position + new Vector3(1.0f,0,0);


        //remove the item from the inventory
        this.quantity -= 1;
        quantityText.text = this.quantity.ToString();
        if (this.quantity <= 0)
        {
            EmptySlot();
        }
    }

    public InventoryManager GetInventoryManager()
    {
        return inventoryManager;
    }

    public void OnLeftClick(InventoryManager inventoryManager)
    {
        if (thisItemSelected)
        {
            bool usable= inventoryManager.UseItem(itemName);
            if (usable)
            {
                this.quantity -= 1;
                quantityText.text = this.quantity.ToString();
                if (this.quantity <= 0)
                {
                    EmptySlot();
                }
            }
        }
        else
        {
            inventoryManager.DeselectAllSlots();
            selectedShader.SetActive(true);
            thisItemSelected = true;
            ItemDescriptionNameText.text = itemName;
            ItemDescriptionText.text = itemDescription;
            itemDescriptionImage.sprite = itemSprite;
        }
    }

    private void EmptySlot()
    {
        //Clearing the slot
        quantityText.enabled = false;
        itemImage.sprite = emptySprite;
        
        //Clearing the item description
        ItemDescriptionNameText.text = "";
        ItemDescriptionText.text = "";
        itemDescriptionImage.sprite = emptySprite;
    }
}
