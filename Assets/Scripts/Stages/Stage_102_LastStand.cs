using UnityEngine.Playables;
using UnityEngine;

public class Stage_102_LastStand : MonoBehaviour {
    public Stage102State StageState;
    public GameObject Spawner;
    public Transform[] FixedPoint;
    public Transform[] RandPoint;
    public float[] Durations;
    public string Objective;
    public float PlayingTime;

    public PlayableDirector Director { get; set; }

    public GameObject CreepRush { get; set; }
    public GameObject CreepGuard { get; set; }

    public Stage102State StageLevel {
        get {
            return StageState;
        }
        set {
            for (int i = 0; i < FixedPoint.Length; i++) {
                for (int j = 0; j < RandPoint[i].childCount; j++) {
                    Destroy(RandPoint[i].GetChild(j).gameObject);
                }
                FixedPoint[i].gameObject.SetActive(false);
            }
            switch (value) {
                case Stage102State.Prolog:
                    Act1_Beginning();
                    break;
                case Stage102State.Warmup:
                    Act2_WarmUp();
                    break;
                case Stage102State.Midnight:
                    Act3_Midnight();
                    break;
                case Stage102State.Lategame:
                    Act4_LateGame();
                    break;
                case Stage102State.Epilog:
                    Act5_Ending();
                    break;
            }
            StageState = value;
        }
    }

    private void AfterCutscene_Act1(PlayableDirector e) {
        StageLevel = Stage102State.Warmup;
    }

    private void AfterCutscene_Act4(PlayableDirector e) {
        for (int i = 0; i < FixedPoint.Length; i++) {
            var RandomSet = Instantiate(Spawner, RandPoint[i].position, Quaternion.identity, RandPoint[i]);
            RandomSet.GetComponent<GeneratorActive>().Lifetime = 20;
            RandomSet.GetComponent<GeneratorActive>().EnemyStock = 4;
            RandomSet.GetComponent<GeneratorActive>().IsUnlimited = false;
            FixedPoint[i].gameObject.SetActive(true);
            FixedPoint[i].GetComponent<GeneratorActive>().EnemyStock = 3;
        }
        Objective = "Bertahanlah hingga fajar";
        Director.stopped -= AfterCutscene_Act4;
    }

    private void AfterCutscene_Act5(PlayableDirector e) {

        Director.stopped -= AfterCutscene_Act5;
    }

    private void Act1_Beginning() {
        var cutscene = Resources.Load<PlayableAsset>("Cutscene/LastStand_Opening");
        Director.stopped += AfterCutscene_Act1;
        Director.playableAsset = cutscene;
        Director.Play();
    }

    private void Act2_WarmUp() {
        FixedPoint[0].GetComponent<GeneratorActive>().EnemySeed = CreepGuard;
        FixedPoint[0].GetComponent<GeneratorActive>().Lifetime = 12;
        FixedPoint[0].GetComponent<GeneratorActive>().EnemyStock = 3;
        FixedPoint[0].GetComponent<GeneratorActive>().IsUnlimited = true;
        FixedPoint[0].gameObject.SetActive(true);

        FixedPoint[1].GetComponent<GeneratorActive>().EnemySeed = CreepRush;
        FixedPoint[1].GetComponent<GeneratorActive>().Lifetime = 10;
        FixedPoint[1].GetComponent<GeneratorActive>().EnemyStock = 1;
        FixedPoint[1].GetComponent<GeneratorActive>().IsUnlimited = true;
        FixedPoint[1].gameObject.SetActive(true);

        var n = Random.Range(0, 4);
        var RandomSet = Instantiate(Spawner, RandPoint[n].position, Quaternion.identity, RandPoint[n]);
        Director.stopped -= AfterCutscene_Act1;
        RandomSet.GetComponent<GeneratorActive>().EnemySeed = CreepRush;
        RandomSet.GetComponent<GeneratorActive>().Lifetime = 12;
        RandomSet.GetComponent<GeneratorActive>().EnemyStock = 3;
        RandomSet.GetComponent<GeneratorActive>().IsUnlimited = false;
        Objective = "Bunuh semua Creep yang muncul";
    }

    private void Act3_Midnight() {
        var n1 = Random.Range(0, 4);
        var n2 = Random.Range(0, 4);
        while (n1 == n2) {
            n2 = Random.Range(0, 4);
        }

        Objective = "Bunuh semua Creep yang muncul dan bertahanlah";
        for (int i = 0; i < FixedPoint.Length; i++) {
            FixedPoint[i].GetComponent<GeneratorActive>().EnemySeed = CreepGuard;
            FixedPoint[i].GetComponent<GeneratorActive>().Lifetime = 12;
            FixedPoint[i].GetComponent<GeneratorActive>().EnemyStock = 3;
            FixedPoint[i].GetComponent<GeneratorActive>().IsUnlimited = true;
            FixedPoint[i].gameObject.SetActive(true);
        }

        FixedPoint[1].GetComponent<GeneratorActive>().EnemySeed = CreepRush;
        FixedPoint[1].GetComponent<GeneratorActive>().Lifetime = 13;
        FixedPoint[1].GetComponent<GeneratorActive>().EnemyStock = 3;
        FixedPoint[1].GetComponent<GeneratorActive>().IsUnlimited = true;
        FixedPoint[1].gameObject.SetActive(true);

        var RandomSet1 = Instantiate(Spawner, RandPoint[n1].position, Quaternion.identity, RandPoint[n1]);
        RandomSet1.GetComponent<GeneratorActive>().Lifetime = 12;
        RandomSet1.GetComponent<GeneratorActive>().EnemyStock = 3;
        RandomSet1.GetComponent<GeneratorActive>().IsUnlimited = false;

        var RandomSet2 = Instantiate(Spawner, RandPoint[n2].position, Quaternion.identity, RandPoint[n2]);
        RandomSet1.GetComponent<GeneratorActive>().Lifetime = 12;
        RandomSet1.GetComponent<GeneratorActive>().EnemyStock = 4;
        RandomSet2.GetComponent<GeneratorActive>().IsUnlimited = false;
    }

    private void Act4_LateGame() {
        var cutscene = Resources.Load<PlayableAsset>("Cutscene/LastStand_Midnight");
        Director.stopped += AfterCutscene_Act4;
        Director.playableAsset = cutscene;
        Director.Play();
    }

    private void Act5_Ending() {
        var cutscene = Resources.Load<PlayableAsset>("Cutscene/LastStand_Ending");
        Director.stopped -= AfterCutscene_Act1;
        Director.playableAsset = cutscene;
        Director.Play();
    }

    private void Start() {
        CreepRush = Resources.Load<GameObject>("Obstacles/Enemy_Rush");
        CreepGuard = Resources.Load<GameObject>("Obstacles/Enemy_Guard");
        Director = GetComponent<PlayableDirector>();
        StageLevel = Stage102State.Prolog; // <---- Cover data
        PlayingTime = Durations[0];
    }

    private void Update() {
        if (PlayingTime > 0) {
            PlayingTime -= Time.deltaTime;
        }

        if (PlayingTime < 0) {
            StageLevel++; 
            if (StageLevel == Stage102State.Epilog) {
                PlayingTime = 0;
                return;
            }
            PlayingTime = Durations[(int)StageLevel];
        }
    }
}

public enum Stage102State {
    Prolog, Warmup, Midnight, Lategame, Epilog,
}