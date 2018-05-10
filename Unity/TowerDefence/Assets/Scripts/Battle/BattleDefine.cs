using Common.FrameWork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public static class BattleParam
    {
        // 変数：現在のステージID
        public static int CurrentStageId = 1;

        // 定数：エネルギー追加
        public const int AddEnergyTimeValue = 1;
        public const float AddEnergyTimeInterval = 5.0f;

        // 定数：残り制限時間ボーナス
        public const float StageLimitTime = 300.0f;
        public const float BonusTimeCreateEnemyInterval = 180.0f;
        public const float BonusRateCreateEnemyInterval = 2.0f;
        public const float BonusTimeAddEnergy = 120.0f;
        public const float BonusRateAddEnergy = 2.0f;
        public const float BonusTimeCreateEnemyAngry = 60.0f;
        public const float BonusRateCreateEnemyAngry = 4.0f;

        // 定数：攻撃高さ制限
        public const float BattleCharacterAttackHeightRange = 1.0f;

        // 状態：AnimationTag
        public enum CharacterState
        {
            Idle = 0,
            Walk,
            Fall,
            Down,
            Attack1,
            Attack2,
            Attack3,
        }

        // オブジェクトタイプ：派生クラス単位
        public enum ObjectType
        {
            None = 0,

            Bullet,
            Building,
            BattleCharacter,
        }

        // 所属：
        public enum Affiliation
        {
            None = 0,

            Player,
            Enemy,
        }

        // キャラクタータイプ
        public enum BattleCharacterType
        {
            None = 0,

            Kohaku,
            Toko,
        }

        // 建造物タイプ
        public enum BuildingType
        {
            None = 0,

            // 本拠地
            Castle,

            // キャラクター生成
            KohakuHouse,
            TokoHouse,

            // 砲台
            Cannon,
            CannonCheap,

            // 壁
            Wall,
            WallCheap,
        }

        // 弾タイプ
        public enum BulletType
        {
            None = 0,

            DropBomb,
            RecoveryBomb,
            PoisonBomb,
            SpeedUpBomb,
            SpeedDownBomb,

            CannonBullet,
            CheapCannonBullet,
        }

        // エフェクトタイプ
        public enum EffectType
        {
            None = 0,

            Explode,
            Kira,
        }

        // 状態異常
        public enum AbnormalStatusType
        {
            None = 0,

            Poison,
            SpeedUp,
            SpeedDown,
            Angry,
        }
    }

    public static class BattleData
    {
        // 作成物情報
        [Serializable]
        public class CreateObjectInfo
        {
            // database key
            public BattleParam.ObjectType ObjectType;
            public int TypeId;

            // data from database
            public int Level;
            public int ConsumeMana;
            public int ConsumeCoin;

            // dynamic param
            [NonSerialized] public BattleParam.Affiliation Affiliation;
            [NonSerialized] public Vector3 Pos;
            [NonSerialized] public GameObject CreaterGameObject;

            // attack
            public int AttackDamage;
            public float AttackRange;

            // damage object
            public int Hp;

            // move rigidbody2d force
            public Vector2 Rigidbody2DMoveVec2;

            // create point offset
            public Vector3 CreatePointOffset;

            // auto action
            public float AutoActionInterval;
            public int AutoActionReduceHp;
            public int PrepareAutoActionCount;

            // abnormal status
            public AbnormalStatusInfo AbnormalStatusInfo = new AbnormalStatusInfo();
        }

        // エネルギー情報
        [Serializable]
        public class EnergyInfo
        {
            // 素材追加時間
            public float ChargeMaterialTime;
            public float CurrentChargeMaterialTime;

            // 各素材追加数
            public int AddCoinCount;
            public int AddManaCount;

            // 素材所持数
            public int Coin;
            public int Mana;

            public void UpdateChargeMaterialTime()
            {
                CurrentChargeMaterialTime += Time.deltaTime;
                if (ChargeMaterialTime < CurrentChargeMaterialTime)
                {
                    Coin += AddCoinCount;
                    Mana += AddManaCount;
                    CurrentChargeMaterialTime = 0;
                }
            }
        }

        // 状態異常情報
        [Serializable]
        public class AbnormalStatusInfo
        {
            public AbnormalStatusInfo()
            { }

            public AbnormalStatusInfo(AbnormalStatusInfo info)
            {
                AbnormalStatusType = info.AbnormalStatusType;
                LimitTime = info.LimitTime;
                DamageInterval = info.DamageInterval;
                DamageIntervalMax = info.DamageIntervalMax;
                DamageValue = info.DamageValue;
                ChangeSpeedRate = info.ChangeSpeedRate;
            }

            // common
            public BattleParam.AbnormalStatusType AbnormalStatusType = BattleParam.AbnormalStatusType.None;
            public float LimitTime;

            // damage
            public float DamageInterval;
            public float DamageIntervalMax;
            public int DamageValue;

            // add speed
            public float ChangeSpeedRate = 1.0f;
            public bool IsUpdateSpeedRate;

            // utility
            public bool IsFinish()
            {
                return LimitTime <= 0;
            }
        }

        // ステージ情報
        [Serializable]
        public class StageInfo
        {
            public int StageId;

            // エネミー作成
            public float CreateEnemyIntervalMin;
            public float CreateEnemyIntervalMax;
            public int CreateEnemyAngryPercent;
            public int CreateEnemyAngryPercentMax;
            public List<BattleParam.BattleCharacterType> CreateBattleCharacterTypeList = new List<BattleParam.BattleCharacterType>();

            // 初期配置
            public List<CreateObjectInfo> InitCreateObjectInfoList = new List<CreateObjectInfo>();
        }
    }

        public static class BattleFunc
    {
        #region animation controller

        public static RuntimeAnimatorController CalcAnimationController(BattleParam.BattleCharacterType type)
        {
            var filePath = "AnimationController/";

            switch (type)
            {
                case BattleParam.BattleCharacterType.Kohaku: filePath += "Animator_Kohaku"; break;
                case BattleParam.BattleCharacterType.Toko: filePath += "Animator_Toko"; break;
            }

            return ResourceManager.LoadRuntimeAnimatorController(filePath);
        }

        #endregion

        #region sprite renderer

        public static Sprite CalcSprite(BattleData.CreateObjectInfo info)
        {
            switch (info.ObjectType)
            {
                case BattleParam.ObjectType.Building: return CalcSprite((BattleParam.BuildingType) info.TypeId);
                case BattleParam.ObjectType.Bullet: return CalcSprite((BattleParam.BulletType)info.TypeId);
                case BattleParam.ObjectType.BattleCharacter: return CalcSprite((BattleParam.BattleCharacterType)info.TypeId);
            }

            Debug.Assert(false, "CalcSprite : ObjectType = " + info.ObjectType);
            return null;
        }
        public static Sprite CalcSprite(BattleParam.BuildingType type)
        {
            var filePath = "Sprites/Building/" + type;
            return ResourceManager.LoadSprite(filePath);
        }
        public static Sprite CalcSprite(BattleParam.BulletType type)
        {
            var filePath = "Sprites/Bullet/" + type;
            return ResourceManager.LoadSprite(filePath);
        }
        public static Sprite CalcSprite(BattleParam.BattleCharacterType type)
        {
            var filePath = "Sprites/BattleCharacter/" + type;
            return ResourceManager.LoadSprite(filePath);
        }
        #endregion

        // プロトなんでここに記載、ちゃんとやるならデータベース化
        #region create object database

        public static BattleData.CreateObjectInfo GetCreateObjectInfo(BattleParam.BuildingType type)
        {
            var info = new BattleData.CreateObjectInfo();
            {
                info.ObjectType = BattleParam.ObjectType.Building;
                info.TypeId = (int)type;
            }

            switch (type)
            {
                case BattleParam.BuildingType.Castle:
                {
                    info.Level = 1;
                    info.Hp = 300;
                }
                    break;

                case BattleParam.BuildingType.KohakuHouse:
                {
                    info.Level = 1;
                    info.ConsumeCoin = 3;
                    info.Hp = 40;
                    info.AutoActionInterval = 2.0f;
                    info.AutoActionReduceHp = 10;
                    info.CreatePointOffset = new Vector3(0, -0.6f, 0);
                }
                    break;

                case BattleParam.BuildingType.TokoHouse:
                {
                    info.Level = 1;
                    info.ConsumeCoin = 5;
                    info.Hp = 40;
                    info.AutoActionInterval = 3.0f;
                    info.AutoActionReduceHp = 10;
                    info.CreatePointOffset = new Vector3(0, -0.6f, 0);
                }
                    break;

                case BattleParam.BuildingType.Cannon:
                {
                    info.Level = 1;
                    info.ConsumeMana = 6;
                    info.Hp = 30;
                    info.AutoActionInterval = 5.0f;
                    info.AutoActionReduceHp = 10;
                    info.CreatePointOffset = new Vector3(0.5f,0.4f,0);
                }
                    break;

                case BattleParam.BuildingType.CannonCheap:
                {
                    info.Level = 1;
                    info.ConsumeMana = 2;
                    info.Hp = 30;
                    info.AutoActionInterval = 3.0f;
                    info.AutoActionReduceHp = 10;
                    info.CreatePointOffset = new Vector3(0.5f, 0.4f, 0);
                }
                    break;

                case BattleParam.BuildingType.Wall:
                {
                    info.Level = 1;
                    info.ConsumeCoin = 2;
                    info.Hp = 500;
                }
                    break;

                case BattleParam.BuildingType.WallCheap:
                {
                    info.Level = 1;
                    info.ConsumeCoin = 1;
                    info.Hp = 200;
                }
                    break;
            }

            return info;
        }
        public static BattleData.CreateObjectInfo GetCreateObjectInfo(BattleParam.BulletType type)
        {
            var info = new BattleData.CreateObjectInfo();
            {
                info.ObjectType = BattleParam.ObjectType.Bullet;
                info.TypeId = (int)type;
            }

            switch (type)
            {
                case BattleParam.BulletType.DropBomb:
                {
                    info.Level = 1;
                    info.AttackDamage = 100;
                    info.AttackRange = 1.0f;
                    info.ConsumeMana = 3;
                }
                break;

                case BattleParam.BulletType.RecoveryBomb:
                {
                    info.Level = 1;
                    info.AttackDamage = -100;
                    info.AttackRange = 2.0f;
                    info.ConsumeMana = 1;
                }
                    break;

                case BattleParam.BulletType.PoisonBomb:
                {
                    info.Level = 1;
                    info.AttackRange = 2.0f;
                    info.ConsumeMana = 2;
                    info.AbnormalStatusInfo.AbnormalStatusType = BattleParam.AbnormalStatusType.Poison;
                    info.AbnormalStatusInfo.LimitTime = 100.0f;
                    info.AbnormalStatusInfo.DamageInterval = info.AbnormalStatusInfo.DamageIntervalMax = 10.0f;
                    info.AbnormalStatusInfo.DamageValue = 5;
                }
                    break;

                case BattleParam.BulletType.SpeedUpBomb:
                {
                    info.Level = 1;
                    info.AttackRange = 2.0f;
                    info.ConsumeMana = 1;
                    info.AbnormalStatusInfo.AbnormalStatusType = BattleParam.AbnormalStatusType.SpeedUp;
                    info.AbnormalStatusInfo.LimitTime = 20.0f;
                    info.AbnormalStatusInfo.ChangeSpeedRate = 2.0f;
                }
                    break;

                case BattleParam.BulletType.SpeedDownBomb:
                {
                    info.Level = 1;
                    info.AttackRange = 2.0f;
                    info.ConsumeMana = 2;
                    info.AbnormalStatusInfo.AbnormalStatusType = BattleParam.AbnormalStatusType.SpeedDown;
                    info.AbnormalStatusInfo.LimitTime = 20.0f;
                    info.AbnormalStatusInfo.ChangeSpeedRate = 0.5f;
                }
                    break;

                case BattleParam.BulletType.CannonBullet:
                {
                    info.Level = 1;
                    info.AttackDamage = 50;
                    info.AttackRange = 1.5f;
                    info.Rigidbody2DMoveVec2 = new Vector2(200.0f, 400.0f);
                }
                    break;

                case BattleParam.BulletType.CheapCannonBullet:
                {
                    info.Level = 1;
                    info.AttackDamage = 30;
                    info.AttackRange = 1.0f;
                    info.Rigidbody2DMoveVec2 = new Vector2(100.0f, 300.0f);
                }
                    break;
            }

            return info;
        }
        public static BattleData.CreateObjectInfo GetCreateObjectInfo(BattleParam.BattleCharacterType type)
        {
            var info = new BattleData.CreateObjectInfo();
            {
                info.ObjectType = BattleParam.ObjectType.BattleCharacter;
                info.TypeId = (int)type;
            }

            switch (type)
            {
                case BattleParam.BattleCharacterType.Kohaku:
                {
                    info.Level = 1;
                    info.Hp = 100;
                    info.Rigidbody2DMoveVec2 = new Vector2(0.5f,0);
                    info.AttackRange = 0.4f;
                    info.ConsumeCoin = 1;
                }
                break;

                case BattleParam.BattleCharacterType.Toko:
                {
                    info.Level = 1;
                    info.Hp = 60;
                    info.Rigidbody2DMoveVec2 = new Vector2(0.3f, 0);
                    info.AttackRange = 0.8f;
                    info.ConsumeCoin = 2;
                }
                    break;
            }

            return info;
        }

        #endregion

        #region stage info database

        public static BattleData.StageInfo CalcStageDataInfo(int stageId)
        {
            var stageInfo = new BattleData.StageInfo();

            // common
            {
                stageInfo.StageId = stageId;

                // 本拠地
                {
                    var info = GetCreateObjectInfo(BattleParam.BuildingType.Castle);
                    {
                        info.Affiliation = BattleParam.Affiliation.Player;
                        info.Level = 1;
                        info.Pos = BattleFunc.GetScreenTopLeft() + Vector3.right;
                    }
                    stageInfo.InitCreateObjectInfoList.Add(info);
                }
            }

            switch (stageId)
            {
                case 1:
                {
                    stageInfo.CreateEnemyIntervalMin = 1.0f;
                    stageInfo.CreateEnemyIntervalMax = 5.0f;
                    stageInfo.CreateEnemyAngryPercent = 5;
                    stageInfo.CreateBattleCharacterTypeList.Add(BattleParam.BattleCharacterType.Kohaku);

                    // 初期配置
                    {
                        var info = GetCreateObjectInfo(BattleParam.BuildingType.KohakuHouse);
                        {
                            info.Affiliation = BattleParam.Affiliation.Player;
                            info.Pos = BattleFunc.GetScreenTopLeft() + new Vector3(3.0f,0,0);
                        }
                        stageInfo.InitCreateObjectInfoList.Add(info);
                    }
                    {
                        var info = GetCreateObjectInfo(BattleParam.BuildingType.KohakuHouse);
                        {
                            info.Affiliation = BattleParam.Affiliation.Player;
                            info.Pos = BattleFunc.GetScreenTopLeft() + new Vector3(5.0f, 0, 0);
                        }
                        stageInfo.InitCreateObjectInfoList.Add(info);
                    }
                }
                    break;

                case 2:
                {
                    stageInfo.CreateEnemyIntervalMin = 1.0f;
                    stageInfo.CreateEnemyIntervalMax = 4.0f;
                    stageInfo.CreateEnemyAngryPercent = 10;
                    stageInfo.CreateBattleCharacterTypeList.Add(BattleParam.BattleCharacterType.Kohaku);
                    stageInfo.CreateBattleCharacterTypeList.Add(BattleParam.BattleCharacterType.Kohaku);
                    stageInfo.CreateBattleCharacterTypeList.Add(BattleParam.BattleCharacterType.Toko);
                }
                    break;

                case 3:
                {
                    stageInfo.CreateEnemyIntervalMin = 1.0f;
                    stageInfo.CreateEnemyIntervalMax = 3.0f;
                    stageInfo.CreateEnemyAngryPercent = 20;
                    stageInfo.CreateBattleCharacterTypeList.Add(BattleParam.BattleCharacterType.Kohaku);
                    stageInfo.CreateBattleCharacterTypeList.Add(BattleParam.BattleCharacterType.Toko);
                }
                    break;
            }

            return stageInfo;
        }

        #endregion

        #region utility

        public static Vector3 ConvertToCreateScreenOutPos(Vector3 pos)
        {
            var topLeft = GetScreenTopLeft();
            return new Vector3(pos.x, topLeft.y);
        }

        public static Vector3 GetScreenTopLeft()
        {
            // 画面の左上を取得
            Vector2 topLeft = Camera.main.ScreenToWorldPoint(Vector2.zero);
            // 上下反転させる
            topLeft.Scale(new Vector2(1f, -1f));
            return topLeft;
        }
        public static Vector3 GetScreenTopRight()
        {
            // 画面の右下を取得
            Vector2 bottomRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0));
            // 上下反転させる
            bottomRight.Scale(new Vector2(1f, -1f));
            return bottomRight;
        }
        #endregion
    }
}
