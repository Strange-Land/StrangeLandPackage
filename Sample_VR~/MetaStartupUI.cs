using System.Linq;
using System.Text;
using Core.Networking;
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

    private ClientConfig _clientConfig;
    
    private void Start()
    {
        PopulateLanguageDropdown();
    }
        
    private void PopulateLanguageDropdown()
    {
        LanguageDropdown.ClearOptions();
    
        var languages = System.Enum.GetNames(typeof(Language));
        LanguageDropdown.AddOptions(languages.ToList());
    }
    public void StartClient()
    {
        ConnectionAndSpawning.Instance.StartAsClient(ipAddress: IPInputField.text,
            po: (ParticipantOrder)PODropdown.value);
    }

}
