using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GiveUpButtonHandler : MonoBehaviour
{
    public void OnGiveUpButtonClick()
    {
        SceneManager.LoadScene(0); // Load the scene with the index of 0 (your menu scene)
    }
}
