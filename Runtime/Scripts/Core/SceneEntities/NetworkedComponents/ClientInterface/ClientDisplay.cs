using System;
using System.Collections.Generic;
using Core.Networking;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.SceneEntities
{
    public abstract class ClientDisplay : NetworkBehaviour
    {
        private NetworkVariable<ParticipantOrder> _participantOrder = new NetworkVariable<ParticipantOrder>();

        public InteractableObject MyInteractableObject;

        private static List<ClientDisplay> instances = new List<ClientDisplay>();
        public static IReadOnlyList<ClientDisplay> Instances => instances.AsReadOnly();

        private Action<bool> serverCalibrationCallback;

        private void Awake()
        {
            instances.Add(this);
        }

        public override void OnDestroy()
        {
            instances.Remove(this);
        }

        public override void OnNetworkSpawn()
        {
            if (!IsLocalPlayer)
            {
                
                foreach (var c in GetComponentsInChildren<Camera>())
                {
                    c.enabled = false;
                }

                foreach (var a in GetComponentsInChildren<AudioListener>())
                {
                    Destroy(a);
                }

                foreach (var e in GetComponentsInChildren<EventSystem>())
                {
                    Destroy(e);
                }
            }
            else
            {
                Debug.Log("ClientInterface OnNetworkSpawn");
            }
        }

        public void SetParticipantOrder(ParticipantOrder _ParticipantOrder)
        {
            _participantOrder.Value = _ParticipantOrder;
        }

        public ParticipantOrder GetParticipantOrder()
        {
            return _participantOrder.Value;
        }

        public void RequestCalibration(Action<bool> callback)
        {
            if (!IsServer)
            {
                Debug.LogError("RequestCalibration can only be called on the server");
                callback?.Invoke(false);
                return;
            }

            var clientOption = ClientOptions.Instance.GetOption(GetParticipantOrder());
            if (clientOption.ClientDisplay < 0 || clientOption.ClientDisplay >= ClientDisplaysSO.Instance.ClientDisplays.Count)
            {
                Debug.LogError($"Invalid ClientDisplay index for PO: {GetParticipantOrder()}");
                callback?.Invoke(false);
                return;
            }

            var displaySO = ClientDisplaysSO.Instance.ClientDisplays[clientOption.ClientDisplay];
            if (displaySO == null)
            {
                Debug.LogError($"ClientDisplaySO is null for PO: {GetParticipantOrder()}");
                callback?.Invoke(false);
                return;
            }

            if (displaySO.AuthoritativeMode == EAuthoritativeMode.Owner)
            {
                serverCalibrationCallback = callback;
                RequestCalibrationClientRpc();
            }
            else
            {
                CalibrateClient(callback);
            }
        }

        [ClientRpc]
        private void RequestCalibrationClientRpc()
        {
            if (!IsLocalPlayer) return;
            
            CalibrateClient((success) =>
            {
                RespondCalibrationServerRpc(success);
            });
        }

        [ServerRpc]
        private void RespondCalibrationServerRpc(bool success)
        {
            serverCalibrationCallback?.Invoke(success);
            serverCalibrationCallback = null;
        }

        public abstract bool AssignFollowTransform(InteractableObject _interactableObject, ulong targetClient);
        public abstract InteractableObject GetFollowTransform();

        public virtual void De_AssignFollowTransform(NetworkObject netobj)
        {
            if (IsServer)
            {
                NetworkObject.TryRemoveParent(true);
                MyInteractableObject = null;
                De_AssignFollowTransformClientRPC();
                Debug.Log("De_AssignFollowTransform");
            }
        }

        [ClientRpc]
        public virtual void De_AssignFollowTransformClientRPC()
        {
            MyInteractableObject = null;
        }

        public abstract Transform GetMainCamera();
        public abstract void CalibrateClient(Action<bool> calibrationFinishedCallback);
        public abstract void GoForPostQuestion();
    }
}