using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton para acesso global fácil
    public static GameManager Instance { get; private set; }

    [Header("UI do Jogador (3 Vidas)")]
    [Tooltip("Arraste aqui as 3 imagens/ícones que representam as vidas da navinha")]
    public GameObject[] playerLifeIcons;

    [Header("UI do Chefão")]
    [Tooltip("Arraste aqui a Slider da barra de vida do chefe")]
    public Slider bossHealthSlider;
    [Tooltip("Arraste aqui o componente Image do Fill da barra de vida")]
    public Image bossHealthFill;

    [Header("Cores da Barra do Chefão por Fase")]
    public Color phaseHighColor = Color.green;   // Vida > 50%
    public Color phaseMediumColor = Color.yellow; // 50% >= Vida > 20%
    public Color phaseLowColor = Color.red;      // Vida <= 20%

    [Header("Telas de Fim de Jogo")]
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    private void Awake()
    {
        // Configuração do Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Garante que as telas de fim de jogo comecem desativadas
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);

        // Despausa o jogo ao iniciar/reiniciar
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Atualiza os ícones de vida da navinha na UI.
    /// Desativa os ícones conforme o jogador toma dano.
    /// </summary>
    public void UpdatePlayerUI(int currentHealth)
    {
        if (playerLifeIcons == null) return;

        for (int i = 0; i < playerLifeIcons.Length; i++)
        {
            if (playerLifeIcons[i] != null)
            {
                // Deixa visível apenas o número de ícones equivalente à vida atual
                playerLifeIcons[i].SetActive(i < currentHealth);
            }
        }
    }

    /// <summary>
    /// Atualiza o valor e a cor da barra de vida do Chefão em tempo real.
    /// </summary>
    public void UpdateBossUI(float currentHealth, float maxHealth, BossController.Phase phase)
    {
        if (bossHealthSlider != null)
        {
            bossHealthSlider.value = Mathf.Clamp01(currentHealth / maxHealth);
        }

        if (bossHealthFill != null)
        {
            switch (phase)
            {
                case BossController.Phase.High:
                    bossHealthFill.color = phaseHighColor;
                    break;
                case BossController.Phase.Medium:
                    bossHealthFill.color = phaseMediumColor;
                    break;
                case BossController.Phase.Low:
                    bossHealthFill.color = phaseLowColor;
                    break;
            }
        }
    }

    /// <summary>
    /// Exibe a tela de Vitória ou Derrota e pausa o jogo.
    /// </summary>
    public void GameOver(bool isVictory)
    {
        if (isVictory)
        {
            if (victoryPanel != null) victoryPanel.SetActive(true);
        }
        else
        {
            if (defeatPanel != null) defeatPanel.SetActive(true);
        }

        // Congela o tempo do jogo (para movimentos, tiros e física)
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Método chamado pelo botão "Tentar Novamente" na tela de derrota.
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        // Recarrega a cena do jogo (garanta que o nome seja 'Game')
        SceneManager.LoadScene("Game");
    }
}