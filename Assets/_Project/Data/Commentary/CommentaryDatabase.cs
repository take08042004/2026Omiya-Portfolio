using UnityEngine;
using System.Collections.Generic;
//いったんコンパイル通すために仮のコメントデータベースを作成
[CreateAssetMenu(fileName = "CommentaryDatabase", menuName = "ScriptableObjects/CommentaryDatabase")]
public class CommentaryDatabase : ScriptableObject
{
    public List<string> attackComments = new List<string> { "鋭い攻撃だ！", "いい一撃！" };
    public List<string> criticalComments = new List<string> { "会心の一撃！", "破壊的な威力だ！" };

    public string GetAttackComment() => attackComments[Random.Range(0, attackComments.Count)];
    public string GetCriticalComment() => criticalComments[Random.Range(0, criticalComments.Count)];
    public string GetVictoryComment() => "見事な勝利だ！";
    public string GetDefeatComment() => "惜しくも敗北…";
}