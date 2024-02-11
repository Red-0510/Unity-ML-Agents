using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private Button AiDrive;
    [SerializeField] private Button BallBalance;
    [SerializeField] private Button DQNBallBalance;
    [SerializeField] private Button MoveToGoal;
    [SerializeField] private Button Camouflage;
    [SerializeField] private Button DodgeBall;
    [SerializeField] private Button Movement;
    [SerializeField] private Button Pong;
    [SerializeField] private Button MazeWalkers;
    [SerializeField] private Button StayOnPlatform;


    private void Awake() {
        AiDrive.onClick.AddListener(()=> {
            Loader.Load(Loader.Scene.AIDrive);
        });
        BallBalance.onClick.AddListener(()=> {
            Loader.Load(Loader.Scene.BallBalance);
        });
        DQNBallBalance.onClick.AddListener(()=> {
            Loader.Load(Loader.Scene.DQNBallBalance);
        });
        MoveToGoal.onClick.AddListener(()=> {
            Loader.Load(Loader.Scene.MoveToGoalAgent);
        });
        Camouflage.onClick.AddListener(()=> {
            Loader.Load(Loader.Scene.GeneticAICamouflage);
        });
        DodgeBall.onClick.AddListener(()=> {
            Loader.Load(Loader.Scene.PerceptronDodgeBall);
        });
        Movement.onClick.AddListener(()=> {
            Loader.Load(Loader.Scene.GeneticAIMovement);
        });
        Pong.onClick.AddListener(()=> {
            Loader.Load(Loader.Scene.Pong);
        });
        MazeWalkers.onClick.AddListener(()=> {
            Loader.Load(Loader.Scene.GeneticMaze);
        });
        StayOnPlatform.onClick.AddListener(()=> {
            Loader.Load(Loader.Scene.GeneticAIStayOnPlatform);
        });

        Time.timeScale = 1f;
    }
}
