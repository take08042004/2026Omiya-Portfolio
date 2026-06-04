using System.Collections.Generic;
using UnityEngine;

// --- 装備マージ & アニメーション同期スクリプト ---
public class EquipmentMerger : MonoBehaviour
{
    [Header("素体の設定")]
    public Transform bodyRoot;
    public Animator targetAnimator; // 素体のAnimatorをセットしてください

    [Header("装備データベース")]
    public List<PartData> allPartsDatabase = new List<PartData>();

    // 生成されたパーツを保持する変数
    private GameObject currentHead;
    private GameObject currentBody;
    private GameObject currentArm;
    private GameObject currentLeg;

    // 素体の全ボーンをキャッシュする辞書
    private Dictionary<string, Transform> bodyBoneDict = new Dictionary<string, Transform>();

    private RuntimeAnimatorController originalController;

    private void Awake()
    {
        // 1. 素体の全ボーンをスキャンして名前に紐付ける
        if (bodyRoot == null) bodyRoot = transform;
        
        foreach (Transform t in bodyRoot.GetComponentsInChildren<Transform>())
        {
            if (!bodyBoneDict.ContainsKey(t.name))
            {
                bodyBoneDict.Add(t.name, t);
            }
        }
    }

    // --- 外部（UI等）から呼び出す関数 ---

    public void ApplyCharacterData(CharacterData data)
    {
        if (data == null) return;
        ApplyHead(data.headID);
        ApplyBody(data.bodyID);
        ApplyArm(data.armID);
        ApplyLeg(data.legID);
    }

    // --- EquipmentMerger.cs の ApplyHead, ApplyBody, ApplyLeg, ApplyArm を以下に書き換え ---

    public void ApplyHead(string id) => ProcessEquipment(ref currentHead, id);
    public void ApplyBody(string id) => ProcessEquipment(ref currentBody, id);
    public void ApplyLeg(string id)  => ProcessEquipment(ref currentLeg, id);
    public void ApplyArm(string id)  => ProcessEquipment(ref currentArm, id);

    /// <summary>
    /// モデルの置換とアニメーションの上書きをセットで行う共通処理
    /// </summary>
    private void ProcessEquipment(ref GameObject currentObj, string id)
    {
        // 1. モデルの差し替え（既存の処理を呼び出し）
        currentObj = ReplaceEquipmentMerged(currentObj, id);
    
        // 2. データベースから今回の装備データを取得
        PartData data = allPartsDatabase.Find(p => p.partID == id);
    
        // 3. データを渡してアニメーションを上書き
        if (data != null)
        {
            UpdateAnimationOverride(data); 
        }
    }

    /// <summary>
    /// ボーンのリマップ処理
    /// </summary>
    private GameObject ReplaceEquipmentMerged(GameObject currentObj, string equipmentID)
    {
        if (currentObj != null) Destroy(currentObj);

        PartData data = allPartsDatabase.Find(p => p.partID == equipmentID);
        if (data == null || data.modelPrefab == null) return null;

        GameObject newObj = Instantiate(data.modelPrefab, transform);
        SkinnedMeshRenderer targetSMR = newObj.GetComponentInChildren<SkinnedMeshRenderer>();

        if (targetSMR != null)
        {
            Transform[] bones = targetSMR.bones;
            Transform[] newBones = new Transform[bones.Length];

            for (int i = 0; i < bones.Length; i++)
            {
                string boneName = bones[i].name;
                if (bodyBoneDict.ContainsKey(boneName))
                {
                    newBones[i] = bodyBoneDict[boneName];
                }
                else
                {
                    newBones[i] = bones[i];
                }
            }
            targetSMR.bones = newBones;

            if (targetSMR.rootBone != null && bodyBoneDict.ContainsKey(targetSMR.rootBone.name))
            {
                targetSMR.rootBone = bodyBoneDict[targetSMR.rootBone.name];
            }
        }

        // 不要なアーマチュアを非表示（Blenderの階層名に合わせる）
        Transform equipArmature = newObj.transform.Find("Armature");
        if (equipArmature != null) equipArmature.gameObject.SetActive(false);

        return newObj;
    }

    /// <summary>
    /// AnimatorOverrideController を使って攻撃モーションを上書きする
    /// </summary>

    // --- UpdateAnimationOverride 関数をこのように修正 ---
    // 引数の型を (AnimationClip data) から (PartData data) に変更します
    private void UpdateAnimationOverride(PartData data)
    {
       if (targetAnimator == null) return;

    // 1. 最初の着替えの時に、元々のコントローラーを originalController に退避する
    if (originalController == null && targetAnimator.runtimeAnimatorController != null)
    {
        // すでに上書きコントローラーに変わっている場合は、その「大元」を取得する
        if (targetAnimator.runtimeAnimatorController is AnimatorOverrideController alreadyOverride)
        {
            originalController = alreadyOverride.runtimeAnimatorController;
        }
        else
        {
            originalController = targetAnimator.runtimeAnimatorController;
        }
    }

    // パーツに上書き用のアニメーターコントローラーが設定されていない場合は、元の状態に戻して終了
    if (data == null || data.animatorController == null)
    {
        if (originalController != null)
        {
            targetAnimator.runtimeAnimatorController = originalController;
        }
        return;
    }

    if (originalController == null)
    {
        Debug.LogError("EquipmentMerger: 素体にベースとなる AnimatorController が設定されていません。");
        return;
    }

    // 2. 必ず「大元のコントローラー」をベースにして上書きコントローラーを新規生成する
    AnimatorOverrideController overrideController = new AnimatorOverrideController(originalController);

    // ここでパーツ固有のアニメーション（Override）を適用する処理
    // (既存のコードの overrideController へクリップを代入する処理をここに記述)
    // 例: overrideController["OriginalClipName"] = part.customClip; など

    // 3. アニメーターにセットする
    targetAnimator.runtimeAnimatorController = overrideController;
    }
}