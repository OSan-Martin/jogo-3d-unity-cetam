using UnityEngine;
using UnityEngine.SceneManagement; // ESSENCIAL para trocar de cena

public class MenuDePause : MonoBehaviour
{
    [Header("Paineis do Menu")]
    public GameObject painelPrincipal; // O painel com os botões Resumo, Reiniciar, etc.
    public GameObject painelControles;  // O painel que vai mostrar os controles

    private bool estaPausado = false;

    void Start()
    {
        // Garante que ambos os paineis comecem desligados e o tempo correndo.
        painelPrincipal.SetActive(false);
        painelControles.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Se o painel de controles estiver ativo, o Esc vai fechá-lo primeiro.
            if (painelControles.activeSelf)
            {
                EsconderControles();
            }
            else
            {
                // Se não, ele pausa ou resume o jogo.
                if (estaPausado)
                {
                    Resumir();
                }
                else
                {
                    Pausar();
                }
            }
        }
    }

    // --- Funções de Controle Geral ---

    public void Resumir()
    {
        painelPrincipal.SetActive(false);
        Time.timeScale = 1f; // O tempo volta a correr
        Cursor.lockState = CursorLockMode.Locked; // Trava o cursor
        Cursor.visible = false;                   // Esconde o cursor
        estaPausado = false;
    }

    void Pausar()
    {
        painelPrincipal.SetActive(true);
        Time.timeScale = 0f; // Congela o tempo
        Cursor.lockState = CursorLockMode.None;   // Libera o cursor
        Cursor.visible = true;                    // Mostra o cursor
        estaPausado = true;
    }

    // --- Funções dos Botões ---

    public void MostrarControles()
    {
        painelPrincipal.SetActive(false);
        painelControles.SetActive(true);
    }

    public void EsconderControles()
    {
        painelControles.SetActive(false);
        painelPrincipal.SetActive(true);
    }
    
    public void ReiniciarCena()
    {
        // Primeiro, garante que o tempo volte ao normal antes de recarregar
        Time.timeScale = 1f; 
        // Recarrega a cena que está ativa no momento
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void VoltarAoMenu(string nomeDaCenaMenu)
    {
        // Garante que o tempo volte ao normal
        Time.timeScale = 1f;
        // Carrega a cena do menu principal (o nome tem que ser EXATO)
        SceneManager.LoadScene(nomeDaCenaMenu);
    }
}