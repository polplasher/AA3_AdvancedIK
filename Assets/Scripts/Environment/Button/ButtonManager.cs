using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    private Button[] buttons;

    private void Start()
    {
        buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            button.OnPressed += () => StartCoroutine(HandleButtonPressed());
        }
    }

    private IEnumerator HandleButtonPressed()
    {
        foreach (Button button in buttons)
        {
            if (!button.IsPressed)
            {
                yield break;
            }
        }

        yield return new WaitForSeconds(2f);

        // Go to next level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void OnDisable()
    {
        foreach (Button button in buttons)
        {
            button.OnPressed -= () => StartCoroutine(HandleButtonPressed());
        }
    }
}