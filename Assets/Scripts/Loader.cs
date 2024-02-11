using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader {

    public enum Scene {
        MainMenuScene,
        AIDrive,
        BallBalance,
        DQNBallBalance,
        GeneticAICamouflage,
        GeneticAIMovement,
        GeneticAIStayOnPlatform,
        GeneticMaze,
        MoveToGoalAgent,
        PerceptronDodgeBall,
        Pong,
    }

    public static void Load(Scene targetScene) {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
