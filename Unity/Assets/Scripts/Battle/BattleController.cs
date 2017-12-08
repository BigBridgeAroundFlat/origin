using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class BattleController : MonoBehaviour
{
    // ref
    public GameObject PlayerObj;
    public GameObject EnemyObj;
    public Button CloseButton;

    // const 
    private const float MOVE_VALUE_PER_FRAME = 2.0f;
    private const float TOTAL_MOVE_VALUE = 50.0f;
    private const float WAIT_INTERVAL = 1.0f;
    private const float SCALE_INTERVAL = 0.5f;
    private const float ROTATE_INTERVAL = 1.0f;


    public enum AI_COMMAND_TYPE
    {
        NONE = 0,

        MOVE_NEAR,
        MOVE_FAR,
        ATTACK,
        WAIT,
    }
    private AI_COMMAND_TYPE PlayerAiCommandType = AI_COMMAND_TYPE.NONE;
    private AI_COMMAND_TYPE EnemyAiCommandType = AI_COMMAND_TYPE.NONE;

    // 移動長
    private float PlayerMoveValue;
    private float EnemyMoveValue;

    private void Start()
    {
        CloseButton.OnClickEtension(() =>
        {
            Engine.TransitionSceneManager.Instance.TransitionScene("Title");
        });
    }

    private void Update()
    {
        UpdatePlayerAI();
        UpdateEnemyAI();

        UpdatePlayerMove();
        UpdateEnemyMove();
    }

    private void UpdatePlayerAI()
    {
        if (PlayerAiCommandType != AI_COMMAND_TYPE.NONE)
        {
            return;
        }
        PlayerMoveValue = TOTAL_MOVE_VALUE;

        var playerPos = PlayerObj.transform.position;
        var enemyPos = EnemyObj.transform.position;
        var distance = Vector3.Distance(playerPos, enemyPos);

        // AIコマンド選択
        {
            var randomPercent = UnityEngine.Random.Range(0, 100);

            // 近い
            if (distance < 2.0f)
            {
                if (randomPercent < 60)
                {
                    PlayerAiCommandType = AI_COMMAND_TYPE.ATTACK;
                }
                else if (randomPercent < 90)
                {
                    PlayerAiCommandType = AI_COMMAND_TYPE.MOVE_FAR;
                }
                else
                {
                    PlayerAiCommandType = AI_COMMAND_TYPE.WAIT;
                }
            }
            // 遠い
            else if (5.0f < distance)
            {
                PlayerAiCommandType = AI_COMMAND_TYPE.MOVE_NEAR;
            }
            // 中間
            else
            {
                if (randomPercent < 60)
                {
                    PlayerAiCommandType = AI_COMMAND_TYPE.MOVE_NEAR;
                }
                else if (randomPercent < 90)
                {
                    PlayerAiCommandType = AI_COMMAND_TYPE.MOVE_FAR;
                }
                else
                {
                    PlayerAiCommandType = AI_COMMAND_TYPE.WAIT;
                }
            }
        }

        // AIアクション
        UpdatePlayerAction();
    }
    private void UpdatePlayerMove()
    {
        switch (PlayerAiCommandType)
        {
            case AI_COMMAND_TYPE.MOVE_NEAR:
            case AI_COMMAND_TYPE.MOVE_FAR:
                break;

            default:
                return;
        }

        // 移動
        var moveValue = PlayerAiCommandType == AI_COMMAND_TYPE.MOVE_NEAR ? MOVE_VALUE_PER_FRAME : -MOVE_VALUE_PER_FRAME;
        PlayerObj.GetComponent<Rigidbody2D>().velocity = new Vector2(moveValue, 0);

        //移動総量チェック
        PlayerMoveValue -= MOVE_VALUE_PER_FRAME;
        if(PlayerMoveValue <= 0)
        {
            PlayerAiCommandType = AI_COMMAND_TYPE.NONE;
        }
    }
    private void UpdatePlayerAction()
    {
        switch (PlayerAiCommandType)
        {
            case AI_COMMAND_TYPE.ATTACK:
                {
                    var seq = DOTween.Sequence();
                    {
                        seq.Append(PlayerObj.transform.DORotate(new Vector3(0,360.0f), ROTATE_INTERVAL, RotateMode.FastBeyond360));
                        seq.AppendCallback(() =>
                        {
                            PlayerAiCommandType = AI_COMMAND_TYPE.NONE;
                        });
                    }
                }
                break;

            case AI_COMMAND_TYPE.WAIT:
                {
                    var seq = DOTween.Sequence();
                    {
                        seq.AppendInterval(WAIT_INTERVAL);
                        seq.AppendCallback(() =>
                        {
                            PlayerAiCommandType = AI_COMMAND_TYPE.NONE;
                        });
                    }
                }
                break;
        }
    }

    private void UpdateEnemyAI()
    {
        if (EnemyAiCommandType != AI_COMMAND_TYPE.NONE)
        {
            return;
        }
        EnemyMoveValue = TOTAL_MOVE_VALUE;

        var EnemyPos = EnemyObj.transform.position;
        var PlayerPos = PlayerObj.transform.position;
        var distance = Vector3.Distance(EnemyPos, PlayerPos);

        // AIコマンド選択
        {
            var randomPercent = UnityEngine.Random.Range(0, 100);

            // 近い
            if (distance < 2.0f)
            {
                if (randomPercent < 60)
                {
                    EnemyAiCommandType = AI_COMMAND_TYPE.ATTACK;
                }
                else if (randomPercent < 90)
                {
                    EnemyAiCommandType = AI_COMMAND_TYPE.MOVE_FAR;
                }
                else
                {
                    EnemyAiCommandType = AI_COMMAND_TYPE.WAIT;
                }
            }
            // 遠い
            else if (5.0f < distance)
            {
                EnemyAiCommandType = AI_COMMAND_TYPE.MOVE_NEAR;
            }
            // 中間
            else
            {
                if (randomPercent < 60)
                {
                    EnemyAiCommandType = AI_COMMAND_TYPE.MOVE_NEAR;
                }
                else if (randomPercent < 90)
                {
                    EnemyAiCommandType = AI_COMMAND_TYPE.MOVE_FAR;
                }
                else
                {
                    EnemyAiCommandType = AI_COMMAND_TYPE.WAIT;
                }
            }
        }

        // AIアクション
        UpdateEnemyAction();
    }
    private void UpdateEnemyMove()
    {
        switch (EnemyAiCommandType)
        {
            case AI_COMMAND_TYPE.MOVE_NEAR:
            case AI_COMMAND_TYPE.MOVE_FAR:
                break;

            default:
                return;
        }

        // 移動
        var moveValue = EnemyAiCommandType == AI_COMMAND_TYPE.MOVE_NEAR ? -MOVE_VALUE_PER_FRAME : MOVE_VALUE_PER_FRAME;
        EnemyObj.GetComponent<Rigidbody2D>().velocity = new Vector2(moveValue, 0);

        //移動総量チェック
        EnemyMoveValue -= MOVE_VALUE_PER_FRAME;
        if (EnemyMoveValue <= 0)
        {
            EnemyAiCommandType = AI_COMMAND_TYPE.NONE;
        }
    }
    private void UpdateEnemyAction()
    {
        switch (EnemyAiCommandType)
        {
            case AI_COMMAND_TYPE.ATTACK:
                {
                    var seq = DOTween.Sequence();
                    {
                        seq.Append(EnemyObj.transform.DOScale(1.5f, SCALE_INTERVAL));
                        seq.Append(EnemyObj.transform.DOScale(1.0f, SCALE_INTERVAL));
                        seq.AppendCallback(() =>
                        {
                            EnemyAiCommandType = AI_COMMAND_TYPE.NONE;
                        });
                    }
                }
                break;

            case AI_COMMAND_TYPE.WAIT:
                {
                    var seq = DOTween.Sequence();
                    {
                        seq.AppendInterval(WAIT_INTERVAL);
                        seq.AppendCallback(() =>
                        {
                            EnemyAiCommandType = AI_COMMAND_TYPE.NONE;
                        });
                    }
                }
                break;
        }
    }
}

