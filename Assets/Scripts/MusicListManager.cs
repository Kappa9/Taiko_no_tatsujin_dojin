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
    List<GameObject> Selection;
    public GameObject SlcPre;
    public GameObject parent;
    public GameObject p1;
    public GameObject statement;
    public Text stateText;

	private int select = 0;
    private int selectDft = 0;
	private bool slcMsc = false;
	private bool slcDft = false;
	private bool showStatement;
	private string[] difficultyName;

	void OnEnable()
    {
		if (FindObjectOfType<GameManager>() != null) gameManager = FindObjectOfType<GameManager>();
		difficultyName = new string[4] { "简单", "普通", "困难", "魔王" };
		fileList = GameManager.GetMusicList();
		Selection = new List<GameObject>();
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
			int j = 0;
			foreach (MusicScore i in musicList)
			{
				GameObject a = Instantiate(SlcPre, parent.transform);
				Selection.Add(a);
				a.GetComponent<RectTransform>().localPosition = new Vector3(0, Selection[0].GetComponent<RectTransform>().localPosition.y - 50 * j, 0);
				a.GetComponent<RectTransform>().localScale = Vector3.one;
				a.transform.GetChild(1).GetComponent<Text>().text = i.title + " " + i.subtitle;
				j++;
			}
			ChangeListPosition();
			slcMsc = true;
		}
		else if (!showStatement)
		{
			if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical") || Input.GetButtonDown("KaL1") || Input.GetButtonDown("KaR1"))
			{
				//GetAxisRaw与GetKey(Down)对照：横坐标大于0为d，小于0为a；纵坐标大于0为w，小于0为s
				if (Input.GetButtonDown("KaR1") || Input.GetAxisRaw("Vertical") < 0 || Input.GetAxisRaw("Horizontal") > 0) select++;
				else if (Input.GetButtonDown("KaL1") || Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("Horizontal") < 0) select--;
				if (select >= musicList.Count) select = 0;
				else if (select < 0) select = musicList.Count - 1;
				ChangeListPosition();
			}
			else if (Input.GetButtonDown("Cancel"))
			{
				stateText.gameObject.SetActive(false);
				showStatement = true;
				statement.SetActive(true);	
			}
			else if (Input.GetButtonDown("DonL1") || Input.GetButtonDown("DonR1") || Input.GetButtonDown("Start")) 
			{
				stateText.text = "请选择难度，按下 Esc 键取消";
				slcDft = true;
				p1.SetActive(true);
				p1.GetComponent<RectTransform>().localPosition = new Vector3(-490, -37 + 21.75f * musicList[select].courses.Count, 0);
			}
		}
		else
		{
			if (Input.GetButtonDown("Start")) Application.Quit();
			else if (Input.GetButtonDown("Cancel"))
			{
				showStatement = false;
				statement.SetActive(false);
				stateText.gameObject.SetActive(true);
			}
		}
	}

	void ShowLevelList() //选定某歌曲时显示其难度列表
	{
		if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical") || Input.GetButtonDown("KaL1") || Input.GetButtonDown("KaR1"))
		{
			if (Input.GetButtonDown("KaR1") || Input.GetAxisRaw("Vertical") < 0 || Input.GetAxisRaw("Horizontal") > 0) selectDft++;
			else if (Input.GetButtonDown("KaL1") || Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("Horizontal") < 0) selectDft--;
			if (selectDft >= musicList[select].courses.Count) selectDft = 0;
			else if (selectDft < 0) selectDft = musicList[select].courses.Count - 1;
			p1.GetComponent<RectTransform>().localPosition = new Vector3(-490, -37f + 21.75f * musicList[select].courses.Count - 43 * selectDft, 0);
		}
		else if (Input.GetButtonDown("Cancel"))
		{
			stateText.text = "按下 Esc 键打开游戏帮助与菜单";
			slcDft = false;
			selectDft = 0;
			p1.SetActive(false);
		}
		else if (Input.GetButtonDown("DonL1") || Input.GetButtonDown("DonR1") || Input.GetButtonDown("Start"))
		{
			LoadMusic(musicList[select], musicList[select].courses[selectDft].difficulty);
			StartCoroutine(gameManager.LoadScene("GamePlay"));
		}
	}
    void ChangeListPosition()
    {
		int j = 0;
		while (j < musicList.Count)
		{
			if (j < select)
			{
				Selection[j].GetComponent<RectTransform>().localPosition = new Vector3(Selection[j].GetComponent<RectTransform>().localPosition.x, 25 * musicList[select].courses.Count + 55 * (select - j), 0);
				Selection[j].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1050F, 50F);
				Selection[j].transform.GetChild(1).GetComponent<Text>().text = musicList[j].title + " " + musicList[j].subtitle;
			}
			if (j == select)
			{
				Selection[j].GetComponent<RectTransform>().localPosition = new Vector3(Selection[j].GetComponent<RectTransform>().localPosition.x, 0, 0);
				Selection[j].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1050F, 50 + 50 * musicList[select].courses.Count);
				Selection[j].transform.GetChild(1).GetComponent<Text>().text = musicList[j].title + " " + musicList[j].subtitle;
				foreach (MusicScore.Course k in musicList[j].courses)
				{
					Selection[j].transform.GetChild(1).GetComponent<Text>().text += "\n" + "   ◯" + difficultyName[k.difficulty];
					for (int m = 1; m <= k.level; m++)
						Selection[j].transform.GetChild(1).GetComponent<Text>().text += "★";
				}
			}
			if (j > select)
			{
				Selection[j].GetComponent<RectTransform>().localPosition = new Vector3(Selection[j].GetComponent<RectTransform>().localPosition.x, -25 * musicList[select].courses.Count + 55 * (select - j), 0);
				Selection[j].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1050F, 50F);
				Selection[j].transform.GetChild(1).GetComponent<Text>().text = musicList[j].title + " " + musicList[j].subtitle;
			}
			j++;
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
		GameManager.songManager.Stop();
		GameManager.songManager.loop = false;
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
