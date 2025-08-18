using System.Collections;
using System.Linq;
using System.Text;
using Core.Networking;
using Core.Utilities;
using Newtonsoft.Json;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MetaStartupUI : MonoBehaviour
{
    public TMP_Dropdown PODropdown;
    public TMP_Dropdown LanguageDropdown;
    public InputField IPInputField;
    
    private IEnumerator Start()
    {
        LoadSavedConfig();
        PopulateLanguageDropdown();
        SetupUIListeners();

        yield return new WaitForSeconds(1f);
        StartClient();
    }
    
    private void LoadSavedConfig()
    {
        ClientConfig.LoadConfig();
        
        IPInputField.text = ClientConfig.GetIPAddress();
        
        PODropdown.value = (int)ClientConfig.GetParticipantOrder();
        PODropdown.RefreshShownValue();
        
        int languageIndex = (int)ClientConfig.GetLanguage();
        if (LanguageDropdown.options.Count > languageIndex)
        {
            LanguageDropdown.value = languageIndex;
            LanguageDropdown.RefreshShownValue();
        }
        
        Debug.Log($"Loaded config - IP: {ClientConfig.GetIPAddress()}, " +
                  $"PO: {ClientConfig.GetParticipantOrder()}, " +
                  $"Language: {ClientConfig.GetLanguage()}");
    }
    
    private void SetupUIListeners()
    {
        IPInputField.onEndEdit.AddListener(OnIPAddressChanged);
        
        PODropdown.onValueChanged.AddListener(OnPOChanged);
        
        LanguageDropdown.onValueChanged.AddListener(OnLanguageChanged);
    }
    
    private void OnIPAddressChanged(string newIP)
    {
        ClientConfig.SetIPAddress(newIP);
    }
    
    private void OnPOChanged(int index)
    {
        ClientConfig.SetParticipantOrder((ParticipantOrder)index);
    }
    
    private void OnLanguageChanged(int index)
    {
        ClientConfig.SetLanguage((Language)index);
    }
    
    private void PopulateLanguageDropdown()
    {
        LanguageDropdown.ClearOptions();
    
        var languages = System.Enum.GetNames(typeof(Language));
        LanguageDropdown.AddOptions(languages.ToList());
        
        LanguageDropdown.value = (int)ClientConfig.GetLanguage();
        LanguageDropdown.RefreshShownValue();
    }
    
    public void StartClient()
    {
        ClientConfig.SetAllConfig(
            IPInputField.text,
            (Language)LanguageDropdown.value,
            (ParticipantOrder)PODropdown.value
        );
        
        ConnectionAndSpawning.Instance.StartAsClient(
            ipAddress: IPInputField.text,
            po: (ParticipantOrder)PODropdown.value
        );
    }
    
    public void ResetConfigToDefaults()
    {
        ClientConfig.ResetToDefault();
        LoadSavedConfig();
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}