using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [SerializeField] GameObject mainCanvas;      // Reference to the main canvas
    [SerializeField] GameObject backgroundCanvas; // Reference to the background canvas
    bool menuActivated = false;

    private void Start()
    {
        // Ensure that the canvases and skill tree menu are disabled at the start
        mainCanvas.SetActive(false);
        backgroundCanvas.SetActive(false);
    }

    private void Update()
    {
        // Listen for the "S" key press to activate the skill tree menu and canvases
        if (Input.GetKeyDown(KeyCode.S) && !menuActivated)
        {
            menuActivated = true;
            OpenSkillTreeMenu(); 
        }

        else if(Input.GetKeyDown(KeyCode.S) && menuActivated)
        {
            menuActivated = false;
            OpenSkillTreeMenu();
        }
    }

    // Method to open the skill tree menu and activate canvases
    void OpenSkillTreeMenu()
    {
        // Activate the main canvas and background canvas
        if (menuActivated)
        {
            mainCanvas.SetActive(true);
            backgroundCanvas.SetActive(true);
        }

        else
        {
            mainCanvas.SetActive(false);
            backgroundCanvas.SetActive(false);
        }
    }
}
