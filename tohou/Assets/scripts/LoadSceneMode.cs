using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        // Verifica se a cena de UI já está carregada para não duplicar
        if (!SceneManager.GetSceneByName("UI").isLoaded)
        {
            // Carrega a cena UI por cima da cena Game
            SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
        }
    }
}