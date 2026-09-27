using UnityEngine;
using UnityEngine.SceneManagement; // ESSA LINHA É CRUCIAL

public class MenuGameOver : MonoBehaviour
{
    // Função pública para ser chamada pelo botão
public void ReiniciarCena()
{
    // Garante que o tempo volte a correr normalmente antes de carregar a cena
    Time.timeScale = 1f; 
    
    // Carrega a cena atual de novo
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}
}