﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFactory : MonoBehaviour {
    public GameObject waveController;
    public List<Settings> settings = new List<Settings>();
    private List<GameObject> waves = new List<GameObject>();

    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private List<GameObject> upgradeObjects = new List<GameObject>();
    private List<GameObject> spawnedObjs = new List<GameObject>();
    [System.Serializable]
    public struct Settings {
        public string name;
        public float remainingTime;
        public List<MinionSettings> minionSettings;

        public Settings(string name, int minMelee, int minRanged, float freqMelee, float freqRanged, float spawnMelee, float spawnRanged, float remainingTime) {
            this.name = name;
            this.remainingTime = remainingTime;
            this.minionSettings = new List<MinionSettings>();
        }
    }

    [System.Serializable]
    public struct MinionSettings {
        public string tag;
        public int minimum;
        public float frequency;
        public int spawnNo;
        public float multiplier;

        public MinionSettings(string tag, int minimum, float frequency, int spawnNo, float multiplier) {
            this.tag = tag;
            this.minimum = minimum;
            this.frequency = frequency;
            this.spawnNo = spawnNo;
            this.multiplier = multiplier;
        }
    }
    
    private void Awake() {
        this.waveController.SetActive(false);   // Waves aren't enabled by default.

        this.settings.ForEach(setting => {
            GameObject wave = Instantiate(waveController, Vector3.zero, Quaternion.identity);
            wave.GetComponent<WaveController>().Setup(setting);
            this.waves.Add(wave);
        });

        this.NextWave();
    }

    public void NextWave() {
        if (waves.Count == 0) return;
        this.spawnPoints[0].GetComponentsInParent<Collider>(true)[0].enabled = false;
        this.DespawnUpgrades(this.spawnedObjs);
        this.waves[0].SetActive(true);  // Enable the next wave.

        // Restore shield charges on wave change.
        GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<LifeUpgrade>().RestoreActiveCharges();
    }

    private void SpawnUpgrades(List<Transform> spawnPoints) {
        for (int i = 0; i < this.spawnPoints.Count; i++)
            spawnedObjs.Add(Instantiate(this.upgradeObjects[i], this.spawnPoints[i].position, Quaternion.identity));
    }

    private void DespawnUpgrades(List<GameObject> upgradeObjs) {
        upgradeObjs.ForEach(up => Destroy(up));
        upgradeObjs.Clear();
    }

    private IEnumerator PlayWaveEndCutscene() {
        GameObject cameraPlayer = GameObject.Find("Main Camera");
        GameObject cameraLevelEnd = GameObject.Find("LevelEndCamera");
        GameObject cameraWaveEnd = GameObject.Find("WaveEndCamera");

        ParticleSystem particles = GameObject.FindGameObjectWithTag("Mountain").GetComponentInChildren<ParticleSystem>();

        cameraPlayer.GetComponent<Camera>().enabled = false;
        cameraLevelEnd.GetComponent<Camera>().enabled = false;
        cameraWaveEnd.GetComponent<Camera>().enabled = true;

        particles.Play();
        cameraWaveEnd.GetComponent<Animator>().SetBool("PlayCinematic", true);

        yield return new WaitForSeconds(5.0f);
        cameraWaveEnd.GetComponent<Animator>().SetBool("PlayCinematic", false);
        
        cameraWaveEnd.GetComponent<Camera>().enabled = false;
        cameraLevelEnd.GetComponent<Camera>().enabled = true;
        cameraPlayer.GetComponent<Camera>().enabled = true;
    }

    public void EndWave() {
        Destroy(this.waves[0]);
        this.waves.RemoveAt(0); // Remove the cleared wave.
        ObjectPooler.SharedInstance.ClearPool();  // Clear pooled game objects on the previous wave.
        
        if (this.waves.Count != 0) {
            StartCoroutine(PlayWaveEndCutscene());
            this.SpawnUpgrades(spawnPoints);
        }
        else {
            DissolveController controller = GameObject.FindGameObjectWithTag("Mountain").GetComponent<DissolveController>();
            controller.StartDissolving();
            controller.PlaySoundEffect();
        }

        if (waves.Count != 0) {
            this.spawnPoints[0].GetComponentsInParent<BoxCollider>(true)[0].enabled = true;
        }
    }

    public bool IsShoppingPhase() =>
        !this.waves.Exists(wave => wave.activeSelf);
}
