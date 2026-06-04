using UnityEngine;
using System.Collections.Generic;
//いったんコンパイル通すためにダミー
public class RankingSystem : MonoBehaviour
{
    //最終的なロジック:GameManager内の関数から過去のプレイヤーのデータを引っ張て来て、勝ち数を比較する
    public int CalculateRank(CharacterData data)
    {
        if (data == null) return 0;

        List<CharacterData> allCharacters = SaveSystem.LoadCharacters();

        int rank = 1;

        foreach(CharacterData character in allCharacters)
        {
            if(character.winCount > data.winCount)
            {
                rank++;
            }
        }

        return rank;



    }
}