using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {
    Dictionary<string, AudioClip> audioClips;
    float soundCooldownRemaining = 0;
    float soundCooldown = 0.2f;

    // Start is called before the first frame update
    void Start() {
        LoadAudioClips();
        WorldController.Instance.world.RegisterFurnitureCreated(OnFurnitureCreated);
        WorldController.Instance.world.RegisterTileChanged(OnTileTypeChanged);
    }

    void LoadAudioClips() {
        audioClips = new Dictionary<string, AudioClip>();
        string toLoad;

        LoadAudioClip("Floor_OnCreated");
        LoadAudioClip("Wall_OnCreated");
    }

    void LoadAudioClip(string res) {
        string path = "Sounds/" + res;
        AudioClip ac = Resources.Load<AudioClip>(path);
        if (ac == null) {
            Debug.LogError("LoadAudioClip -- Clip at: " + path + " not found!");
            return;
        }
        audioClips[res] = ac;
    }

    // Update is called once per frame
    void Update() {
        soundCooldownRemaining -= Time.deltaTime;
    }

    public void OnTileTypeChanged(Tile tile_data) {
        if (OnCooldown()) return;
        AudioClip ac = audioClips["Floor_OnCreated"];
        if (ac == null) {
            Debug.LogError("OnTileTypeChanged -- Sound file missing!");
            return;
        }
        PlaySound(ac);
    }

    public void OnFurnitureCreated(Furniture furn) {
        if (OnCooldown()) return;
        AudioClip ac = audioClips[furn.objectType + "_OnCreated"];
        if (ac == null) {
            Debug.LogError("OnFurnitureCreated -- Sound file for " + furn.objectType + " is missing!");
            return;
        }
        PlaySound(ac);
    }

    bool OnCooldown() {
        return soundCooldownRemaining > 0;
    }

    void PlaySound(AudioClip ac) {
        AudioSource.PlayClipAtPoint(ac, Camera.main.transform.position);
        soundCooldownRemaining = soundCooldown;
    }
}
