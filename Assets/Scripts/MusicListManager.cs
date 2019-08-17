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

	private int select = 0;
    private int selectDft = 0;
    private bool slcMsc;
    private bool slcDft;
	private bool showStatement;
	private string[] difficultyName;

	void OnEnable()
    {
		if (FindObjectOfType<GameManager>() != null) gameManager = FindObjectOfType<GameManager>();
		difficultyName = new string[4] { "简单", "普通", "困难", "魔王" };
		fileList = GameManager.GetMusicList();
		GetAllMusicInfo();
		ShowMusicList();
    }

    void Update()
    {
		if (!GameManager.fading)
		{
			if (!slcDft) ShowMusicList();
			else ShowLevelList();
		}
	}

    void ShowMusicList() //显示歌曲列表
	{
		if (!slcMsc)
		{
			if (Input.GetButtonDown("Start") && showStatement) Application.Quit();
			else if (Input.GetButtonDown("DonL1") || Input.GetButtonDown("DonR1") || Input.GetButtonDown("Start"))
			{
				Selection[0].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1050F, 60F);
				int i = 1;
				while (i <= musicList.Count)
				{
					GameObject a = GameObject.Instantiate(SlcPre, parent.transform) as GameObject;
					Selection.Add(a);
					a.GetComponent<RectTransform>().localPosition = new Vector3(20, Selection[0].GetComponent<RectTransform>().localPosition.y - 20 - 50 * i, 0);
					a.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
					a.transform.GetChild(1).GetComponent<Text>().text = musicList[i - 1].title + " " + musicList[i - 1].subtitle;
					i++;
				}
				slcMsc = true;
			}
			else if (Input.GetButtonDown("Cancel"))
			{
				showStatement = statement.activeInHierarchy;
				statement.SetActive(!showStatement);
			}
		}
		else
		{
			if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical") || Input.GetButtonDown("KaL1") || Input.GetButtonDown("KaR1"))
			{
				//GetAxisRaw与GetKey(Down)对照：横坐标大于0为d，小于0为a；纵坐标大于0为w，小于0为s
				if (Input.GetButtonDown("KaR1") || Input.GetAxisRaw("Vertical") < 0 || Input.GetAxisRaw("Horizontal") > 0) select++;
				else if (Input.GetButtonDown("KaL1") || Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("Horizontal") < 0) select--;
				if (select > musicList.Count) select = 0;
				else if (select < 0) select = musicList.Count;
				ChangeListPosition();
			}
			else if (Input.GetButtonDown("Cancel"))
			{
				Selection[0].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1050F, 45F);
				Selection[0].GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
				for (int i = musicList.Count; i > 0; i--)
				{
					Destroy(Selection[i]);
					Selection.Remove(Selection[i]);
				}
				slcMsc = false;
			}
			else if ((Input.GetButtonDown("DonL1") || Input.GetButtonDown("DonR1")) && select != 0)
			{
				slcDft = true;
				p1.SetActive(true);
			}
		}
	}

	void ShowLevelList() //选定某歌曲时显示其难度列表
	{
		if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical") || Input.GetButtonDown("KaL1") || Input.GetButtonDown("KaR1"))
		{
			if (Input.GetButtonDown("KaR1") || Input.GetAxisRaw("Vertical") < 0 || Input.GetAxisRaw("Horizontal") > 0) selectDft++;
			else if (Input.GetButtonDown("KaL1") || Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("Horizontal") < 0) selectDft--;
			if (selectDft >= musicList[select - 1].courses.Count) selectDft = 0;
			else if (selectDft < 0) selectDft = musicList[select - 1].courses.Count - 1;
			p1.GetComponent<RectTransform>().localPosition = new Vector3(-470, 40 - 40 * selectDft, 0);
		}
		else if (Input.GetButtonDown("Cancel"))
		{
			slcDft = false;
			p1.GetComponent<RectTransform>().localPosition = new Vector3(-470, 40, 0);
			selectDft = 0;
			p1.SetActive(false);
		}
		else if (Input.GetButtonDown("DonL1") || Input.GetButtonDown("DonR1") || Input.GetButtonDown("Start"))
		{
			LoadMusic(musicList[select - 1], selectDft);
			StartCoroutine(gameManager.LoadScene("GamePlay"));
		}
	}
    void ChangeListPosition()
    {
        if (slcMsc&&select!=0)
        {
            int j = 0;
            while (j <= musicList.Count)
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
						Selection[j].transform.GetChild(1).GetComponent<Text>().text += "\n" + "   ◯" + difficultyName[k.difficulty];
                        for (int m = 1; m <= k.level; m++)
							Selection[j].transform.GetChild(1).GetComponent<Text>().text += "★";
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
            while (i <= musicList.Count)
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
				else if (i == "#END" && course.level > 0) 
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
		gameManager.songManager.Stop();
		gameManager.songManager.loop = false;
		string[] str = GameManager.ReadFile(score.filePath);
		GameManager.currentSong = new MusicScore();
		GameManager.currentcourse = new MusicScore.Course();
		GameManager.currentSong = score;
		GameManager.currentcourse = score.courses.Find(s => s.difficulty == difficulty);
		bool inCourse = false;
		foreach (string i in str)
		{
			if (i == ("COURSE:" + difficulty.ToString()))  inCourse = true;
			if (inCourse)
			{
				GameManager.currentcourse.course.Add(i);
				if (i == "#END") break;
			}
		}
		StartCoroutine(gameManager.ChangeSong(score.wave));
	}
}
