using System.Collections;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomSpawner : MonoBehaviour
{
    public int openingDirection;
    private RoomTemplates roomTemplates;
    private int rand;
    private bool spawned = false;

    // Static counter to track the number of spawned presets
    private static int totalSpawned = 0;
    private const int maxPresets = 6;

    private void Start()
    {
        roomTemplates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        Invoke("Spawn", 0.1f);
    }

    void Spawn()
    {
        if (!spawned && totalSpawned < maxPresets)
        {
            if (totalSpawned == maxPresets - 1)
            {
                // Spawn the last preset as the third preset
                Instantiate(roomTemplates.presets[2], transform.position, Quaternion.identity);

                Invoke("LoadNextLevel", 1f);
            }
            else
            {
                if (openingDirection == 1)
                {
                    // Spawning from the right side
                    rand = Random.Range(0, roomTemplates.presets.Length);
                    Instantiate(roomTemplates.presets[rand], transform.position, Quaternion.identity);
                }
            }
            totalSpawned++;
            spawned = true;
        }
        else if (totalSpawned >= maxPresets)
        {
            Destroy(gameObject); // Prevent further spawns
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SpawnPoint"))
        {
            if (other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
            {
                Destroy(gameObject);
            }
            spawned = true;
        }
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
