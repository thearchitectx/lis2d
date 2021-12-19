using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TheArchitect.SceneObjects;

public class PanelTextInput : SceneObject
{
    
    [SerializeField] private InputField m_Input;
    [SerializeField] private Text m_TextPrompt;
    [SerializeField] private Button m_ButtonGenerate;
    [SerializeField] private Button m_ButtonConfirm;
    [SerializeField] private string[] m_GenerateList;
    [SerializeField] private string m_FirstGenerate;

    // Start is called before the first frame update
    void Start()
    {
        this.m_ButtonGenerate.onClick.AddListener( () => {
            if (!string.IsNullOrEmpty(this.m_FirstGenerate))
            {
                this.m_Input.text = this.m_FirstGenerate;
                this.m_FirstGenerate = null;
            }
            else
            {
                this.m_Input.text = this.m_GenerateList[Random.Range(0, this.m_GenerateList.Length)];
                this.m_ButtonConfirm.interactable = true;
            }
        });

        this.m_ButtonGenerate.gameObject.SetActive(this.m_GenerateList != null && this.m_GenerateList.Length > 0);

        this.m_ButtonConfirm.interactable = false;

        this.m_Input.onValueChanged.AddListener(
            evt => this.m_ButtonConfirm.interactable = !string.IsNullOrEmpty(this.m_Input.text)
        );

        this.m_ButtonConfirm.onClick.AddListener( () => this.Outcome = this.m_Input.text );
    }

    public void SetPrompt(string prompt)
    {
        this.m_TextPrompt.text = prompt;
    }

    // Update is called once per frame
    public void LoadGenerateList(string streamingAssetTextFile)
    {
        this.m_GenerateList = System.IO.File.ReadAllLines($"{Application.streamingAssetsPath}/{streamingAssetTextFile}");
        this.m_ButtonGenerate.gameObject.SetActive(this.m_GenerateList != null && this.m_GenerateList.Length > 0);
    }

    public void SetFirstGenerate(string firstGenerate)
    {
        this.m_FirstGenerate = firstGenerate;
    }
}
