using System;
using System.Collections.Generic;

[Serializable] // これをつけることでJSON保存やインスペクターでの表示が可能になります
public class CharacterData
{
    // --- 識別用フラグ --- 
    public bool isEnemy;       // 完了したキャラかどうかの判定。画面8でfalse、終了時にtrue。
    public int playerIndex;    // 何人目の冒険者かを示す通し番号（nの値） 
    public string saveDateTime; // いつ作られたかの記録（ソート用にあると便利）

    // --- 1. 基本情報 ---
    public string name;          // 画面8で抽選される名前
    public int level;            // 画面8で決定されるLv
    public int rank;             // 画面13で決まる最終順位
    public int winCount;         // 10戦中の勝利数
    public int totalWins;         // これまでの全プレイを通じたこのキャラの累計勝利数。
    public int totalMatches;         // このキャラが対戦相手として選ばれた回数。
    public List<bool> battleLogs;         // 10戦それぞれの勝敗（True/False）を記録。

    // --- 2. 装備データ (IDや名前を保持) ---
    // 画面5で選択された装備のIDを格納します
    public string headID;
    public string bodyID;
    public string armID;
    public string legID;

    // --- 3. 成長・抽選用隠しパラメータ ---
    // 装備の「隠しポイント」を合算した値。Lvテーブルの決定に使用
    public int totalLevelPoints; 

    // --- 4. 戦闘用最終ステータス (計算後の値) ---
    // BattleCalculatorによって、Lvや作戦補正が加味された最終的な数値
    public float hpMax;          // 最大HP
    public float currentHp;      // バトル中の現在HP（0になったら負け）
    public float attack;         // 攻撃力
    public float defense;        // 防御力
    public float speed;          // 素早さ（行動順・クリティカル補正）
    public float critRate;       // クリティカル率 (%)
    public int attackCount;      // 攻撃回数

    // --- 5. コンストラクタ（初期化用） ---
    public CharacterData()
    {
        // 新しく箱を作った時の初期値
        name = "なまえのない冒険者";
        level = 1;
        currentHp = 1; 
    }
}