using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

/// <summary>
/// Gerencia a lógica de jumpscare com vídeo e a tela de Game Over.
/// </summary>
public class JumpscareManager : MonoBehaviour
{
    [Header("Configuração do Jumpscare")]
    [Tooltip("O Canvas que contém o VideoPlayer do jumpscare.")]
    public GameObject jumpscareVideoCanvas;
    [Tooltip("O VideoPlayer que vai reproduzir o vídeo do jumpscare.")]
    public VideoPlayer jumpscareVideoPlayer;
    [Tooltip("O painel de UI que aparece após o jumpscare.")]
    public GameObject gameOverPanel;

    [Header("Componentes de Jogo")]
    [Tooltip("Referência ao script do patrulheiro para resetar a posição.")]
    public RandomPatrol randomPatrol;
    [Tooltip("Referência ao objeto do jogador para resetar a posição.")]
    public Transform player;

    [Header("Configuração de Cenas")]
    [Tooltip("O nome da cena do menu principal para onde o jogador retornará.")]
    public string menuSceneName = "Menu Scene";

    // A posição inicial do jogador, para que ele possa ser resetado.
    private Vector3 initialPlayerPosition;

    // Método que é executado no início do jogo, antes de qualquer frame.
    void Start()
    {
        // Garante que o Canvas do jumpscare esteja desativado ao iniciar o jogo.
        if (jumpscareVideoCanvas != null)
        {
            jumpscareVideoCanvas.SetActive(false);
        }

        // Garante que o painel de Game Over também esteja desativado no início.
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Armazena a posição inicial do jogador.
        if (player != null)
        {
            initialPlayerPosition = player.position;
        }

        // Trava o cursor do mouse no início do jogo.
        SetCursorState(false);
    }

    /// <summary>
    /// Inicia o jumpscare e congela o jogo.
    /// Esta função é chamada por outro script (ex: RandomPatrol).
    /// </summary>
    public void TriggerJumpscare()
    {
        StartCoroutine(DoJumpscareVideo());
    }

    /// <summary>
    /// Coroutine que gerencia a reprodução do vídeo do jumpscare e a exibição da tela de game over.
    /// </summary>
    private IEnumerator DoJumpscareVideo()
    {
        // Ativa o Canvas do susto e toca o vídeo
        if (jumpscareVideoCanvas != null)
        {
            jumpscareVideoCanvas.SetActive(true);
        }

        if (jumpscareVideoPlayer != null)
        {
            // O componente VideoPlayer também precisa ser ativado para que a tela do vídeo apareça novamente.
            jumpscareVideoPlayer.gameObject.SetActive(true);
            jumpscareVideoPlayer.Play();
        }

        // Espera o vídeo terminar
        if (jumpscareVideoPlayer != null && jumpscareVideoPlayer.clip != null)
        {
            yield return new WaitForSeconds((float)jumpscareVideoPlayer.clip.length);
        }
        
        // Desativa o vídeo e mostra a tela de game over
        if (jumpscareVideoPlayer != null)
        {
            jumpscareVideoPlayer.gameObject.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Congela o jogo e destrava o cursor
        Time.timeScale = 0;
        SetCursorState(true);
    }

    /// <summary>
    /// Reseta o estado do jumpscare, desativando o vídeo e o painel de Game Over,
    /// e também reseta o monstro e o jogador.
    /// </summary>
    public void ResetJumpscare()
    {
        if (jumpscareVideoCanvas != null)
        {
            jumpscareVideoCanvas.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Reseta o monstro para a posição inicial
        if (randomPatrol != null)
        {
            randomPatrol.ResetPosition();
        }

        // Reseta o jogador para a posição inicial
        if (player != null)
        {
            player.position = initialPlayerPosition;
        }
        
        // Restaura o tempo do jogo e trava o cursor
        Time.timeScale = 1f;
        SetCursorState(false);
    }

    /// <summary>
    /// Carrega a cena do menu principal usando o SceneLoader para salvar automaticamente no editor.
    /// Esta função deve ser chamada pelo botão "Tentar Novamente" no painel de Game Over.
    /// </summary>
    public void VoltarAoMenu()
    {
        // É importante restaurar a escala de tempo para 1 antes de carregar uma nova cena.
        Time.timeScale = 1f;
        // Usa nosso novo script para carregar a cena, que salva automaticamente no editor.
        SceneLoader.LoadScene(menuSceneName);
    }

    /// <summary>
    /// Gerencia o estado do cursor do mouse (trava/destrava).
    /// </summary>
    /// <param name="isUIActive">True se a UI está ativa, false se o jogo está rodando.</param>
    private void SetCursorState(bool isUIActive)
    {
        if (isUIActive)
        {
            // Se a UI está ativa (tela de game over), destrava o cursor para clicar.
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Se o jogo está rodando, trava o cursor no centro e o esconde.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}

