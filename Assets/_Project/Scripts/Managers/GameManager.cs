using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Debug.Log("GameManager Awake:" + gameObject.name);
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public enum GameState
    {
        Opening,
        Menu,
        HistoryList,
        Detail,
        Customization,
        RandomRoll,
        Confirm,
        Finalize,
        Matchup,
        Battle,
        BattleScene,
        Result,
        Digest,
        Ranking
    }

    private GameState currentState;

    [Header("Panels")]
    public GameObject openingPanel;
    public GameObject menuPanel;
    public GameObject historyListPanel;
    public GameObject detailPanel;
    public GameObject customizationPanel;
    public GameObject randomRollPanel;
    public GameObject confirmPanel;
    public GameObject finalizePanel;
    public GameObject matchupPanel;
    public GameObject battlePanel;
    public GameObject battleScenePanel;
    public GameObject resultPanel;
    public GameObject digestPanel;
    public GameObject rankingPanel;

    private Dictionary<GameState, GameObject> panelMap;

    private CharacterData selectedCharacter;

    private CharacterData enemyCharacter;

    private BattleCalculator.BattleResult latestBattleResult;

    private PartData.PartType selectedPartType;

    public void GoToRandomRollHead()
    {
        selectedPartType = PartData.PartType.Head;
        ChangeState(GameState.RandomRoll);
    }

    public void GoToRandomRollBody()
    {
        selectedPartType = PartData.PartType.Body;
        ChangeState(GameState.RandomRoll);
    }
    
    public void GoToRandomRollLeg()
    {
        selectedPartType = PartData.PartType.Leg;
        ChangeState(GameState.RandomRoll);
    }

    public void GoToRandomRollArm()
    {
        selectedPartType = PartData.PartType.Arm;
        ChangeState(GameState.RandomRoll);
    }

    public PartData.PartType GetSelectedPartType()
    {
        return selectedPartType;
    }
    
    void Start()
    {
        panelMap = new Dictionary<GameState, GameObject>()
        {
            { GameState.Opening, openingPanel },
            { GameState.Menu, menuPanel },
            { GameState.HistoryList, historyListPanel },
            { GameState.Detail, detailPanel },
            { GameState.Customization, customizationPanel },
            { GameState.RandomRoll, randomRollPanel },
            { GameState.Confirm, confirmPanel },
            { GameState.Finalize, finalizePanel },
            { GameState.Matchup, matchupPanel },
            { GameState.Battle, battlePanel },
            { GameState.BattleScene, battleScenePanel },
            { GameState.Result, resultPanel },
            { GameState.Digest, digestPanel },
            { GameState.Ranking, rankingPanel }
        };

        ChangeState(GameState.Opening);
    }

    public void ChangeState(GameState newState)
    {
        foreach(var panel in panelMap.Values)
        {
            if(panel != null)
            {
                panel.SetActive(false);
            }
        }

        if (panelMap.ContainsKey(newState) && panelMap[newState] != null)
        {
            panelMap[newState].SetActive(true);
        }

        currentState = newState;

    }

    public void OnOpeningFinished()
    {
        ChangeState(GameState.Menu);
    }

    public void GoToCreate()
    {
        ChangeState(GameState.Customization);
    }

    public void GoToView()
    {
        ChangeState(GameState.HistoryList);
    }
    
    public void GoToDetail()
    {
        ChangeState(GameState.Detail);
    }

    public void OnHistoryItemSelected(CharacterData data)
    {
        selectedCharacter = data;
        ChangeState(GameState.Detail);
    }

    public void BackToMenuFromHistory()
    {
        ChangeState(GameState.Menu);
    }

    public void BackToHistory()
    {
        ChangeState(GameState.HistoryList);
    }

    public void GoToRandomRoll()
    {
        ChangeState(GameState.RandomRoll);
    }

    public void GoToConfirm()
    {
        ChangeState(GameState.Confirm);
    }

    public void OnRandomRollFinished()
    {
        ChangeState(GameState.Customization);
    }

    public void ConfirmCharacter()
    {
        ChangeState(GameState.Finalize);
    }

    public void BackToCustomization()
    {
        ChangeState(GameState.Customization);
    }

    public void GoToMatchUp()
    {
        ChangeState(GameState.Matchup);
    }

    public void StartBattle()
    {
        ChangeState(GameState.Battle);
    }

    public void GoToBattleScene()
    {
        ChangeState(GameState.BattleScene);
    }

    public void OnBattleFinished()
    {
        ChangeState(GameState.Result);
    }

    public void GoToDigest()
    {
        ChangeState(GameState.Digest);
    }

    public void GoToRanking()
    {
        ChangeState(GameState.Ranking);
    }

    public void BackToMenu()
    {
        ChangeState(GameState.Menu);
    }

    public void ResetToOpening()
    {
        ChangeState(GameState.Opening);
    }

    public void EndTheGame()
    {
        ChangeState(GameState.Opening);
        /*ゲームを終わらせる処理*/
    }

    public void SetSelectedCharacter(CharacterData data)
    {
        selectedCharacter = data;
    }

    public CharacterData GetSelectedCharacter()
    {
        return selectedCharacter;
    }

    public void SetEnemyCharacter(CharacterData enemy)
    {
        enemyCharacter = enemy;
    }

    public CharacterData GetEnemyCharacter()
    {
        return enemyCharacter;
    }

    public void SetBattleResult(BattleCalculator.BattleResult result)
    {
        latestBattleResult = result;
    }
    
    public BattleCalculator.BattleResult GetBattleResult()
    {
        return latestBattleResult;
    }

    public GameState GetCurrentState()
    {
        return currentState;
    }

    private PartData.PartType lastRandomPartType;
    private bool hasLastRandomPartType = false;

    public void SetLastRandomPartType(PartData.PartType type)
    {
        lastRandomPartType = type;
        hasLastRandomPartType = true;
    }
    
    public bool HasLastRandomPartType()
    {
        return hasLastRandomPartType;
    }
    
    public PartData.PartType GetLastRandomPartType()
    {
        return lastRandomPartType;
    }
    
    public void ClearLastRandomPartType()
    {
        hasLastRandomPartType = false;
    }



}