using UnityEngine;
using UnityEngine.SceneManagement;

public class LuckyEggHuntLobby : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void BtnContinue()
    {
        SceneManager.LoadScene("MagicCauldron");
    }
}
