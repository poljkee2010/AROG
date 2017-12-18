using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//namespace test { 
public class MenuScript : MonoBehaviour {

    public GameObject levelChanger; // Смена уровня
    public GameObject exitPanel;
	private bool isPaused = false;
	public GameObject pp; //pause

	void Update()
	{
        if (levelChanger.activeSelf == true && Input.GetKeyDown(KeyCode.Escape)) //если нажата кнопка escape и включена панель
        {
            levelChanger.SetActive (false);
		}
		else if (exitPanel.activeSelf == false && Input.GetKeyDown(KeyCode.Escape))
			{
			exitPanel.SetActive (true);
			}
		else if (Input.GetKeyDown(KeyCode.Escape))
		{
			exitPanel.SetActive(false);
		}
		//if (Input.GetKeyDown (KeyCode.Escape) && !isPaused && exitPanel.activeSelf == false) {
		//	pp.SetActive (true);
		//	Time.timeScale = 0;
		//	isPaused = true;
		//} else if (Input.GetKeyDown (KeyCode.Escape) && isPaused && exitPanel.activeSelf == false) {
		//	pp.SetActive (false);
		//	Time.timeScale = 1;
		//	isPaused = false;
		//}
	}

	public void OnClickStart()
	{
	//    Debug.Assert(levelChanger != null, "levelChanger != null");
	    levelChanger.SetActive(true); //если нажатии на кнопку "НАЧАТЬ ИГРУ" сможем выбирать уровни 
	}
    public void OnClickExit()
	{
		Application.Quit (); //Выход из игры
	}
    public void levelBttons(int level) //функция загрузки уровня по переданному паметру
    {
        SceneManager.LoadScene(level);
    }
	public void pauseOn() //пауза
	{
		pp.SetActive (true);
		Time.timeScale = 0;
		isPaused = true;
	}
	public void _continue() //продолжить
	{
		pp.SetActive (false);
		Time.timeScale = 1;
		isPaused = false;
	}
	public void gotomenu()
	{
		Time.timeScale = 1;
		SceneManager.LoadScene("Menu");
	}
}
//}
