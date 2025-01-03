using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;

[Serializable]
public class SerializableList<T> {
    public List<T> Collection = new();
}

[Serializable]
public class Discuss {
    public string Speaker;
    public string Dialog;
}

public class DialogActive : MonoBehaviour {
    public GameObject Form;
    public TextMeshProUGUI Speaker;
    public TextMeshProUGUI Dialog;

    public List<Discuss> Sentences;
    public TextAsset ScriptText;

    public bool IsOnceTime;
    public bool IsWriting;
    public bool IsCutscene;

    public int Index = 0;

    public int EventCutsceneLoop {
        set {
            var director = transform.root.GetComponent<PlayableDirector>();
            if (value != 0) {
                director.time = value;
            } else {
                InputActive.Instance.OnInteract -= NextSentence;
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator WriteSentence() {
        Speaker.text = Sentences[Index].Speaker;
        foreach (char text in Sentences[Index].Dialog.ToCharArray()) {
            if (Dialog.text != Sentences[Index].Dialog) {
                Dialog.text += text;
                IsWriting = true;
                yield return new WaitForSeconds(0.025f);
            } else {
                break;
            }
        }
        IsWriting = false;
        Index++;
    }

    private void NextSentence() {
        Dialog.text = "";
        if (IsWriting) {
            Dialog.text = Sentences[Index].Dialog;
            IsWriting = false;
        } else {
            var state = Index <= Sentences.Count - 1;
            if (state) {
                StartCoroutine(WriteSentence());
            } else {
                if (IsOnceTime || IsCutscene) {
                    if (IsCutscene) {
                        EventCutsceneLoop = 0;
                    }
                    InputActive.Instance.OnInteract -= NextSentence;
                    gameObject.SetActive(false);
                    if (IsOnceTime) {
                        Destroy(gameObject);
                    }
                }
                Index = 0;
            }
            Form.SetActive(state);
            PlayerActive.Instance.IsInteract = state;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (IsOnceTime) {
            NextSentence();
        }
        InputActive.Instance.OnInteract = NextSentence;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (!IsOnceTime) {
            InputActive.Instance.OnInteract -= NextSentence;
            Index = 0;
        }
    }

    private void Start() {
        Sentences = JsonUtility.FromJson<SerializableList<Discuss>>(ScriptText.text).Collection;
        if (IsCutscene) {
            InputActive.Instance.OnInteract = NextSentence;
            NextSentence();
        }
    }
}
