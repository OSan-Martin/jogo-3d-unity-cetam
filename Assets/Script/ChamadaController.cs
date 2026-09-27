using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ChamadaController : MonoBehaviour
{
    [Header("Configuração")]
    public string proximaCena = "Jogo";   // cena que será carregada depois
    public VideoPlayer videoPlayer;       // arraste o VideoPlayer aqui
    public GameObject telaPreta;          // arraste uma UI Image preta aqui

    private bool carregado;

    void Start()
    {
        if (videoPlayer == null)
            videoPlayer = FindObjectOfType<VideoPlayer>();

        if (videoPlayer != null)
        {
            // não destruir esse objeto (mantém som rodando entre cenas)
            DontDestroyOnLoad(videoPlayer.gameObject);

            videoPlayer.started += OnVideoComecou;
            videoPlayer.loopPointReached += OnVideoAcabou;
        }

        if (telaPreta != null)
            telaPreta.SetActive(true);
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.started -= OnVideoComecou;
            videoPlayer.loopPointReached -= OnVideoAcabou;
        }
    }

    void OnVideoComecou(VideoPlayer vp)
    {
        if (telaPreta != null)
            telaPreta.SetActive(false);
    }

    void OnVideoAcabou(VideoPlayer vp)
    {
        CarregarProxima();
    }

    void CarregarProxima()
    {
        if (carregado) return;
        carregado = true;

        Time.timeScale = 1f;
        SceneManager.LoadScene(proximaCena);

        // quando trocar, destruir só depois do áudio acabar
        Destroy(videoPlayer.gameObject, 1f); 
    }
}