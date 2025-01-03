using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
public class Stage203Des : MonoBehaviour
{
    [Serializable]
    public struct Attribute {
        public string Name;
        public Transform Point;
        public int Qty;
    }


    public Stage203States StageState;
    public Transform Player;
    public Transform Basecamp;
    public Transform Sources;
    public Transform Mayors;
    public Transform Source;
    public Attribute[] Check;
    public string Objective;
    public int Index;

    public PocketActive Pocket { get; set; }
    public PlayableDirector Director { get; set; }
    public Stage203States StageLevel {
        get {
            return StageState;
        }
        set {
            switch (value) {
                case Stage203States.Beginning:
                    // Act1_Beginning(true);
                    break;
                case Stage203States.Explore:
                    Act2_Explore(true);
                    break;
                case Stage203States.Finding:
                    Act3_Finding(true);
                    break;
                case Stage203States.Collecting:
                    Act4_Collecting(true);
                    break;
                case Stage203States.Cook:
                    Act5_Cook(true);
                    break;
                case Stage203States.Following:
                    Act6_Following(true);
                    break;
                case Stage203States.Report:
                    Act7_Report(true);
                    break;
                case Stage203States.Preparation:
                    Act8_Preparation(true);
                    break;
                case Stage203States.Repair:
                    //Act9_Repair(true);
                    break;
            }
            StageState = value;
        }
    }

    private void Act1_Beginning(bool isInit) {
        var cutscene = Resources.Load<PlayableAsset>("Cutscene/Castaway_Openings");
        if (isInit) {
            Director.playableAsset = cutscene;
            Director.Play();
            

        }
        Destroy(transform.Find("Act1").gameObject);
        StageLevel++;
    }

    private void Act2_Explore(bool isInit) {
        if (isInit) {
            var n = transform.Find("Act2").childCount;
            Check = new Attribute[n];
            for (int i = 0; i < transform.Find("Act2").childCount; i++) {
                Check[i].Qty = 1;
                Check[i].Point = transform.Find("Act2").GetChild(i);
                transform.Find("Act2").GetChild(i).gameObject.SetActive(false);
            }
            Check[0].Name = "Telusuri area kuburan";
            Check[1].Name = "Telusuri area kuburan";
            Check[2].Name = "Temukan mayor di Area kuburan";
            Check[3].Name = "Mencari keberadaan rumah mayor";

            Player.position = new Vector3(-11.52f, -35.2f);
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
            StageLevel = Stage203States.Finding;
        }
    }
    private void Act3_Finding(bool isInit) {
        if (isInit) {
            var n = transform.Find("Act3").childCount;
            Check = new Attribute[n];
            for (int i = 0; i < transform.Find("Act3").childCount; i++) {
                Check[i].Qty = 1;
                Check[i].Point = transform.Find("Act3").GetChild(i);
                transform.Find("Act3").GetChild(i).gameObject.SetActive(false);
            }
            Check[0].Name = "Mencari petunjuk";
            Check[1].Name = "Mencari Irish di area sekitar";
            Check[2].Name = "Mencari rumah Mayor";
            Check[3].Name = "Temui Diana";
            Check[4].Name = "Temukan Spice";

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
            Destroy(transform.Find("Act3").gameObject);
            StageLevel = Stage203States.Collecting;
         }
    }
    
    private void Act4_Collecting(bool isInit) {
        Basecamp.gameObject.SetActive(true);
        Sources.gameObject.SetActive(true);
        if (isInit) {
            Check = new[] {
                new Attribute { Qty = 4, Name = "Kumpulkan jagung", Point = Resources.Load<Transform>("ItemWorld/Jagung")},
                new Attribute { Qty = 1, Name = "Ambil spice", Point = Resources.Load<Transform>("ItemWorld/Spice")},   
            };
            Pocket.ExtraEvent+= ExtraEvent_Act4;
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
                Destroy(transform.Find("Act4").gameObject);
                Pocket.HeadDisplaySet.SetActive(false);
                Pocket.ExtraEvent -= ExtraEvent_Act4;
                StageLevel = Stage203States.Cook;
            }
        }
    }



    private void Act5_Cook(bool isInit) {
        if (isInit) {
            Check = new[] {
                new Attribute { Qty = 4, Name = "Berikan jagung ke dalam pot", Point = Resources.Load<Transform>("ItemWorld/Jagung")},
                new Attribute { Qty = 1, Name = "Berikan spice ke dalam pot", Point = Resources.Load<Transform>("ItemWorld/Spice")},
            };
            Pocket.ExtraEvent += ExtraEvent_Act5;
            Objective = Check[0].Name + " " + Check[0].Qty;
            for (int i = 0; i < Check.Length; i++) {
                transform.Find("Act5").GetChild(i).gameObject.SetActive(false);
            }
            GuiActive.Instance.ShowObjective(true);
            GuiActive.Instance.Request[0].sprite = Check[0].Point.GetComponent<SpriteRenderer>().sprite;
            GuiActive.Instance.Request[1].sprite = Check[1].Point.GetComponent<SpriteRenderer>().sprite;
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
            for (int i = 0; i < Check.Length; i++) {
                if (transform.Find("Act5")) {
                    var child = transform.Find("Act5").GetChild(i).gameObject;
                    if (!child.activeInHierarchy) {
                        return;
                    }
                } else {
                    return;
                }
            }
            var potboiler = Resources.Load<GameObject>("Quest/PotBoiler");
            Instantiate(potboiler, Basecamp.position, Quaternion.identity, Basecamp);
            Basecamp.GetComponent<CircleCollider2D>().enabled = true;
            Basecamp.GetComponent<SpriteRenderer>().enabled = false;
            InputActive.Instance.OnInteract = null;

            Destroy(transform.Find("Act5").gameObject);
            StageLevel = Stage203States.Following;
        }
    }
    
    private void Act6_Following(bool isInit) {
        if (isInit) {
            var n = transform.Find("Act6").childCount;
            Check = new Attribute[n];
            for (int i = 0; i < transform.Find("Act6").childCount; i++) {
                Check[i].Qty = 1;
                Check[i].Point = transform.Find("Act6").GetChild(i);
                transform.Find("Act6").GetChild(i).gameObject.SetActive(false);
            }
            Check[0].Name = "Melapor kepada istri mayor";
            Check[1].Name = "Menemukan petunjuk baru";
            Check[2].Name = "Temui tukang kayu";
            Check[3].Name = "Pergi mencari istri mayor";
            Check[4].Name = "Mencari polish stone";
            Check[5].Name = "Mencari Mayor untuk melapor";
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
            Destroy(transform.Find("Act6").gameObject);
            StageLevel = Stage203States.Report;
         }
    }

    private void Act7_Report(bool isInit)
    {
        Mayors.gameObject.SetActive(true);
        Source.gameObject.SetActive(true);
        if (isInit)
        {
            Check = new[] {
                new Attribute { Qty = 5, Name = "Kumpulkan Batu", Point = Resources.Load<Transform>("ItemWorld/Stones")},
                new Attribute { Qty = 5, Name = "Kumpulkan Kayu", Point = Resources.Load<Transform>("ItemWorld/Lumbers")},
                new Attribute { Qty = 1, Name = "Mencari Polish stone", Point = Resources.Load<Transform>("ItemWorld/Polish")},
            };
            Pocket.ExtraEvent+= ExtraEvent_Act7;
            Objective = Check[0].Name;
            GuiActive.Instance.ShowObjective(true);
            Index = 0;
        }
        else
        {
            var isCheck = true;
            for (int i = 0; i < Check.Length; i++)
            {
                if (Check[i].Qty > 0)
                {
                    Objective = Check[i].Name + " x" + Check[i].Qty;
                    isCheck = false;
                }
            }

            if (isCheck)
            {
                Destroy(transform.Find("Act7").gameObject);
                Pocket.HeadDisplaySet.SetActive(false);
                Pocket.ExtraEvent -= ExtraEvent_Act7;
                StageLevel = Stage203States.Preparation;
            }
        }
    }

    private void Act8_Preparation(bool isInit) {
        if (isInit) {
            Check = new[] {
                new Attribute { Qty = 5, Name = "Berikan Batu ke Mayor", Point = Resources.Load<Transform>("ItemWorld/Stones")},
                new Attribute { Qty = 5, Name = "Berikan Kayu ke Mayor", Point = Resources.Load<Transform>("ItemWorld/Lumbers")},
                new Attribute { Qty = 1, Name = "Berikan Polish Stone", Point = Resources.Load<Transform>("ItemWorld/Polish")},
            };
            Pocket.ExtraEvent += ExtraEvent_Act8;
            Objective = Check[0].Name + " " + Check[0].Qty;
            for (int i = 0; i < Check.Length; i++) {
                transform.Find("Act8").GetChild(i).gameObject.SetActive(false);
            }
            GuiActive.Instance.ShowObjective(true);
            GuiActive.Instance.Request[0].sprite = Check[0].Point.GetComponent<SpriteRenderer>().sprite;
            GuiActive.Instance.Request[1].sprite = Check[1].Point.GetComponent<SpriteRenderer>().sprite;
            GuiActive.Instance.Request[2].sprite = Check[2].Point.GetComponent<SpriteRenderer>().sprite;
            Index = 0;
        } else {
            var distance = Vector3.Distance(Player.position, Mayors.position);
            if (distance < 2f)
            {
                InputActive.Instance.OnInteract = Pocket.PocketHandler;
            } else if (distance > 2f && distance < 5f) {
                InputActive.Instance.OnInteract -= Pocket.PocketHandler;
            }
            GuiActive.Instance.TxtRequest[0].text = Check[0].Qty.ToString();
            GuiActive.Instance.TxtRequest[1].text = Check[1].Qty.ToString();
            GuiActive.Instance.TxtRequest[2].text = Check[2].Qty.ToString();
            for (int i = 0; i < Check.Length; i++) {
                if (transform.Find("Act8")) {
                    var child = transform.Find("Act8").GetChild(i).gameObject;
                    if (!child.activeInHierarchy) {
                        return;
                    }
                } else {
                    return;
                }
            }
            var mayor = Resources.Load<GameObject>("Quest/Mayor");
            Instantiate(mayor, Mayors.position, Quaternion.identity, Mayors);
            Mayors.GetComponent<CircleCollider2D>().enabled = true;
            Mayors.GetComponent<SpriteRenderer>().enabled = false;
            InputActive.Instance.OnInteract = null;

            Destroy(transform.Find("Act8").gameObject);
            StageLevel = Stage203States.Repair;
        }
    }
    public bool ExtraEvent_Act4(int n) {
        var result = false;
        for (int i = 0; i < Check.Length; i++) {
            if (Pocket.Itemhand.Point == Check[i].Point.gameObject) {
                Check[i].Qty -= Mathf.Clamp(1, 0, 9);
                result = true;
            }
        }
        return result;
    }

    private bool ExtraEvent_Act5(int n) {
        var result = false;

        var distance = Vector3.Distance(Player.position, Basecamp.position);
        for (int i = 0; i < Check.Length && distance < 2f; i++) {
            var check = Check[i].Point.gameObject == Pocket.Pockets[n].Point;
            if (Pocket.Pockets[n].Qty > 0 && Check[i].Point.gameObject == Pocket.Pockets[n].Point) {
                Check[i].Qty -= Mathf.Clamp(1, 0, 9);
                var state = Check[i].Qty <= 0;
                if (state) {
                    var remain = i + 1;
                    if (remain >= Check.Length) {
                        remain = 0;
                    }
                    Objective = Check[remain].Name + " " + Check[remain].Qty;
                    GuiActive.Instance.ShowObjective(state);
                    transform.Find("Act5").GetChild(i).gameObject.SetActive(state);
                } else {
                    Objective = Check[i].Name + " " + Check[i].Qty;
                }
                result = true;
                transform.Find("Act5").GetChild(i).gameObject.SetActive(Check[i].Qty <= 0);
            }
        }
        return result;
    }

    public bool ExtraEvent_Act7(int n)
    {
        var result = false;
        for (int i = 0; i < Check.Length; i++)
        {
            if (Pocket.Itemhand.Point == Check[i].Point.gameObject)
            {
                Check[i].Qty -= Mathf.Clamp(1, 0, 9);
                result = true;
            }
        }
        return result;
    }

    public bool ExtraEvent_Act8(int n) {
        var result = false;

        var distance = Vector3.Distance(Player.position, transform.Find("Act8").position);
        for (int i = 0; i < Check.Length && distance < 2f; i++) {
            var check = Check[i].Point.gameObject == Pocket.Pockets[n].Point;
            if (Pocket.Pockets[n].Qty > 0 && Check[i].Point.gameObject == Pocket.Pockets[n].Point) {
                Check[i].Qty -= Mathf.Clamp(1, 0, 9);
                var state = Check[i].Qty <= 0;
                if (state) {
                    var remain = i + 1;
                    if (remain >= Check.Length) {
                        remain = 0;
                    }
                    Objective = Check[remain].Name + " " + Check[remain].Qty;
                    GuiActive.Instance.ShowObjective(state);
                    transform.Find("Act8").GetChild(i).gameObject.SetActive(state);
                } else {
                    Objective = Check[i].Name + " " + Check[i].Qty;
                }
                result = true;
                transform.Find("Act8").GetChild(i).gameObject.SetActive(Check[i].Qty <= 0);
            }
        }
        return result;
    }


    private void Start() {
        Director = GetComponent<PlayableDirector>();
        Pocket = Player.GetComponent<PocketActive>();
        StageLevel = Stage203States.Explore;
    }

    private void Update() {
        switch (StageLevel) {
            case Stage203States.Beginning:
                // Act1_Beginning(false);
                break;
            case Stage203States.Explore:
                Act2_Explore(false);
                break;
            case Stage203States.Finding:
                Act3_Finding(false);
                break;
            case Stage203States.Collecting:
                Act4_Collecting(false);
                break;
            case Stage203States.Cook:
                Act5_Cook(false);
                break;
            case Stage203States.Following:
                Act6_Following(false);
                break;
            case Stage203States.Report:
                Act7_Report(false);
                break;
            case Stage203States.Preparation:
                Act8_Preparation(false);    
                break;
            case Stage203States.Repair:
                //Act9_Repair(false);
                break;
        }
        GuiActive.Instance.TmpObjective.text = Objective;
    }
}

public enum Stage203States {
    Beginning, Explore, Finding, 
    Collecting, Cook, Following, Report, Preparation, 
    Repair,  
    
}