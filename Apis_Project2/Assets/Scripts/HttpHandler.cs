using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;  // <-- Importar TextMeshPro

public class API_Manager : MonoBehaviour
{
    [SerializeField]
    private string rickMortyUrl = "https://rickandmortyapi.com/api/character"; 

    [SerializeField]
    private string rainWorldUrl = "https://raw.githubusercontent.com/LuisaRoech/APIS_Proyecto2/refs/heads/main/db.json";

    [SerializeField] private TMP_Dropdown apiDropdown;
    [SerializeField] private TMP_Dropdown idDropdown;

    [SerializeField] private Card[] cards; 
    private string selectedUrl; 

    void Start()
    {
        HandleAPIDropdownValueChanged(apiDropdown.value);
        apiDropdown.onValueChanged.AddListener(HandleAPIDropdownValueChanged);
        idDropdown.onValueChanged.AddListener(HandleIDDropdownValueChanged);
    }

    private void HandleAPIDropdownValueChanged(int value)
    {
        switch (value)
        {
            case 0:
                selectedUrl = rickMortyUrl;
                break;
            case 1:
                selectedUrl = rainWorldUrl;
                break;
            default:
                selectedUrl = rickMortyUrl;
                break;
        }
        Debug.Log("API seleccionada: " + selectedUrl);

        // Recargar opciones del Dropdown de IDs
        StartCoroutine(LoadIDs());
    }

    private void HandleIDDropdownValueChanged(int value)
    {
        Debug.Log("ID seleccionado: " + idDropdown.options[value].text);
    }

    public void SendRequest(int cardIndex)
    {
        if (idDropdown.options.Count == 0)
        {
            Debug.Log("No hay IDs disponibles");
            return;
        }

        int id = int.Parse(idDropdown.options[idDropdown.value].text);
        StartCoroutine(GetCharacter(id, cardIndex));
    }

    IEnumerator LoadIDs()
    {
        UnityWebRequest www = UnityWebRequest.Get(selectedUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            idDropdown.ClearOptions();
            if (selectedUrl == rickMortyUrl)
            {
                RickAndMortyResponse response = JsonUtility.FromJson<RickAndMortyResponse>(www.downloadHandler.text);
                foreach (var character in response.results)
                {
                    idDropdown.options.Add(new TMP_Dropdown.OptionData(character.id.ToString()));
                }
            }
            else if (selectedUrl == rainWorldUrl)
            {
                CharactersList characterList = JsonUtility.FromJson<CharactersList>(www.downloadHandler.text);
                foreach (var character in characterList.characters)
                {
                    idDropdown.options.Add(new TMP_Dropdown.OptionData(character.id.ToString()));
                }
            }
            idDropdown.RefreshShownValue();
        }
    }

    IEnumerator GetCharacter(int id, int cardIndex)
    {
        string requestUrl = selectedUrl + (selectedUrl == rickMortyUrl ? "/" + id : "");

        UnityWebRequest www = UnityWebRequest.Get(requestUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {
                if (selectedUrl == rickMortyUrl)
                {
                    Character character = JsonUtility.FromJson<Character>(www.downloadHandler.text);
                    StartCoroutine(GetImage(character.image, character, cardIndex));
                }
                else if (selectedUrl == rainWorldUrl)
                {
                    CharactersList characterList = JsonUtility.FromJson<CharactersList>(www.downloadHandler.text);
                    Character foundCharacter = System.Array.Find(characterList.characters, c => c.id == id);

                    if (foundCharacter != null)
                    {
                        StartCoroutine(GetImage(foundCharacter.image, foundCharacter, cardIndex));
                    }
                    else
                    {
                        Debug.Log("Personaje no encontrado en Rain World con ID: " + id);
                    }
                }
            }
            else
            {
                Debug.Log("Error: " + www.responseCode + " - " + www.error);
            }
        }
    }

    IEnumerator GetImage(string imageUrl, Character character, int cardIndex)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            UpdateCard(character, texture, cardIndex);
        }
    }

    private void UpdateCard(Character character, Texture2D texture, int cardIndex)
    {
        if (cardIndex >= 0 && cardIndex < cards.Length)
        {
            cards[cardIndex].UpdateCard(character, texture);
        }
    }

    [System.Serializable]
    public class Character
    {
        public int id;
        public string name;
         public string status;
        public string type;
        public string species;
        public string gender;
        public string image;
        public Origin origin;
    }

    [System.Serializable]
    public class Origin
    {
        public string name;
        public string url;
    }

    [System.Serializable]
    public class CharactersList
    {
        public Character[] characters;
    }

    [System.Serializable]
    public class RickAndMortyResponse
    {
        public Character[] results;
    }
}
