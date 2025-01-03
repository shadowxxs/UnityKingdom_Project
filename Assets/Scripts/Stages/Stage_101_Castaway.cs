using System;
using UnityEngine;
using UnityEngine.Playables;

public class Stage_101_Castaway : MonoBehaviour {
    [Serializable]
    public struct Attribute {
        public string Name;
        public Transform Point;
        public int Qty;
    }

    public Stage101State StageState;
    public Transform Player;
    public Transform Basecamp;
    public Transform Sources;
    public Attribute[] Check;
    public string Objective;
    public int Index;

    public PocketActive Pocket { get; set; }
    public PlayableDirector Director { get; set; }
    public Action ActCallback { get; set; }

    public Stage101State StageLevel {
        get {
            return StageState;
        }
        set {
            switch (value) {
                case Stage101State.Prolog:
                    Act1_Prolog(true);
                    break;
                case Stage101State.Explore:
                    Act2_Explore(true);
                    break;
                case Stage101State.Campfire:
                    Act3_Campfire(true);
                    break;
                case Stage101State.LightUp:
                    Act4_LightUp(true);
                    break;
                case Stage101State.Hungry:
                    Act5_Hungry(true);
                    break;
                case Stage101State.Equipment:
                    Act6_Equipment(true);
                    break;
                case Stage101State.Enemy:
                    Act7_KillEnemy(true);
                    break;
                case Stage101State.SpawnPit:
                    Act8_SpawnPit(true);
                    break;
                case Stage101State.Survive:
                    Act9_Survive(true);
                    break;
                case Stage101State.ExitPath:
                    Act10_ExitPath(true);
                    break;
                case Stage101State.Epilog:
                    Act11_Epilog(true);
                    break;
            }
            StageState = value;
        }
    }

    private void Act1_Prolog(bool isInit) {
        if (isInit) {
            var cutscene = Resources.Load<PlayableAsset>("Cutscene/Castaway_Opening");
            Director.stopped += AfterCutscene_Act1;
            Director.playableAsset = cutscene;
            Director.Play();
        } 
    }

    private void Act2_Explore(bool isInit) {
        if (isInit) {
            Director.stopped -= AfterCutscene_Act1;

            var n = transform.Find("Act2").childCount;
            Check = new Attribute[n];
            for (int i = 0; i < transform.Find("Act2").childCount; i++) {
                Check[i].Qty = 1;
                Check[i].Point = transform.Find("Act2").GetChild(i);
                transform.Find("Act2").GetChild(i).gameObject.SetActive(false);
            }
            Check[0].Name = "Telusuri area sekitar";
            Check[1].Name = "Temukan jalan menuju pantai";
            Check[2].Name = "Telusuri pantai";
            Check[3].Name = "Naik ke dataran lebih tinggi";
            Check[4].Name = "Temukan lapangan hijau";

            Objective = Check[0].Name;
            GuiActive.Instance.ShowObjective(true);
            Check[0].Point.gameObject.SetActive(true);
            Index = 0;
        } else {
            if (Index < Check.Length) {
                var distance = Vector3.Distance(Player.position, Check[Index].Point.position);
                if (distance < 2f) {
                    Check[Index].Point.GetComponent<SpriteRenderer>().enabled = false;
                    if (++Index < Check.Length) {
                        Objective = Check[Index].Name;
                        Check[Index].Point.gameObject.SetActive(true);
                    }
                }
                return;
            }
            Destroy(transform.Find("Act2").gameObject);
            StageLevel = Stage101State.Campfire;
        }
    }

    private void Act3_Campfire(bool isInit) {
        Basecamp.gameObject.SetActive(true);
        Sources.gameObject.SetActive(true);
        if (isInit) {
            Check = new[] {
                new Attribute { Qty = 3, Name = "Kumpulkan kayu", Point = Resources.Load<Transform>("ItemWorld/Lumber")},
                new Attribute { Qty = 3, Name = "Kumpulkan batu", Point = Resources.Load<Transform>("ItemWorld/Stone") },
                new Attribute { Qty = 3, Name = "Kumpulkan daun", Point = Resources.Load<Transform>("ItemWorld/Fiber")},
            };
            Pocket.ExtraEvent += ExtraEvent_Act3;
            Objective = Check[0].Name; 
            GuiActive.Instance.ShowObjective(true);
            Index = 0;
        } else {
            var isCheck = true;
            for (int i = 0; i < Check.Length; i++) {
                if (Check[i].Qty > 0) {
                    Objective = Check[i].Name + " x" + Check[i].Qty;
                    isCheck = false;
                }
            }

            if (isCheck) {
                Destroy(transform.Find("Act3").gameObject);
                Pocket.HeadDisplaySet.SetActive(false);
                Pocket.ExtraEvent -= ExtraEvent_Act3;
                StageLevel = Stage101State.LightUp;
            }
        }
    }

    private void Act4_LightUp(bool isInit) {
        if (isInit) {
            Director.playableAsset = null;
            Check = new[] {
                new Attribute { Qty = 5, Name = "Berikan kayu ke perapian", Point = Resources.Load<Transform>("ItemWorld/Lumber")},
                new Attribute { Qty = 5, Name = "Berikan batu ke perapian", Point = Resources.Load<Transform>("ItemWorld/Stone") },
                new Attribute { Qty = 5, Name = "Berikan daun ke perapian", Point = Resources.Load<Transform>("ItemWorld/Fiber")},
            };
            Pocket.ExtraEvent += ExtraEvent_Act4;
            Objective = Check[0].Name + " " + Check[0].Qty;
            for (int i = 0; i < Check.Length; i++) {
                transform.Find("Act4").GetChild(i).gameObject.SetActive(false);
            }
            GuiActive.Instance.ShowObjective(true);
            GuiActive.Instance.Request[0].sprite = Check[0].Point.GetComponent<SpriteRenderer>().sprite;
            GuiActive.Instance.Request[1].sprite = Check[1].Point.GetComponent<SpriteRenderer>().sprite;
            GuiActive.Instance.Request[2].sprite = Check[2].Point.GetComponent<SpriteRenderer>().sprite;
            Index = 0;
        } else {
            var distance = Vector3.Distance(Player.position, Basecamp.position);
            if (distance < 2f) {
                InputActive.Instance.OnInteract = Pocket.PocketHandler;
            } else if (distance > 2f && distance < 5f) {
                InputActive.Instance.OnInteract -= Pocket.PocketHandler;
            }
            GuiActive.Instance.TxtRequest[0].text = Check[0].Qty.ToString();
            GuiActive.Instance.TxtRequest[1].text = Check[1].Qty.ToString();
            GuiActive.Instance.TxtRequest[2].text = Check[2].Qty.ToString();
            for (int i = 0; i < Check.Length; i++) {
                if (transform.Find("Act4")) {
                    var child = transform.Find("Act4").GetChild(i).gameObject;
                    if (!child.activeInHierarchy) {
                        return;
                    }
                } else {
                    return;
                }
            }

            var cutscene = Resources.Load<PlayableAsset>("Cutscene/Castaway_LightUp");
            Director.stopped += AfterCutscene_Act4;
            Director.playableAsset = cutscene;
            Director.Play();

            Destroy(transform.Find("Act4").gameObject);
        }
    }

    private void Act5_Hungry(bool isInit) {
        if (isInit) {
            Director.stopped -= AfterCutscene_Act4;

            var bonfire = Resources.Load<GameObject>("Quest/BonFire");
            Instantiate(bonfire, Basecamp.position, Quaternion.identity, Basecamp);
            Basecamp.GetComponent<CircleCollider2D>().enabled = true;
            Basecamp.GetComponent<SpriteRenderer>().enabled = false;
            InputActive.Instance.OnInteract = null;

            var n = transform.Find("Act5").childCount;
            Check = new Attribute[n];
            for (int i = 0; i < transform.Find("Act5").childCount; i++) {
                Check[i].Qty = 1;
                Check[i].Point = transform.Find("Act5").GetChild(i);
                transform.Find("Act5").GetChild(i).gameObject.SetActive(false);
            }
            Check[0].Name = "Temukan tanaman hidup";
            Check[1].Name = "Telusuri tebing";
            Check[2].Name = "Periksa titik awal";
            Check[3].Name = "Cari peralatan yang bisa digunakan";
            Check[3].Name = "Cari peralatan yang bisa digunakan";

            Objective = Check[0].Name;
            GuiActive.Instance.ShowObjective(true);
            Check[0].Point.gameObject.SetActive(true);
            Index = 0;
        } else {
            if (Index < Check.Length) {
                var distance = Vector3.Distance(Player.position, Check[Index].Point.position);
                // ToDo: Update UI Distance
                if (distance < 2f) {
                    switch (Index) {
                        case 0:
                            var berry = Resources.Load<GameObject>("PowerPlant/PlantWildBerry");
                            Instantiate(berry, new Vector3(6.12f, -11.75f), Quaternion.identity, Sources);
                            Instantiate(berry, new Vector3(8.08f, -9.7f), Quaternion.identity, Sources);
                            Instantiate(berry, new Vector3(25.53f, -21.73f), Quaternion.identity, Sources);
                            Instantiate(berry, new Vector3(16.53f, -41.63f), Quaternion.identity, Sources);
                            break;
                        case 1:
                            var portal = Resources.Load<GameObject>("Quest/StackPile");
                            Instantiate(portal, new Vector3(13.1f, -10.17f), Quaternion.identity, transform.Find("Act10"));
                            break;
                        case 2:
                            var hole = Resources.Load<GameObject>("Obstacles/HoleTrap");
                            var enemy = Resources.Load<GameObject>("Obstacles/Enemy_Guard");

                            hole.GetComponent<GeneratorActive>().EnemySeed = enemy;
                            hole.GetComponent<GeneratorActive>().Lifetime = 1;
                            hole.GetComponent<GeneratorActive>().EnemyStock = 1;
                            hole.GetComponent<GeneratorActive>().enabled = true;
                            enemy.GetComponent<CreepGuardActive>().enabled = false;
                            
                            var spawner = Instantiate(hole, new Vector3(37.37f, -12.5f), Quaternion.identity, transform.Find("Act7"));
                            var creep = enemy.transform;
                            spawner.GetComponent<GeneratorActive>().ExtraAction = (e) => {
                                e.GetComponent<CreepGuardActive>().OnEliminate += () => {
                                    Destroy(transform.Find("Act7").gameObject);
                                    StageLevel = Stage101State.SpawnPit;
                                };
                                creep = e;
                            };
                            ActCallback = () => {
                                Check = new[] { new Attribute { Qty = 1, Name = "Bunuh Creep Guard", Point = creep } };
                                creep.GetComponent<CreepGuardActive>().enabled = true;
                                enemy.GetComponent<CreepGuardActive>().enabled = true;
                            };
                            break;
                        case 3:
                            var equip = Resources.Load<GameObject>("Quest/Stash");
                            Instantiate(equip, new Vector3(42.82f, -41.43f), Quaternion.identity, transform.Find("Act6"));
                            break;
                    }
                    Check[Index].Point.GetComponent<SpriteRenderer>().enabled = false;
                    if (++Index < Check.Length) {
                        Check[Index].Point.gameObject.SetActive(true);
                        Objective = Check[Index].Name;
                    }
                }
                return;
            }
            Destroy(transform.Find("Act5").gameObject);
            StageLevel = Stage101State.Equipment;
        }
    }

    private void Act6_Equipment(bool isInit) {
        if (isInit) {
            Pocket.ExtraEvent -= ExtraEvent_Act6;
            Director.playableAsset = null;
            Check = new[] {
                new Attribute { Qty = 5, Name = "Siapkan kebutuhan batu ke peralatan", Point = Resources.Load<Transform>("ItemWorld/Stone")},
                new Attribute { Qty = 5, Name = "Siapkan kebutuhan beri ke peralatan", Point = Resources.Load<Transform>("ItemWorld/Berry") },
                new Attribute { Qty = 5, Name = "Siapkan kebutuhan kayu ke peralatan", Point = Resources.Load<Transform>("ItemWorld/Lumber")},
            };
            Pocket.ExtraEvent += ExtraEvent_Act6;
            Objective = Check[0].Name;
            for (int i = 0; i < Check.Length; i++) {
                transform.Find("Act6").GetChild(i).gameObject.SetActive(false);
            }
            GuiActive.Instance.ShowObjective(true);
            GuiActive.Instance.Request[0].sprite = Check[0].Point.GetComponent<SpriteRenderer>().sprite;
            GuiActive.Instance.Request[1].sprite = Check[1].Point.GetComponent<SpriteRenderer>().sprite;
            GuiActive.Instance.Request[2].sprite = Check[2].Point.GetComponent<SpriteRenderer>().sprite;
            Index = 0;
        } else {
            var distance = Vector3.Distance(Player.position, transform.Find("Act6").Find("Stash(Clone)").position);
            if (distance < 2f) {
                InputActive.Instance.OnInteract = Pocket.PocketHandler;
            } else if (distance > 2f && distance < 5f) {
                InputActive.Instance.OnInteract -= Pocket.PocketHandler;
            }
            GuiActive.Instance.TxtRequest[0].text = Check[0].Qty.ToString();
            GuiActive.Instance.TxtRequest[1].text = Check[1].Qty.ToString();
            GuiActive.Instance.TxtRequest[2].text = Check[2].Qty.ToString();
            for (int i = 0; i < Check.Length; i++) {
                if (transform.Find("Act6")) {
                    var child = transform.Find("Act6").GetChild(i).gameObject;
                    if (!child.activeInHierarchy) {
                        return;
                    }
                } else {
                    return;
                }
            }

            Pocket.ExtraEvent -= ExtraEvent_Act6;
            Destroy(transform.Find("Act6").gameObject);
            StageLevel = Stage101State.Enemy;
        }
    }
        
    private void Act7_KillEnemy(bool isInit) {
        if (isInit) {
            ActCallback?.Invoke();
        } 
    }
    
    private void Act8_SpawnPit(bool isInit) {
        if (isInit) {
            var cutscene = Resources.Load<PlayableAsset>("Cutscene/Castaway_LightUp");
            Director.stopped += AfterCutscene_Act8;
            Director.playableAsset = cutscene;
            Director.Play();
        } 
    }
    
    private void Act9_Survive(bool isInit) {
        if (isInit) {
            Director.stopped -= AfterCutscene_Act8;

            var hole = Resources.Load<GameObject>("Obstacles/HoleTrap");
            var enemy = Resources.Load<GameObject>("Obstacles/Enemy_Guard");

            Check = new[] { new Attribute { Qty = 3, Name = "Bunuh Creep Guard", Point = enemy.transform } };
            enemy.GetComponent<CreepGuardActive>().enabled = true;
            hole.GetComponent<GeneratorActive>().Lifetime = 3;
            hole.GetComponent<GeneratorActive>().EnemyStock = 3;
            hole.GetComponent<GeneratorActive>().EnemySeed = enemy;
            hole.GetComponent<GeneratorActive>().enabled = true;

            var spawner = Instantiate(hole, new Vector3(37.37f, -12.5f), Quaternion.identity, transform.Find("Act9"));
            spawner.GetComponent<GeneratorActive>().ExtraAction = (e) => {
                e.GetComponent<CreepGuardActive>().OnEliminate += () => {
                    Objective = $"Bunuh {--Check[0].Qty} Creep";
                };
            };
            Objective = $"Bunuh 3x Creep";
            GuiActive.Instance.ShowObjective(true);
        } else {
            if (Check[0].Qty <= 0) {
                Destroy(transform.Find("Act9").gameObject);
                StageLevel = Stage101State.ExitPath;
            }
        }
    }
    
    private void Act10_ExitPath(bool isInit) {
        if (isInit) {
            Pocket.ExtraEvent -= ExtraEvent_Act10;
            Director.playableAsset = null;
            Check = new[] {
                new Attribute { Qty = 5, Name = "Siapkan kebutuhan batu membantu pembakaran", Point = Resources.Load<Transform>("ItemWorld/Stone")},
                new Attribute { Qty = 5, Name = "Siapkan kebutuhan beri menambah energi", Point = Resources.Load<Transform>("ItemWorld/Berry") },
                new Attribute { Qty = 5, Name = "Siapkan kebutuhan kayu bahan bakar", Point = Resources.Load<Transform>("ItemWorld/Lumber")},
                new Attribute { Qty = 5, Name = "Siapkan kebutuhan daun untuk alas", Point = Resources.Load<Transform>("ItemWorld/Fiber")},
                new Attribute { Qty = 5, Name = "Siapkan kebutuhan tulang bahan bakar", Point = Resources.Load<Transform>("ItemWorld/Bone")},
            };
            Pocket.ExtraEvent += ExtraEvent_Act10;
            Objective = Check[0].Name;
            for (int i = 0; i < Check.Length; i++) {
                transform.Find("Act10").GetChild(i).gameObject.SetActive(false);
            }
            GuiActive.Instance.ShowObjective(true);
            GuiActive.Instance.Request[0].sprite = Check[0].Point.GetComponent<SpriteRenderer>().sprite;
            GuiActive.Instance.Request[1].sprite = Check[1].Point.GetComponent<SpriteRenderer>().sprite;
            GuiActive.Instance.Request[2].sprite = Check[2].Point.GetComponent<SpriteRenderer>().sprite;
            GuiActive.Instance.Request[3].sprite = Check[3].Point.GetComponent<SpriteRenderer>().sprite;
            GuiActive.Instance.Request[4].sprite = Check[4].Point.GetComponent<SpriteRenderer>().sprite;
            Index = 0;
        } else {
            var distance = Vector3.Distance(Player.position, transform.Find("Act10").Find("StackPile(Clone)").position);
            if (distance < 2f) {
                InputActive.Instance.OnInteract = Pocket.PocketHandler;
            } else if (distance > 2f && distance < 5f) {
                InputActive.Instance.OnInteract -= Pocket.PocketHandler;
            }
            GuiActive.Instance.TxtRequest[0].text = Check[0].Qty.ToString();
            GuiActive.Instance.TxtRequest[1].text = Check[1].Qty.ToString();
            GuiActive.Instance.TxtRequest[2].text = Check[2].Qty.ToString();
            GuiActive.Instance.TxtRequest[3].text = Check[3].Qty.ToString();
            GuiActive.Instance.TxtRequest[4].text = Check[4].Qty.ToString();
            for (int i = 0; i < Check.Length; i++) {
                if (transform.Find("Act10")) {
                    var child = transform.Find("Act10").GetChild(i).gameObject;
                    if (!child.activeInHierarchy) {
                        return;
                    }
                } else {
                    return;
                }
            }

            Pocket.ExtraEvent -= ExtraEvent_Act10;
            Destroy(transform.Find("Act10").gameObject);
            StageLevel = Stage101State.Epilog;
        }
    }

    private void Act11_Epilog(bool isInit) {
        if (isInit) {
            var cutscene = Resources.Load<PlayableAsset>("Cutscene/Castaway_Opening");
            Director.stopped += AfterCutscene_Act11;
            Director.playableAsset = cutscene;
            Director.Play();
        }
    }

    private void AfterCutscene_Act1(PlayableDirector e) {
        Destroy(transform.Find("Act1").gameObject);
        StageLevel = Stage101State.Explore;
    }

    private void AfterCutscene_Act4(PlayableDirector e) {
        Objective = "Objektif Selesai";
        Pocket.ExtraEvent -= ExtraEvent_Act4;
        StageLevel = Stage101State.Hungry;
    }

    private void AfterCutscene_Act8(PlayableDirector e) {
        Destroy(transform.Find("Act8").gameObject);
        StageLevel = Stage101State.Survive;
    }

    private void AfterCutscene_Act11(PlayableDirector e) {
        Destroy(transform.Find("Act11").gameObject);
        //HUD game Over
    }

    private bool ExtraEvent_Act3(int n) {
        var result = false;
        for (int i = 0; i < Check.Length; i++) {
            if (Pocket.Itemhand.Point == Check[i].Point.gameObject) {
                Check[i].Qty -= Mathf.Clamp(1, 0, 9);
                result = true;
            }
        }
        return result;
    }
    
    private bool ExtraEvent_Act4(int n) {
        var result = false;
        var distance = Vector3.Distance(Player.position, Basecamp.position);
        for (int i = 0; i < Check.Length && distance < 2f; i++) {
            var check = Check[i].Point.gameObject == Pocket.Pockets[n].Point;
            if (Pocket.Pockets[n].Qty > 0 && Check[i].Qty > 0 && check) {
                Check[i].Qty -= Mathf.Clamp(1, 0, 9);
                var state = Check[i].Qty <= 0;
                if (state) {
                    var remain = i + 1;
                    if (remain >= Check.Length) {
                        remain = 0;
                    }
                    Objective = Check[remain].Name + " " + Check[remain].Qty;
                    GuiActive.Instance.ShowObjective(state);
                    transform.Find("Act4").GetChild(i).gameObject.SetActive(state);
                } else {
                    Objective = Check[i].Name + " " + Check[i].Qty;
                }
                result = true;
            }
        }
        return result;
    }
    
    private bool ExtraEvent_Act6(int n) {
        var result = false;
        var distance = Vector3.Distance(Player.position, transform.Find("Act6").Find("Stash(Clone)").position);
        for (int i = 0; i < Check.Length && distance < 2f; i++) {
            var check = Check[i].Point.gameObject == Pocket.Pockets[n].Point;
            if (Pocket.Pockets[n].Qty > 0 && Check[i].Qty > 0 && check) {
                Check[i].Qty -= Mathf.Clamp(1, 0, 9);
                var state = Check[i].Qty <= 0;
                if (state) {
                    var remain = i + 1;
                    if (remain >= Check.Length) {
                        remain = 0;
                    }
                    Objective = Check[remain].Name + " " + Check[remain].Qty;
                    GuiActive.Instance.ShowObjective(state);
                    transform.Find("Act6").GetChild(i).gameObject.SetActive(state);
                } else {
                    Objective = Check[i].Name + " " + Check[i].Qty;
                }
                result = true;
            }
        }
        return result;
    }

    private bool ExtraEvent_Act10(int n) {
        var result = false;
        var distance = Vector3.Distance(Player.position, transform.Find("Act10").Find("StackPile(Clone)").position);
        for (int i = 0; i < Check.Length && distance < 2f; i++) {
            var check = Check[i].Point.gameObject == Pocket.Pockets[n].Point;
            if (Pocket.Pockets[n].Qty > 0 && Check[i].Qty > 0 && check) {
                Check[i].Qty -= Mathf.Clamp(1, 0, 9);
                var state = Check[i].Qty <= 0;
                if (state) {
                    var remain = i + 1;
                    if (remain >= Check.Length) {
                        remain = 0;
                    }
                    Objective = Check[remain].Name + " " + Check[remain].Qty;
                    GuiActive.Instance.ShowObjective(state);
                    transform.Find("Act10").GetChild(i).gameObject.SetActive(state);
                } else {
                    Objective = Check[i].Name + " " + Check[i].Qty;
                }
                result = true;
            }
        }
        return result;
    }
   
    private void Start() {
        Director = GetComponent<PlayableDirector>();
        Pocket = Player.GetComponent<PocketActive>();
        StageLevel = Stage101State.Prolog;

        //Destroy(transform.Find("Act1").gameObject);
        //Destroy(transform.Find("Act2").gameObject);
        //Destroy(transform.Find("Act3").gameObject);
        //Destroy(transform.Find("Act4").gameObject);
        //Destroy(transform.Find("Act5").gameObject);
        //Destroy(transform.Find("Act6").gameObject);
        //Destroy(transform.Find("Act7").gameObject);
        //Destroy(transform.Find("Act8").gameObject);
        //Destroy(transform.Find("Act9").gameObject);
    }

    private void Update() {
        switch (StageLevel) {
            case Stage101State.Prolog:
                Act1_Prolog(false);
                break;
            case Stage101State.Explore:
                Act2_Explore(false);
                break;
            case Stage101State.Campfire:
                Act3_Campfire(false);
                break;
            case Stage101State.LightUp:
                Act4_LightUp(false);
                break;
            case Stage101State.Hungry:
                Act5_Hungry(false);
                break;
            case Stage101State.Equipment:
                Act6_Equipment(false);
                break;
            case Stage101State.Enemy:
                Act7_KillEnemy(false);
                break;
            case Stage101State.SpawnPit:
                Act8_SpawnPit(false);
                break;
            case Stage101State.Survive:
                Act9_Survive(false);
                break;
            case Stage101State.ExitPath:
                Act10_ExitPath(false);
                break;
        }
        GuiActive.Instance.TmpObjective.text = Objective;
    }
}

public enum Stage101State {
    Prolog, Explore, Campfire, LightUp,
    Hungry, Equipment, Enemy,
    SpawnPit, Survive, ExitPath, Epilog,
}