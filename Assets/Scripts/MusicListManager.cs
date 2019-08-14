using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicListManager : MonoBehaviour
{
	GameManager gameManager;
	List<string> fileList;
	List<MusicScore> musicList;

    void OnEnable()
    {
		if (FindObjectOfType<GameManager>() != null) gameManager = FindObjectOfType<GameManager>();
		fileList = GameManager.GetMusicList();
		GetAllMusicInfo();
		ShowMusicList();
	}

    void Update()
    {
        
    }

	void ShowMusicList()//显示歌曲列表
	{

	}

	void ShowLevelList()//选定某歌曲时显示其难度列表
	{

	}

	void GetAllMusicInfo()
	{
		musicList = new List<MusicScore>();
		foreach (string name in fileList)
		{
			MusicScore score = new MusicScore();
			MusicScore.Course course = new MusicScore.Course();
			string[] str = GameManager.ReadFile(name);
			foreach (string i in str)
			{
				if (i.StartsWith("TITLE:")) score.title = i.Substring(6);
				else if (i.StartsWith("SUBTITLE:")) score.subtitle = i.Substring(9);
				else if (i.StartsWith("BPM:")) score.bpm = float.Parse(i.Substring(4));
				else if (i.StartsWith("WAVE:")) score.subtitle = i.Substring(5);
				else if (i.StartsWith("COURSE:")) course.difficulty = int.Parse(i.Substring(7));
				else if (i.StartsWith("LEVEL:")) course.level = int.Parse(i.Substring(6));
				else if (i.StartsWith("#END") && course.level > 0)
				{
					score.courses.Add(course);
					course.level = 0;
				}
			}
			if (score.title != "" && score.bpm != 0 && score.courses.Count > 0)
			{
				score.courses = score.courses.OrderBy(s => s.difficulty).ToList();
				musicList.Add(score);
			}
		}
	}

}
