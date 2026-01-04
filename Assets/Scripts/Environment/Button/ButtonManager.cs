using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    private Button[] buttons;

    private void Start()
    {
        buttons = GetComponentsInChildren<Button>(true);

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
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }

        SceneManager.LoadScene(nextSceneIndex);
    }

    private void OnDisable()
    {
        foreach (Button button in buttons)
        {
            button.OnPressed -= () => StartCoroutine(HandleButtonPressed());
        }
    }
}