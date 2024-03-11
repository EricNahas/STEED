using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public Animator playerAnimator;
    float input_x = 0;
    float input_y = 0;
    public float speed = 1f;
    bool isWalking = false;
    public float interactionDistance = 6f; // Distância máxima para interação
    public LayerMask interactableLayer; // Camada dos objetos interativos

    Rigidbody2D rb2D;
    Vector2 movement = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        isWalking = false;
        rb2D = GetComponent<Rigidbody2D>();

        // Carregar a posição do jogador se existir
        if (PlayerPrefs.HasKey("PlayerPositionX") && PlayerPrefs.HasKey("PlayerPositionY"))
        {
            float x = PlayerPrefs.GetFloat("PlayerPositionX");
            float y = PlayerPrefs.GetFloat("PlayerPositionY");
            transform.position = new Vector3(x, y, transform.position.z);
        }
        else
        {
            Debug.Log("Sem preferencia");
        }
    }


    // Update is called once per frame
    void Update()
    {
        input_x = Input.GetAxisRaw("Horizontal");
        input_y = Input.GetAxisRaw("Vertical");
        isWalking = (input_x != 0 || input_y != 0);
        movement = new Vector2(input_x, input_y);

        if (isWalking)
        {
            playerAnimator.SetFloat("input_x", input_x);
            playerAnimator.SetFloat("input_y", input_y);
        }

        playerAnimator.SetBool("isWalking", isWalking);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            string currentscene = SceneManager.GetActiveScene().name;

            if (currentscene != "Opcoes")
            {
                PlayerPrefs.SetFloat("Px", transform.position.x);
                PlayerPrefs.SetFloat("Py", transform.position.y);

                PlayerPrefs.SetString("LastScene", currentscene);
                SceneManager.LoadScene("Opcoes");
            }
            else
            {
                SceneManager.LoadScene(PlayerPrefs.GetString("LastScene"));

            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            // Raycast para detectar objetos interativos à frente do jogador
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, interactionDistance, interactableLayer);

            // Se o raio atingir um objeto interativo
            if (hit.collider != null)
            {
                // Verifica se o objeto interativo é o computador
                if (hit.collider.CompareTag("Computador"))
                {
                    // Carrega a cena do computador
                    SceneManager.LoadScene("Computador");
                }
            }
        }
    }


    private void FixedUpdate()
    {
        rb2D.MovePosition(rb2D.position + movement * speed * Time.fixedDeltaTime);
    }

    private void OnApplicationQuit()
    {
        //Salvar a posição do jogador quando o aplicativo é fechado
        PlayerPrefs.SetFloat("PlayerPositionX", transform.position.x);
        PlayerPrefs.SetFloat("PlayerPositionY", transform.position.y);
        PlayerPrefs.Save();
    }
}
