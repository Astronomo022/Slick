using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;


public class MainMenuController : MonoBehaviour
{
    private InputAction m_MoveAction;
    private InputAction m_JumpAction;
    private InputAction m_DashAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_MoveAction = InputSystem.actions.FindAction("Player/Move");
        m_JumpAction = InputSystem.actions.FindAction("Player/Jump");
        m_DashAction = InputSystem.actions.FindAction("Player/Dash");
        m_MoveAction.Enable();
        m_JumpAction.Enable();
        m_DashAction.Enable();

        StartCoroutine(WaitForKeyPress());
    }

    private IEnumerator WaitForKeyPress()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second

        // Wait for any input (keyboard or gamepad)
        /*while (!Keyboard.current.wasUpdatedThisFrame && !Gamepad.current.wasUpdatedThisFrame)
        {
            yield return null;
        }*/
        while (true)
        {
            if (m_MoveAction.triggered || m_JumpAction.triggered || m_DashAction.triggered)
            {
                break; // Exit the loop when any of the actions are triggered
            }
            yield return null; // Wait for the next frame
        }

        // Assuming "Logo" and "ContinueText" are GameObjects in the Canvas
        GameObject logo = GameObject.Find("Logo");
        GameObject continueText = GameObject.Find("ContinueText");

        if (logo != null)
        {
            // Perform actions on the logo, e.g., enable or change color
            logo.GetComponent<LogoAnimation>().StartCoroutine("SlideOutToLeft");
        }

        if (continueText != null)
        {
            // Perform actions on the continue text, e.g., enable or change color
            //continueText.SetActive(true);
        }

        // Load the "Scramble" scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scramble");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
