using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;
	public enum GameState
	{
		Title,Selection,Gameplay,Result
	}
	public static GameState state = GameState.Title;
	public static bool fading = false;
	public static MusicScore currentSong = null;

	public AudioClip[] taikoSound;
	public AudioClip selectionMusic;

	AudioSource au;

	void Awake()
    {
		DontDestroyOnLoad(gameObject.transform.parent.gameObject);
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject.transform.parent.gameObject);
	}

	void OnEnable()
	{
		au = GetComponent<AudioSource>();
		GetMusicList();
	}

	void Update()
    {
		Control();
    }

	void Control()
	{
		if (!fading)
		{
			if (state == GameState.Selection || state == GameState.Gameplay)
			{
				if (Input.GetButtonDown("DonL1") || Input.GetButtonDown("DonR1")) au.PlayOneShot(taikoSound[0]);
				if (Input.GetButtonDown("KaL1") || Input.GetButtonDown("KaR1")) au.PlayOneShot(taikoSound[1]);
			}
			if (Input.GetButtonDown("Start") && state != GameState.Gameplay)
				au.PlayOneShot(taikoSound[0]);
		}
	}

	public static List<string> GetMusicList()
	{
		string fullPath = "Music/";
		List<string> filePaths = new List<string>();
		string filetype = "*.txt|*.tja";
		string[] FileType = filetype.Split('|');
		for (int i = 0; i < FileType.Length; i++)
		{
			string[] dirs = Directory.GetFiles(fullPath, FileType[i]);
			for (int j = 0; j < dirs.Length; j++) filePaths.Add(dirs[j]);
		}
		return filePaths;
	}

	public static string[] ReadFile(string filePath)
	{
		return File.ReadAllLines(filePath);
	}

	//public IEnumerator ChangeSong(string name)
	//{
	//	//WWW w = new WWW(url_voice);
	//	//yield return w;
	//	//myclip = w.audioClip;
	//	yield return null;
	//}

}
public class MusicScore
{
	public string title;//标题
	public string subtitle;//副标题
	public float bpm;
	public string wave;//音乐文件名
	public float offset;
	public class Course
	{
		public int difficulty;//0简单，1普通，2困难，3魔王
		public int level;//星数
		public List<string> course = new List<string>();//曲谱
	}
	public List<Course> courses = new List<Course>();
}
