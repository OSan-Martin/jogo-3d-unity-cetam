using UnityEngine;
using UnityEngine.SceneManagement;

// Importa as bibliotecas do Editor da Unity
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// Uma classe auxiliar para carregar cenas que, quando executada no Editor,
/// salva automaticamente as alterações da cena atual antes de prosseguir.
/// </summary>
public static class SceneLoader
{
    public static void LoadScene(string sceneName)
    {
        // Este bloco de código só existe quando estamos dentro do Editor da Unity.
        // Ele é removido quando você compila o jogo final.
#if UNITY_EDITOR
        // Verifica se a cena ativa tem alterações não salvas (se está "dirty")
        if (EditorSceneManager.GetActiveScene().isDirty)
        {
            Debug.Log("Cena atual tem alterações não salvas. Salvando agora...");
            // Salva todas as cenas que estiverem abertas
            EditorSceneManager.SaveOpenScenes();
        }
#endif

        // Agora, carrega a cena normalmente.
        SceneManager.LoadScene(sceneName);
    }
}
