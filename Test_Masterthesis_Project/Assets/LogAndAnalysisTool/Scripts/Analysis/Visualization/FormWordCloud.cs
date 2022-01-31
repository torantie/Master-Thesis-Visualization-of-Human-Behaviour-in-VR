/**
 * https://medium.com/@SymboticaAndrew/a-vr-word-cloud-in-unity-f7cb8cf17b6b
 * https://gist.github.com/andrewsage/c8c3738a61a324a24bd5#file-formwordcloud-cs
 */

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class Phrase
{
    public string term;

    public float occurrences;
}

[Serializable]
public class PhrasesJSONRoot
{
    public Phrase[] phrases;
}

public class FormWordCloud : MonoBehaviour
{
    private List<Phrase> m_phrases = new List<Phrase>();

    private List<Phrase> m_randomisedPhrases = new List<Phrase>();

    private float m_totalOccurances = 0.0f;

    [SerializeField]
    private GameObject m_childObject;

    [SerializeField]
    private float m_size = 10.0f;

    private readonly string m_jsonString = "{\"phrases\":[{\"term\":\"the\", \"occurrences\":504},{\"term\":\"to\",\"occurrences\":447},{\"term\":\"rt\",\"occurrences\":433},{\"term\":\"a\",\"occurrences\":382},{\"term\":\"in\",\"occurrences\":299},{\"term\":\"of\",\"occurrences\":274},{\"term\":\"adventure\",\"occurrences\":236},{\"term\":\"and\",\"occurrences\":216},{\"term\":\"for\",\"occurrences\":166},{\"term\":\"is\",\"occurrences\":157},{\"term\":\"on\",\"occurrences\":154},{\"term\":\"cars\",\"occurrences\":136},{\"term\":\"it\",\"occurrences\":122},{\"term\":\"you\",\"occurrences\":116},{\"term\":\"with\",\"occurrences\":100},{\"term\":\"from\",\"occurrences\":87},{\"term\":\"at\",\"occurrences\":85},{\"term\":\"i\",\"occurrences\":85},{\"term\":\"this\",\"occurrences\":85},{\"term\":\"that\",\"occurrences\":83}]}";

    public GameObject ChildObject { get => m_childObject; set => m_childObject = value; }

    public float Size { get => m_size; set => m_size = value; }

    void Start()
    {
        ProcessWords(m_jsonString);
        Sphere();
    }

    // Update is called once per frame
    void Update()
    {
        Transform camera = Camera.main.transform;

        // Tell each of the objects to look at the camera
        foreach (Transform child in transform)
        {
            child.LookAt(camera.position);
            child.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void Sphere()
    {
        float points = m_phrases.Count;
        float increment = Mathf.PI * (3 - Mathf.Sqrt(5));
        float offset = 2 / points;
        for (float i = 0; i < points; i++)
        {
            float y = i * offset - 1 + (offset / 2);
            float radius = Mathf.Sqrt(1 - y * y);
            float angle = i * increment;
            Vector3 pos = new Vector3((Mathf.Cos(angle) * radius * Size), y * Size, Mathf.Sin(angle) * radius * Size);

            // Create the object as a child of the sphere
            GameObject child = Instantiate(ChildObject, pos, Quaternion.identity) as GameObject;
            child.transform.parent = transform;
            TextMesh phraseText = child.transform.GetComponent<TextMesh>();

            Phrase phrase = m_randomisedPhrases[(int)i];
            phraseText.text = phrase.term;

            float scale = (phrase.occurrences / m_totalOccurances) * 100.0f;
            child.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    private void ProcessWords(string jsonString)
    {
        m_totalOccurances = 0.0f;

        /*var jsonvale = JsonUtility.FromJson<Phrases>(jsonString);
        for (int i = 0; i < jsonvale.Count; i++)
        {
            Phrase phrase = new Phrase();
            phrase.term = jsonvale[i].term;
            phrase.occurrences = jsonvale[i].occurrences;
            phrases.Add(phrase);
            totalOccurances += phrase.occurrences;
        }*/
        var phrasesJSONRoot = JsonUtility.FromJson<PhrasesJSONRoot>(jsonString);
        foreach (var phrase in phrasesJSONRoot.phrases)
        {
            m_totalOccurances += phrase.occurrences;
        }
        m_phrases = phrasesJSONRoot.phrases.ToList();


        System.Random random = new System.Random();
        m_randomisedPhrases.Clear();
        for (int i = 0; i < m_phrases.Count; i++)
        {
            m_randomisedPhrases.Add(m_phrases[i]);
        }

        for (int i = 0; i < m_randomisedPhrases.Count; i++)
        {
            int first = i;
            int second = random.Next(0, m_randomisedPhrases.Count);
            Phrase temp = m_randomisedPhrases[second];
            m_randomisedPhrases[second] = m_randomisedPhrases[first];
            m_randomisedPhrases[first] = temp;
        }
    }
}
