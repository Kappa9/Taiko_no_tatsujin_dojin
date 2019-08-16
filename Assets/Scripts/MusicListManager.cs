﻿using System.Linq;
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
    private int select=0;
    private int selectDft = 0;
    private bool slcMsc;
    private bool slcDft;
    void OnEnable()
    {
		if (FindObjectOfType<GameManager>() != null) gameManager = FindObjectOfType<GameManager>();
		fileList = GameManager.GetMusicList();
		GetAllMusicInfo();
		ShowMusicList();
    }

    void Update()
    {
        if (slcMsc == false && Input.GetButtonDown("DonL1"))
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
        if (slcDft==false&&slcMsc == true && (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical")))   //要用GetButtonDown而非GetButton，防止按键持续生效
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
        if(slcMsc == true && Input.GetButtonDown("Cancel"))
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
        if(slcDft==false&&slcMsc&& Input.GetButtonDown("DonR1"))
        {
            slcDft = true;
            GameObject a = Instantiate(p1, parent.transform);
            if ((Input.GetAxisRaw("Vertical") < 0 || Input.GetAxisRaw("Horizontal") > 0))
            {
                selectDft += 1;
                if (select > musicList[select - 1].courses.ToArray().Length)
                {
                    selectDft = 0;
                }               
            }
            else if ((Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("Horizontal") < 0))
            {
                selectDft -= 1;
                if (selectDft < 0)
                {
                    selectDft = musicList[select - 1].courses.ToArray().Length;
                }
            }
            a.GetComponent<RectTransform>().localPosition = new Vector3(-470, 40-15*selectDft, 0);
        }
        if (slcDft == true&& slcMsc && Input.GetButtonDown("KaL1"))
        {
            slcDft = false;
            Destroy (GameObject.Find("P1(Clone)")); 
        }
        //Debug.Log("");
    }
    void ShowMusicList()//显示歌曲列表
	{

	}

	void ShowLevelList()//选定某歌曲时显示其难度列表
	{

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
		
		bool inCourse = false;
		foreach (string i in str)
		{
			if (i.Contains("COURSE:" + difficulty.ToString())) inCourse = true;
			if (inCourse)
			{
				score.courses.Find(s => s.difficulty == difficulty).course.Add(i);
				if (i.Contains("#END")) break;
			}
		}
		GameManager.currentSong = score;
		gameManager.StartCoroutine("ChangeSound", score.wave);
	}
}
