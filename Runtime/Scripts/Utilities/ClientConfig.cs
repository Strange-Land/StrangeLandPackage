using System;
using System.IO;
using Core.Networking;
using UnityEngine;

namespace Core.Utilities
{
    [Serializable]
    public class ClientConfigData
    {
        public string ipAddress = "172.20.182.1";
        public Language language = Language.English;
        public ParticipantOrder po = ParticipantOrder.A;
    }

    public static class ClientConfig
    {
        private const string CONFIG_FILE_NAME = "ClientConfig.json";
        private static string ConfigFilePath => Path.Combine(Application.persistentDataPath, CONFIG_FILE_NAME);

        private static ClientConfigData _data;
        private static bool _isLoaded = false;

        public static ClientConfigData Data
        {
            get
            {
                if (!_isLoaded)
                {
                    LoadConfig();
                }
                return _data;
            }
        }

        public static string GetIPAddress()
        {
            var data = Data;
            if (string.IsNullOrEmpty(data.ipAddress))
            {
                data.ipAddress = "127.0.0.1";
                SaveConfig();
            }
            return data.ipAddress;
        }

        public static void SetIPAddress(string ipAddress)
        {
            if (!_isLoaded)
            {
                LoadConfig();
            }

            _data.ipAddress = string.IsNullOrEmpty(ipAddress) ? "127.0.0.1" : ipAddress;
            SaveConfig();
        }

        public static Language GetLanguage()
        {
            return Data.language;
        }

        public static void SetLanguage(Language language)
        {
            if (!_isLoaded)
            {
                LoadConfig();
            }

            _data.language = language;
            SaveConfig();
        }

        public static ParticipantOrder GetParticipantOrder()
        {
            return Data.po;
        }

        public static void SetParticipantOrder(ParticipantOrder po)
        {
            if (!_isLoaded)
            {
                LoadConfig();
            }

            _data.po = po;
            SaveConfig();
        }

        public static void SetAllConfig(string ipAddress, Language language, ParticipantOrder po)
        {
            if (!_isLoaded)
            {
                LoadConfig();
            }

            _data.ipAddress = string.IsNullOrEmpty(ipAddress) ? "127.0.0.1" : ipAddress;
            _data.language = language;
            _data.po = po;
            SaveConfig();
        }

        public static void LoadConfig()
        {
            if (_isLoaded)
                return;

            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    _data = JsonUtility.FromJson<ClientConfigData>(json);

                    if (_data == null)
                    {
                        _data = new ClientConfigData();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(_data.ipAddress))
                        {
                            _data.ipAddress = "127.0.0.1";
                        }
                    }
                    
                    Debug.Log($"Client config loaded from: {ConfigFilePath}");
                }
                else
                {
                    _data = new ClientConfigData();
                    Debug.Log("No existing client config found, using defaults");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load client config: {e.Message}");
                _data = new ClientConfigData();
            }

            _isLoaded = true;
        }

        public static void SaveConfig()
        {
            try
            {
                if (_data == null)
                {
                    _data = new ClientConfigData();
                }

                string json = JsonUtility.ToJson(_data, true);
                File.WriteAllText(ConfigFilePath, json);
                Debug.Log($"Client config saved to: {ConfigFilePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save client config: {e.Message}");
            }
        }

        public static void ResetToDefault()
        {
            _data = new ClientConfigData();
            SaveConfig();
            Debug.Log("Client config reset to defaults");
        }
    }
}