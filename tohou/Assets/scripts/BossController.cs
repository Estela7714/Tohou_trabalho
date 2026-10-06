using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Atributos")]
    public float maxHealth = 1000f;
    public float currentHealth;
    public bool isInvulnerable = false;

    [Header("Projéteis do Chefão")]
    public Projectile bossBulletPrefab;
    public Transform firePoint;
    public IObjectPool<Projectile> bossBulletPool;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Fases de Vida
    public enum Phase { High, Medium, Low }
    public Phase currentPhase = Phase.High;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        bossBulletPool = new ObjectPool<Projectile>(
            createFunc: () => {
                Projectile p = Instantiate(bossBulletPrefab);
                p.SetPool(bossBulletPool);
                p.tag = "BossBullet";
                return p;
            },
            actionOnGet: (p) => p.gameObject.SetActive(true),
            actionOnRelease: (p) => p.gameObject.SetActive(false),
            actionOnDestroy: (p) => Destroy(p.gameObject),
            defaultCapacity: 50,
            maxSize: 300
        );
    }

    private void Start()
    {
        currentHealth = maxHealth;
        GameManager.Instance?.UpdateBossUI(currentHealth, maxHealth, currentPhase);
    }

    public void TakeDamage(float amount)
    {
        if (isInvulnerable) return;

        currentHealth -= amount;
        GameManager.Instance?.UpdateBossUI(currentHealth, maxHealth, currentPhase);

        CheckPhaseTransition();

        if (currentHealth <= 0)
        {
            GameManager.Instance?.GameOver(true); // Vitória
            Destroy(gameObject);
        }
    }

    private void CheckPhaseTransition()
    {
        float healthPercent = currentHealth / maxHealth;

        if (currentPhase == Phase.High && healthPercent <= 0.5f)
        {
            StartCoroutine(PhaseTransitionRoutine(Phase.Medium, "ToPhaseMedium"));
        }
        else if (currentPhase == Phase.Medium && healthPercent <= 0.2f)
        {
            StartCoroutine(PhaseTransitionRoutine(Phase.Low, "ToPhaseLow"));
        }
    }

    private IEnumerator PhaseTransitionRoutine(Phase newPhase, string triggerName)
    {
        isInvulnerable = true;
        currentPhase = newPhase;

        // Feedback visual de Invulnerabilidade (pode piscar ou mudar a cor para cinza/transparente)
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Dispara a transição de fase no Animator
        animator.SetTrigger(triggerName);

        // Aguarda os 2 segundos obrigatórios
        yield return new WaitForSeconds(2.0f);

        spriteRenderer.color = originalColor;
        isInvulnerable = false;
    }
}