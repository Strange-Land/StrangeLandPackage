using System.Collections.Generic;
using Core.Networking;
using Core.SceneEntities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Core.Utilities;
using System.Linq;

namespace Core.UI
{
    public class ResearcherUI : MonoBehaviour
    {
        public GameObject SceneButtonPrefab;
        public Transform SceneButtonParent;
        public Transform CalibrationButtonParent;
        public TMP_Text ServerState;

        private Dictionary<ClientDisplay, GameObject> calibrationButtons = new Dictionary<ClientDisplay, GameObject>();

        private void Start()
        {
            List<SceneField> scenes = ConnectionAndSpawning.Instance.ScenarioScenes;
            foreach (var scene in scenes)
            {
                var sceneButton = Instantiate(SceneButtonPrefab, SceneButtonParent);
                sceneButton.GetComponent<Button>().onClick.AddListener(() => SwitchToScenario(scene.SceneName));
                sceneButton.GetComponentInChildren<TMP_Text>().text = scene.SceneName;
            }
        }

        private void Update()
        {
            string serverState = ConnectionAndSpawning.Instance.GetServerState();
            serverState = serverState.Substring(5);
            ServerState.text = $"Server State: {serverState}";

            UpdateCalibrationButtons();
        }

        private void UpdateCalibrationButtons()
        {
            foreach (var clientDisplay in ClientDisplay.Instances)
            {
                if (clientDisplay != null && !calibrationButtons.ContainsKey(clientDisplay))
                {
                    ParticipantOrder po = clientDisplay.GetParticipantOrder();
                    if (po != ParticipantOrder.Researcher)
                    {
                        CreateCalibrationButton(clientDisplay);
                    }
                }
            }

            var toRemove = new List<ClientDisplay>();
            foreach (var kvp in calibrationButtons)
            {
                if (kvp.Key == null || !ClientDisplay.Instances.Contains(kvp.Key))
                {
                    if (kvp.Value != null)
                    {
                        Destroy(kvp.Value);
                    }
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (var key in toRemove)
            {
                calibrationButtons.Remove(key);
            }
        }

        private void CreateCalibrationButton(ClientDisplay clientDisplay)
        {
            var buttonGO = Instantiate(SceneButtonPrefab, CalibrationButtonParent);
            var button = buttonGO.GetComponent<Button>();
            var text = buttonGO.GetComponentInChildren<TMP_Text>();
            
            ParticipantOrder po = clientDisplay.GetParticipantOrder();
            text.text = $"Calibrate {po}";
            
            button.onClick.AddListener(() => OnCalibrationButtonPressed(clientDisplay, button, text));
            
            calibrationButtons[clientDisplay] = buttonGO;
        }

        private void OnCalibrationButtonPressed(ClientDisplay clientDisplay, Button button, TMP_Text buttonText)
        {
            button.interactable = false;
            buttonText.text = $"Calibrating {clientDisplay.GetParticipantOrder()}...";
            
            clientDisplay.RequestCalibration((success) =>
            {
                if (success)
                {
                    buttonText.text = $"Calibrate {clientDisplay.GetParticipantOrder()} ✓";
                    buttonText.color = Color.green;
                }
                else
                {
                    buttonText.text = $"Calibrate {clientDisplay.GetParticipantOrder()} ✗";
                    buttonText.color = Color.red;
                }
                
                button.interactable = true;
                
                StartCoroutine(ResetButtonTextAfterDelay(buttonText, clientDisplay.GetParticipantOrder(), 3f));
            });
        }

        private System.Collections.IEnumerator ResetButtonTextAfterDelay(TMP_Text buttonText, ParticipantOrder po, float delay)
        {
            yield return new WaitForSeconds(delay);
            buttonText.text = $"Calibrate {po}";
            buttonText.color = Color.white;
        }

        private void SwitchToScenario(string scenarioName)
        {
            ConnectionAndSpawning.Instance.SwitchToLoading(scenarioName);
        }

        [ContextMenu("SwitchToInteract")]
        public void SwitchToInteract()
        {
            ConnectionAndSpawning.Instance.SwitchToInteract();
        }

        public void SwitchToWaiting()
        {
            ConnectionAndSpawning.Instance.BackToWaitingRoom();
        }

        private void OnDestroy()
        {
            foreach (var kvp in calibrationButtons)
            {
                if (kvp.Value != null)
                {
                    Destroy(kvp.Value);
                }
            }
            calibrationButtons.Clear();
        }
    }
}