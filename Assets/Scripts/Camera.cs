using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    // A sensibilidade do mouse, pra tu poder ajustar depois.
    public float sensibilidadeMouse = 100f;

    // A rotação total no eixo X (pra olhar pra cima e pra baixo).
    float rotacaoX = 0f;

    // A função Start() pra bloquear o cursor do mouse no meio da tela.
    void Start()
    {
        // Trava o cursor no meio da tela pra ele não sair da janela.
        Cursor.lockState = CursorLockMode.Locked;
    }

    // A função Update() pra atualizar a cada frame.
    void Update()
    {
        // Pega o movimento do mouse nos eixos X e Y.
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadeMouse * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadeMouse * Time.deltaTime;

        // Diminui a rotação do mouse no eixo Y para controlar o olhar para cima e para baixo.
        rotacaoX -= mouseY;
        // Limita a rotação pra não virar o pescoço e quebrar.
        rotacaoX = Mathf.Clamp(rotacaoX, -90f, 90f);

        // Aplica a rotação no eixo Y (pro personagem girar) e no eixo X (pra câmera olhar pra cima/baixo).
        transform.localRotation = Quaternion.Euler(rotacaoX, 0f, 0f);
        // Roda o pai da câmera no eixo Y, que no caso é a cápsula.
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}