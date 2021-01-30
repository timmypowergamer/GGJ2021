using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
	public string newGameScene;
	public GameObject TitleCanvasGameObject;
	public GameObject SecondCanvasGameObject;	
	public GameObject LobbyCanvasGameObject;
	public GameObject Player1Model;
	public float xAngle, yAngle, zAngle;

	// Start is called before the first frame update
    void Start()
    {
        TitleCanvasGameObject.SetActive(true);
		SecondCanvasGameObject.SetActive(false);
		LobbyCanvasGameObject.SetActive(false);		
    }

    // Update is called once per frame
    void Update()
    {
        Player1Model.transform.Rotate(xAngle,yAngle,zAngle, Space.Self);
    }
	    
	public void NewGame()
    {
        TitleCanvasGameObject.SetActive(false);
		SecondCanvasGameObject.SetActive(true);
		LobbyCanvasGameObject.SetActive(false);	
    }

	public void HostGame()
    {
        TitleCanvasGameObject.SetActive(false);
		SecondCanvasGameObject.SetActive(false);
		LobbyCanvasGameObject.SetActive(true);	
    }	
	
    public void GoBack()
    {
        TitleCanvasGameObject.SetActive(true);
		SecondCanvasGameObject.SetActive(false);
		LobbyCanvasGameObject.SetActive(false);		
    }
	
    public void QuitGame()
    {
		UnityEditor.EditorApplication.isPlaying = false;
		Application.Quit();
    }
}
