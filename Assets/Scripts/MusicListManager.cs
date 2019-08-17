using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicListManager : MonoBehaviour
{
	GameManager gameManager;
	List<string> fileList;
	List<MusicScore> musicList;
    public List<GameObject> Selection;
    public GameObject SlcPre;
    public GameObject parent;
    public GameObject p1;
    public GameObject statement;
    private int select=0;
    private int selectDft = 0;
    private bool slcMsc;
    private bool slcDft;
    private bool showStatement;
    void OnEnable()
    {
		if (FindObjectOfType<GameManager>() != null) gameManager = FindObjectOfType<GameManager>();
		fileList = GameManager.GetMusicList();
		GetAllMusicInfo();
		ShowMusicList();
    }

    void Update()
    {
		//if (!slcMsc && (Input.GetButtonDown("DonL1") || Input.GetButtonDown("DonR1"))) 
		if (!slcMsc && Input.GetButtonDown("DonL1")) 
        {
            Selection[0].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1050F, 60F);
            int i = 1;
            while (i <= musicList.ToArray().Length)
            {
                GameObject a = GameObject.Instantiate(SlcPre, parent.transform) as GameObject;
                Selection.Add(a);
                a.GetComponent<RectTransform>().localPosition = new Vector3(20, Selection[0].GetComponent<RectTransform>().localPosition.y - 20 - 50 * i, 0);
                a.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                a.transform.GetChild(1).GetComponent<Text>().text = musicList[i - 1].title+ " " + musicList[i - 1].subtitle;
                i++;
            }
            slcMsc = true;
        }
		if (!slcDft && slcMsc && (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))) 
        {
            //GetAxisRaw与GetKey(Down)对照：横坐标大于0为d，小于0为a；纵坐标大于0为w，小于0为s
            if ((Input.GetAxisRaw("Vertical") < 0 || Input.GetAxisRaw("Horizontal") > 0))
            {
                select += 1;
                if (select > musicList.ToArray().Length)
                {
                    select = 0;
                }
                ChangeListPosition();
            }
            else if ((Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("Horizontal") < 0))
            {
                select -= 1;
                if (select < 0)
                {
                    select = musicList.ToArray().Length;
                }
                ChangeListPosition();
            }
        }
        if(slcMsc && Input.GetButtonDown("KaR1"))
        {
            Selection[0].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1050F, 45F);
            Selection[0].GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            for(int i= musicList.ToArray().Length; i>0; i--)
            {
                Destroy(Selection[i]);
                Selection.Remove(Selection[i]);
            }
            slcMsc = false;
        }
		if (!slcDft && slcMsc&&select!=0 && Input.GetButtonDown("DonR1")) 
        {
            slcDft = true;
            p1.SetActive(true);  
        }
        if (slcDft&&( Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical")))
        {
            if ((Input.GetAxisRaw("Vertical") < 0 || Input.GetAxisRaw("Horizontal") > 0))
            {
                selectDft += 1;
                if (selectDft >= musicList[select - 1].courses.ToArray().Length)
                {
                    selectDft = 0;
                }
                p1.GetComponent<RectTransform>().localPosition = new Vector3(-470, 40 - 40 * selectDft, 0);
            }
            else if (slcDft && ((Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("Horizontal") < 0)))
            {
                selectDft -= 1;
                if (selectDft < 0)
                {
                    selectDft = musicList[select - 1].courses.ToArray().Length-1;
                }
                p1.GetComponent<RectTransform>().localPosition = new Vector3(-470, 40 - 40 * selectDft, 0);
            }
        }
        if (slcDft && slcMsc && Input.GetButtonDown("KaL1"))
        {
            slcDft = false;
            p1.GetComponent<RectTransform>().localPosition = new Vector3(-470, 40, 0);
            selectDft = 0;
            p1.SetActive(false);
        }
        if(slcDft && slcMsc && Input.GetButtonDown("Start"))
        {
            Debug.Log("filepath: "+musicList[select-1].filePath+ "  difficulty: "+selectDft);
            GameObject.Find("Fading_Screen").SetActive(true);
        }
		//以下为测试用脚本，测试完毕后删除
		if (Input.GetKeyDown(KeyCode.P))
		{
			Debug.Log("测试加载场景中。。。");
			LoadMusic(musicList[0], 0);
			StartCoroutine(GameManager.LoadScene("GamePlay"));
		}
        if (!slcMsc&& Input.GetButtonDown("Cancel"))
        {
            switch (showStatement)
            {
                case false: statement.SetActive(true);showStatement = true; break;
                case true: statement.SetActive(false); showStatement = false; break;
            }
        }
        if (showStatement)
        {
            if (Input.GetButtonDown("Start"))
            {
                Application.Quit();
            }
        }
    }
    void ShowMusicList()//显示歌曲列表
	{

	}

	void ShowLevelList()//选定某歌曲时显示其难度列表
	{

	}
    void LoadGame()
    {
        StartCoroutine(GameManager.LoadScene("GamePlay"));
    }
    void ChangeListPosition()
    {
        if (slcMsc&&select!=0)
        {
            int j = 0;
            while (j <= musicList.ToArray().Length)
            {
                if (j < select)
                {
                    Selection[j].GetComponent<RectTransform>().localPosition = new Vector3(Selection[j].GetComponent<RectTransform>().localPosition.x, 90 + 50 * (select - j), 0);
                    Selection[j].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1050F, 45F);
                    if (j == 0)
                    {
                        Selection[j].transform.GetChild(1).GetComponent<Text>().text = "Music";
                    }
                    else
                    {
                        Selection[j].transform.GetChild(1).GetComponent<Text>().text = musicList[j - 1].title + " " + musicList[j - 1].subtitle;
                    }
                }
                if (j == select)
                {
                    Selection[j].GetComponent<RectTransform>().localPosition = new Vector3(Selection[j].GetComponent<RectTransform>().localPosition.x, 0, 0);                   
                    Selection[j].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1050F, 220F);
                    Selection[j].transform.GetChild(1).GetComponent<Text>().text = musicList[j - 1].title + " " + musicList[j - 1].subtitle ;
                    foreach (MusicScore.Course k in musicList[j - 1].courses)
                    {
                        switch (k.difficulty)
                        {
                            case 0: Selection[j].transform.GetChild(1).GetComponent<Text>().text += "\n" + "   " + "◯" + "Easy"; break;
                            case 1: Selection[j].transform.GetChild(1).GetComponent<Text>().text += "\n" + "   " + "◯" + "Normal"; break;
                            case 2: Selection[j].transform.GetChild(1).GetComponent<Text>().text += "\n" + "   " + "◯" + "Hard"; break;
                            case 3: Selection[j].transform.GetChild(1).GetComponent<Text>().text += "\n" + "   " + "◯" + "Oni"; break;
                        }
                        for (int m = 1; m <= k.level; m++)
                        {
                            Selection[j].transform.GetChild(1).GetComponent<Text>().text += "★";
                        }
                    }

                }
                if (j > select)
                {
                    Selection[j].GetComponent<RectTransform>().localPosition = new Vector3(Selection[j].GetComponent<RectTransform>().localPosition.x, -90 + 50 * (select - j), 0);
                    Selection[j].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1050F, 45F);
                    Selection[j].transform.GetChild(1).GetComponent<Text>().text = musicList[j - 1].title + " " + musicList[j - 1].subtitle;
                }
                j++;
            }
        }
        else
        {
            Selection[0].GetComponent<RectTransform>().localPosition = new Vector3(Selection[0].GetComponent<RectTransform>().localPosition.x, 0, 0);
            Selection[0].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1050F, 60F);
            int i = 1;
            while (i <= musicList.ToArray().Length)
            {

                Selection[i].GetComponent<RectTransform>().localPosition = new Vector3(20, Selection[0].GetComponent<RectTransform>().localPosition.y - 20 - 50 * i, 0);
                Selection[i].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1050F, 45F);
                Selection[i].transform.GetChild(1).GetComponent<Text>().text = musicList[i - 1].title + " " + musicList[i - 1].subtitle;
                i++;
            }
        }
    }
	void GetAllMusicInfo()
	{
		musicList = new List<MusicScore>();
		foreach (string name in fileList)
		{
			MusicScore score = new MusicScore();
			MusicScore.Course course = new MusicScore.Course();
			string[] str = GameManager.ReadFile(name);
			score.filePath = name;
			foreach (string i in str)
			{
				if (i.StartsWith("TITLE:")) score.title = i.Substring(6);
				else if (i.StartsWith("SUBTITLE:")) score.subtitle = i.Substring(9);
				else if (i.StartsWith("BPM:")) score.bpm = float.Parse(i.Substring(4));
				else if (i.StartsWith("WAVE:")) score.wave = i.Substring(5);
				else if (i.StartsWith("OFFSET:")) score.offset = float.Parse(i.Substring(7));
				else if (i.StartsWith("COURSE:")) course.difficulty = int.Parse(i.Substring(7));
				else if (i.StartsWith("LEVEL:")) course.level = int.Parse(i.Substring(6));
				else if (i.StartsWith("#END") && course.level > 0)
				{
					score.courses.Add(course);
					course = new MusicScore.Course();
				}
			}
			if (score.title != "" && score.bpm != 0 && score.courses.Count > 0)
			{
				score.courses = score.courses.OrderBy(s => s.difficulty).ToList();
				musicList.Add(score);
			}
		}
	}

	void LoadMusic(MusicScore score, int difficulty)
	{
		string[] str = GameManager.ReadFile(score.filePath);
		GameManager.currentSong = new MusicScore();
		GameManager.currentcourse = new MusicScore.Course();
		GameManager.currentSong = score;
		GameManager.currentcourse = score.courses.Find(s => s.difficulty == difficulty);
		bool inCourse = false;
		foreach (string i in str)
		{
			if (i.Contains("COURSE:" + difficulty.ToString())) inCourse = true;
			if (inCourse)
			{
				GameManager.currentcourse.course.Add(i);
				if (i.Contains("#END")) break;
			}
		}
		StartCoroutine(gameManager.ChangeSong(score.wave));
	}
}
