using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    [SerializeField] private RawImage characterImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text speciesText;
    [SerializeField] private TMP_Text typeText;
    [SerializeField] private TMP_Text originText;

    public void UpdateCard(API_Manager.Character character, Texture2D texture)
    {
        nameText.text = character.name;
        speciesText.text = character.species;
        typeText.text = character.type;
        originText.text = character.origin.name; 
        characterImage.texture = texture;
    }
}
