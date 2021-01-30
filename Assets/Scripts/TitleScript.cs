using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
	public string newGameScene;
	public Canvas mainCanvasObject;
	public Canvas secondCanvasObject;	
	public Canvas LobbyCanvasObject;		

	// Start is called before the first frame update
    void Start()
    {
        mainCanvasObject.enabled = true;
		secondCanvasObject.enabled = false;
		LobbyCanvasObject.enabled = false;		
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	    
	public void NewGame()
    {
        mainCanvasObject.enabled = false;
		secondCanvasObject.enabled = true;
		LobbyCanvasObject.enabled = false;		
    }

	public void HostGame()
    {
        mainCanvasObject.enabled = false;
		secondCanvasObject.enabled = false;
		LobbyCanvasObject.enabled = true;
    }	
	
    public void GoBack()
    {
        mainCanvasObject.enabled = true;
		secondCanvasObject.enabled = false;
		LobbyCanvasObject.enabled = false;		
    }
	
    public void QuitGame()
    {
		UnityEditor.EditorApplication.isPlaying = false;
		Application.Quit();
    }
}
