using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickStartDown()
    {
        Debug.Log("Press Start Button");

        SceneManager.LoadScene("scLevel01");
        SceneManager.LoadScene("scPlay", LoadSceneMode.Additive);
    }

    public void OnClickOptionButton()
    {
        Debug.Log("Press Option Button");
    }

    public void OnClickCreditButton()
    {
        Debug.Log("Press Credit Button");
    }
}
