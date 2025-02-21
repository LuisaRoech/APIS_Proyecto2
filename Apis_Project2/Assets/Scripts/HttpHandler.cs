using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using UnityEditor.PackageManager.Requests;
using TMPro;
using System;

public class HttpHandler :  MonoBehaviour
{
    [SerializeField]
    private RawImage[] images;

    [SerializeField]
    private string url = "https://rickandmortyapi.com/api/character";
    public void SendRequest()
    {
        StartCoroutine("GetUser", 1);
    }

    IEnumerator GetCharacters()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.ConnectionError)
        {
          Debug.Log(www.error);
        }
        else
        {
            if(www.responseCode == 200)
            {
                FakeUser user = JsonUtility.FromJson<FakeUser>(www.downloadHandler.text);

                GameObject.Find("username").GetComponent<TMP_Text>().text = user.username;
                Console.WriteLine(user.username);
                for(int i = 0; i < user.deck.Length; i++)
                {
                    var cardId = user.deck[i];
                    StartCoroutine(GetCharacter(cardId,i));

                }
                
            }
            else{
                string mensaje = "status:" + www.responseCode;
                mensaje += "\nError:" + www.error;
                Debug.Log(mensaje);
            }
        }
    }

     IEnumerator GetCharacter(int id, int index)
    {
        UnityWebRequest www = UnityWebRequest.Get(url+ "/"+ id);
        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.ConnectionError)
        {
          Debug.Log(www.error);
        }
        else
        {
            if(www.responseCode == 200)
            {
                Character character = JsonUtility.FromJson<Character>(www.downloadHandler.text);
                Debug.Log($"{character.id}:{character.name} is a {character.species}");
                StartCoroutine(GetImage(character.image,index));

            }
            else if (www.responseCode == 404)
            {
                string mensaje = "status:" + www.responseCode;
                mensaje += "\nError:" + www.error;
                Debug.Log(mensaje);
            }
        }
    }

    IEnumerator GetImage(string imageUrl, int index)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            var texture = ((DownloadHandlerTexture) request.downloadHandler).texture;

            if (index >= 0 && index < images.Length)
            {
                images[index].texture = texture;
            } 
            else 
            {
                Debug.LogError($"Index {index} is out of range for images array (size {images.Length})");
            }

        }

    }
}

[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string species;
    public string image;
}

[System.Serializable]
public class FakeUser
{
   public int id;
   public string username;
   public bool state;
   public int[] deck;
}
