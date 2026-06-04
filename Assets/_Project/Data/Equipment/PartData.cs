using UnityEngine;

[CreateAssetMenu(fileName = "NewPartData", menuName = "ScriptableObjects/PartData")]
public class PartData : ScriptableObject
{
    public enum PartType { Head, Body, Arm, Leg }

    [Header("基本設定")]
    public string partID;        // CharacterData の headID 等に代入される識別名
    public string partName;      // 表示用の名前
    public PartType type;        // 部位の種類

    [Header("ビジュアル設定")]
    public GameObject modelPrefab;   // 装備のFBX（プレハブ）

    [Header("アニメーション設定")]
    // Blender班が命名ルール通りに作った各モーションを登録
    public AnimationClip idleAnimation;
    public AnimationClip attackAnimation;
    public AnimationClip defenseAnimation;
    public AnimationClip eventAnimation;

    public RuntimeAnimatorController animatorController;

    [Header("ステータス補正値")]
    // CharacterData の各戦闘ステータスに対応するボーナス値
    public float hpBonus;
    public float attackBonus;
    public float defenseBonus;
    public float speedBonus;
    public float critBonus; //クリティカル率
    public float attackNumber; //攻撃回数

    [Header("成長・抽選用ポイント")]
    public int levelPoint;       // CharacterData の totalLevelPoints に加算される値
}
