using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AddRoom : MonoBehaviour
{
    private RoomTemplates templates;

    private void Start()
    {
        templates=GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        templates.Generatedpreset.Add(this.gameObject);
    }
}
