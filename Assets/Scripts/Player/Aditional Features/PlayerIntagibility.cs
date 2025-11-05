using System.Collections;
using UnityEngine;

public class PlayerIntagibility : MonoBehaviour
{
    [Header("Intangibilidade")]
    public float intangibleDuration = 3f; // quanto tempo fica intangível
    public float cooldownDuration = 5f;   // tempo de recarga após acabar

    public KeyCode intangibleKey = KeyCode.I;

    public bool IsIntangible { get; private set; } = false;
    public bool IsOnCooldown { get; private set; } = false;

    private int playerLayer;
    private int intangibleLayer;

    private AnimationManager animManager;

    void Start()
    {
        playerLayer = gameObject.layer;
        intangibleLayer = LayerMask.NameToLayer("Intangivel");
        Physics2D.IgnoreLayerCollision(playerLayer, intangibleLayer, false);

        animManager = GetComponent<AnimationManager>();
    }

    void Update()
    {
        if(Game.Instance.isReceivingInputs())
        {
            if (Input.GetKeyDown(intangibleKey) && !IsIntangible && !IsOnCooldown)
            {
                //StartCoroutine(IntangibleRoutine());

                animManager.PlayHighPriority("WaterMode");
            }
            else if (Input.GetKeyDown(intangibleKey) && IsOnCooldown)
            {
                Debug.Log("Intangibilidade ainda em recarga!");
            }
        }
    }

    private IEnumerator IntangibleRoutine()
    {
        IsIntangible = true;
        Debug.Log("Jogador ficou intangível!");

        // Ignora colisão do jogador com a layer "Intangivel"
        Physics2D.IgnoreLayerCollision(playerLayer, intangibleLayer, true);

        yield return new WaitForSeconds(intangibleDuration);

        IsIntangible = false;
        Debug.Log("Jogador voltou ao normal!");

        // Reativa a colisão com a layer "Intangivel"
        Physics2D.IgnoreLayerCollision(playerLayer, intangibleLayer, false);

        // Inicia cooldown
        StartCoroutine(CooldownRoutine());
    }

    private void StartIntangible()
    {
        IsIntangible = true;
        Debug.Log("Jogador ficou intangível!");

        // Ignora colisão do jogador com a layer "Intangivel"
        Physics2D.IgnoreLayerCollision(playerLayer, intangibleLayer, true);
    }
    private void EndIntangible()
    {
        IsIntangible = false;
        Debug.Log("Jogador voltou ao normal!");

        // Reativa a colisão com a layer "Intangivel"
        Physics2D.IgnoreLayerCollision(playerLayer, intangibleLayer, false);

        // Inicia cooldown
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        IsOnCooldown = true;
        Debug.Log("Intangibilidade em recarga...");

        yield return new WaitForSeconds(cooldownDuration);

        IsOnCooldown = false;
        Debug.Log("Intangibilidade pronta para uso novamente!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Se encostar em algo com a tag Fire enquanto está intangível
        if (IsIntangible && other.CompareTag("Fire"))
        {
            FireEnemy fire = other.GetComponent<FireEnemy>();
            if (fire != null)
            {
                fire.DisableFireParent();
            }
        }
    }
}
