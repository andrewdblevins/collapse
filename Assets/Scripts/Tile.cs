using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public Transform container;

    public List<SpriteRenderer> Images;

    public void SetHeight(float height) {
        container.localPosition = new Vector3(0f, height, 0f);
    }

    public void Init() {
        SetHeight(Random.Range(-0.1f, 0.1f));

        bool flip = Random.Range(0, 3) == 0;
        if (flip) {
            Images[1].gameObject.SetActive(true);
            flip = Random.Range(0, 3) == 0;
            if (flip) {
                Images[2].gameObject.SetActive(true);
            }
        }
    }

    public void OnMouseDown() {
        Debug.Log("Clicked " + gameObject.name);
    }

    public void OnMouseEnter() {
        foreach (SpriteRenderer s in Images) {
            s.color = Color.yellow;
        }
    }

    public void OnMouseExit() {
        foreach(SpriteRenderer s in Images) {
            s.color = Color.white;
        }
    }
}
