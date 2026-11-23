using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider; //プレイヤーの体力を表示するスライダー
    private PlayerStats playerstats; //プレイヤーのステータスを管理するクラスの参照

    private void Start()
    {
        playerstats = FindObjectOfType<PlayerStats>();
        Debug.Log(playerstats);

        healthSlider.maxValue = playerstats.MaxHP;
        healthSlider.value = playerstats.CurrentHP;

        playerstats.OnHpChanged += UpdateHpBar;
    }
    /// <summary>
    /// Updates the health slider value to match the given health percentage.
    /// </summary>
    /// <param name="hp">hp: The current health percentage (0.0 to 1.0).</param>
    private void UpdateHpBar(float hp)
    {
        Debug.Log("HP更新:" + hp);
        healthSlider.value = hp;
    }

    private void OnDestroy()
    {
        if (playerstats != null) playerstats.OnHpChanged -= UpdateHpBar;
    }
}
