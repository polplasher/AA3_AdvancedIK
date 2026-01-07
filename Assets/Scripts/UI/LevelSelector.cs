using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private int level;

    public void SelectLevel()
    {
        SceneManager.LoadScene("Level" + level.ToString());
    }
}
