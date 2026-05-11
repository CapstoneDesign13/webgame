using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 이 스크립트 역할:
/// - 5라운드 이후 시작되는 일기토 전투를 처리한다.
/// - 일기토 시작 시 Console에 "Duel start"를 출력한다.
/// - 플레이어와 적들을 중앙 세로줄로 모은다.
/// - 적이 죽으면 남은 적들이 앞으로 한 칸 이동한다.
/// - 플레이어가 모든 적을 쓰러뜨리면 스테이지 클리어.
/// - 플레이어가 죽으면 게임오버.
/// </summary>
public class DuelManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TurnManager turnManager;

    [Header("Duel Settings")]
    [SerializeField] private float duelActionDelay = 0.6f;

    // 일기토 시작 후 중앙에 줄지어 모인 모습을 보여주기 위한 대기 시간
    [SerializeField] private float lineupDelayBeforeFight = 1.2f;

    [Header("Runtime State")]
    [SerializeField] private bool isRunning;

    private void OnEnable()
    {
        StartDuelPhase(MapManager.Instance.Player, MapManager.Instance.Enemies);
    }

    /// <summary>
    /// 일기토 시작.
    /// </summary>
    public void StartDuelPhase(PlayerUnit player, IReadOnlyList<EnemyUnit> livingEnemies)
    {
        if (isRunning)
        {
            Debug.LogWarning("이미 일기토가 진행 중입니다.");
            return;
        }

        if (player == null || player.IsDead)
        {
            FinishDuel(false);
            return;
        }

        List<EnemyUnit> duelEnemies = new List<EnemyUnit>();

        if (livingEnemies != null)
        {
            for (int i = 0; i < livingEnemies.Count; i++)
            {
                EnemyUnit enemy = livingEnemies[i];

                if (enemy != null && enemy.IsAlive)
                {
                    duelEnemies.Add(enemy);
                }
            }
        }

        if (duelEnemies.Count <= 0)
        {
            FinishDuel(true);
            return;
        }

        StartCoroutine(DuelRoutine(player, duelEnemies));
    }

    /// <summary>
    /// 일기토 전체 흐름.
    /// 
    /// 흐름:
    /// 1. Duel start 로그 출력
    /// 2. 플레이어와 적들이 중앙 줄로 모임
    /// 3. 잠깐 대기
    /// 4. 플레이어가 첫 번째 적과 싸움
    /// 5. 적이 죽으면 남은 적들이 앞으로 이동
    /// 6. 다음 적과 계속 싸움
    /// </summary>
    private IEnumerator DuelRoutine(PlayerUnit player, List<EnemyUnit> duelEnemies)
    {
        isRunning = true;

        Debug.Log("Duel start");

        WhoAmI(player);
        WhoIsNext(duelEnemies);

        Debug.Log("Duel lineup complete. Combat will begin soon.");

        yield return new WaitForSeconds(lineupDelayBeforeFight);

        for (int i = 0; i < duelEnemies.Count; i++)
        {
            EnemyUnit currentEnemy = duelEnemies[i];

            if (currentEnemy == null || !currentEnemy.IsAlive)
            {
                continue;
            }

            Debug.Log(
                "Duel Match Started: " +
                player.name +
                " vs " +
                currentEnemy.name
            );

            yield return StartCoroutine(RunDuelRound(player, currentEnemy));

            if (!player.IsAlive)
            {
                FinishDuel(false);
                yield break;
            }
        }

        FinishDuel(true);
    }

    public Image ally_img;
    public infoCardUI ally_info;

    void WhoAmI(PlayerUnit player)
    {
        Sprite spr = null;
        ally_img.sprite = spr;
        ally_info.setup(player);
    }

    public Image enemy_img;
    public infoCardUI enemy_info;

    void WhoIsNext(List<EnemyUnit> duelEnemies)
    {
        for (int i = 0; i < duelEnemies.Count; i++)
        {
            if (duelEnemies[i] != null && duelEnemies[i].IsAlive)
            {
                WhoIsNext(duelEnemies[i]);
                return;
            }
        }
    }

    void WhoIsNext(EnemyUnit duelEnemy)
    {
        Sprite spr = null;
        enemy_img.sprite = spr;
        enemy_info.setup(duelEnemy);
    }

    /// <summary>
    /// 플레이어와 적 1명의 1대1 공방.
    /// 
    /// 진행 방식:
    /// - 서로 동시 공격
    /// - 누군가 죽을 때까지 반복
    /// </summary>
    private IEnumerator RunDuelRound(PlayerUnit player, EnemyUnit enemy)
    {
        while (player.IsAlive && enemy.IsAlive)
        {
            player.TakeDamage(enemy);
            enemy.TakeDamage(player);

            WhoAmI(player);
            WhoIsNext(enemy);

            yield return new WaitForSeconds(duelActionDelay);

            if (player.IsDead)
            {
                Debug.Log(player.name + " defeated in duel.");
                break;
            }
            else if (enemy.IsDead)
            {
                Debug.Log(enemy.name + " defeated in duel.");
                break;
            }
        }
    }

    public TMP_Text txt;
    List<string> flavor_text = new List<string>()
    {
        "군집하였으나, 결국 잔모의 무리일 뿐.",
        "백이 모여, 한 줌의 위협 조차 되지 않는다.",
        "흩어진 개미가 산을 이룰 수는 없는 법.",
        "검 아래 모인 잡졸, 검 한 번에 흩어지리."
    };

    /// <summary>
    /// 일기토 최종 결과 처리.
    /// </summary>
    private void FinishDuel(bool playerWon)
    {
        isRunning = false;

        if (playerWon)
        {
            int rand = Random.Range(0, flavor_text.Count);
            txt.text = flavor_text[rand];
        }
    }
}