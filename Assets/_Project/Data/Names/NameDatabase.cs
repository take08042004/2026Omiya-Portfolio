using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NameDatabase", menuName = "ScriptableObjects/NameDatabase")]
public class NameDatabase : ScriptableObject
{
    public List<string> names = new List<string>();  //後ほど名前を入れるかする

    public string GetRandomName()
    {
        if (names == null || names.Count == 0)
        {
            return "なまえのない冒険者";
        }

        int index = Random.Range(0, names.Count);
        return names[index];
    }
}