using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public void ShowGameOver()
    {
        if (panel != null)
            panel.SetActive(true);
        else
            gameObject.SetActive(true);
    }

    public void HideGameOver()
    {
        if (panel != null)
            panel.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    public void OnRetryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnExitButton()
    {
        #if UNITY_EDITOR
                // only exists inside editor
                UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_ANDROID || UNITY_IOS
                // optional small delay before quit for mobile
                StartCoroutine(QuitAfterDelay(0.2f));
        #else
                Application.Quit();
        #endif
            }

#if UNITY_ANDROID || UNITY_IOS
    private System.Collections.IEnumerator QuitAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Application.Quit();
    }
#endif
}
