using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayerFollow : MonoBehaviour
{
    public Transform target; // Referencia ao personagem
    public float smoothing = 5f; // Velocidade de suavizacao da camera
    Vector3 offset; // Distancia entre a camera e o personagem

    void Start()
    {
        // Calcula a diferenca inicial entre a posicao da camera e do personagem
        offset = transform.position - target.position;
    }

    void FixedUpdate()
    {
        // A nova posicao da camera e a posicao do personagem mais o deslocamento inicial
        Vector3 targetCamPos = target.position + offset;
        // Suaviza a movimentacao da camera para seguir o personagem
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}