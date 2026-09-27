using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MenuUI : MonoBehaviour
{
    [Header("Menus")]
    public GameObject menuPrincipal;   // Painel do menu principal
    public GameObject menuConfig;      // Painel de configurações
    public GameObject painelDePausa;   // Painel de pausa

    [Header("Configuração de Cena")]
    public float delayCena = 2.5f; // Delay antes de trocar de cena

    private bool jogoEstaPausado = false;

    void Update()
    {
        // Atalho para pausar/despausar (só funciona se não estiver no menu principal)
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "Menu")
        {
            if (jogoEstaPausado)
            {
                ContinuarJogo();
            }
            else
            {
                PausarJogo();
            }
        }
    }

    // ---------------- MENU PRINCIPAL ----------------
    public void Jogar()
    {
        StartCoroutine(TrocarCenaComDelay("Chamada"));
    }

    public void AbrirConfig()
    {
        menuPrincipal.SetActive(false);
        menuConfig.SetActive(true);
    }

    public void VoltarMenu()
    {
        menuConfig.SetActive(false);
        menuPrincipal.SetActive(true);
    }

    public void Sair()
    {
        Application.Quit();
        Debug.Log("Saiu do jogo");
    }

    // ---------------- MENU DE PAUSA ----------------
    public void PausarJogo()
    {
        painelDePausa.SetActive(true);
        Time.timeScale = 0f; // congela o jogo
        jogoEstaPausado = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ContinuarJogo()
    {
        painelDePausa.SetActive(false);
        Time.timeScale = 1f; // volta ao normal
        jogoEstaPausado = false;
    }

    public void VoltarParaOMenuPrincipal()
    {
        Time.timeScale = 1f; // garante que o tempo volte ao normal
        StartCoroutine(TrocarCenaComDelay("Menu"));
    }

    // ---------------- DELAY NA TROCA DE CENA ----------------
    private IEnumerator TrocarCenaComDelay(string nomeCena)
    {
        yield return new WaitForSeconds(delayCena);
        SceneManager.LoadScene(nomeCena);
    }
}