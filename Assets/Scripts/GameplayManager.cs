using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
	public GameObject[] note;
	public Transform noteParent;
	public Transform hitPoint;
	public Animator[] interact_Control;
	public GameObject[] interact_Hit;
	public GameObject gogoTimebg;
	public Image soulGauge;//全长866，合格线683，剩余183
	public Text[] title;
	public Text difficultyText;
	public SpriteRenderer difficultySymbol;
	public Sprite[] difficultySprite;

	int score = 0;
	int soulPoint = 0;

	float[] hitTime;
	int[] basicpoint;
	bool gogoTime = false;
	bool loseFullCombo = false;
	float notePos = 0;

	void OnEnable()
    {
		hitTime = new float[4];
		basicpoint = new int[5];
		DisplayInformation();
		GenerateGame();
	}

    void Update()
    {
		OnControl();
		CheckGameState();
    }

	void LateUpdate()
	{
		DestroyNote();
	}

	void CheckGameState()
	{
		LayerMask mask = LayerMask.GetMask("Special");
		Collider2D[] hit = Physics2D.OverlapPointAll(hitPoint.position, mask);
		if (hit != null)
			foreach (Collider2D i in hit)
			{
				switch (i.name)
				{
					case "#START":
						GameManager manager = FindObjectOfType<GameManager>();
						manager.songManager.Play();
						break;
					case "#GOGOSTART": gogoTime = true; break;
					case "#GOGOEND": gogoTime = false; break;
					case "#END": break;
				}
			}
		gogoTimebg.SetActive(gogoTime);
	}

	void OnControl()
	{
		if (Input.GetButtonDown("DonL1"))
		{
			hitTime[0] = Time.time;
			if (hitTime[0] - hitTime[1] <= 0.03f) interact_Control[2].Play("Active");
			interact_Control[0].Play("Active");
			interact_Control[1].Play("Active");
		}
		if (Input.GetButtonDown("DonR1"))
		{
			hitTime[1] = Time.time;
			if (hitTime[1] - hitTime[0] <= 0.03f) interact_Control[1].Play("Active");
			interact_Control[0].Play("Active");
			interact_Control[2].Play("Active");
		}
		if (Input.GetButtonDown("KaL1"))
		{
			hitTime[2] = Time.time;
			if (hitTime[2] - hitTime[3] <= 0.03f) interact_Control[5].Play("Active");
			interact_Control[3].Play("Active");
			interact_Control[4].Play("Active");
		}
		if (Input.GetButtonDown("KaR1"))
		{
			hitTime[3] = Time.time;
			if (hitTime[3] - hitTime[2] <= 0.03f) interact_Control[4].Play("Active");
			interact_Control[3].Play("Active");
			interact_Control[5].Play("Active");
		}
	}

	void DisplayInformation()
	{
		title[0].text = GameManager.currentSong.title;
		title[1].text = GameManager.currentSong.subtitle;
		difficultySymbol.sprite = difficultySprite[GameManager.currentcourse.difficulty];
		string[] str = new string[4] { "简单", "普通", "困难", "魔王" };
		difficultyText.text = str[GameManager.currentcourse.difficulty];
	}

	void GenerateGame() //每拍(四分音符)长度3，每小节4拍总长12，每拍秒数=60/BPM，每小节秒数=240/BPM
	{
		int measure = 4;
		int maxCombo = 0;
		int rendaState = 0;
		foreach (string i in GameManager.currentcourse.course)
		{
			if (!i.StartsWith("#"))
			{
				foreach(char c in i.ToCharArray())
				{
					if (c != ',')
					{
						int num = int.Parse(c.ToString());
						if (rendaState == 0)
						{
							if (num >= 1 && num <= 4)
							{
								GenerateNote(num);
								maxCombo++;
							}
							else if(num == 5 || num == 7)
							{
								rendaState = 1;
								GameObject obj = GenerateNote(5);
								obj.transform.localScale = new Vector3(2 * measure / (i.Length - 1), 1, 1);
							}
							else if (num == 6 || num == 9)
							{
								rendaState = 2;
								GameObject obj = GenerateNote(6);
								obj.transform.localScale = new Vector3(2 * measure / (i.Length - 1), 1, 1);
							}
						}
						else
						{
							if (num == 8)
							{
								GenerateNote(rendaState + 8);
								rendaState = 0;
							}
							else
							{
								GameObject obj = GenerateNote(rendaState + 6);
								obj.transform.localScale = new Vector3(2 * measure / (i.Length - 1), 1, 1);
							}
						}
						notePos += 3 * measure / (i.Length - 1);
					}
				}
			}
			else
			{
				GameObject obj = GenerateNote(11);
				obj.name = i;
				switch (i)
				{
					case "#START":
						float offset = GameManager.currentSong.offset / 60 * GameManager.currentSong.bpm * 3;
						obj.transform.Translate(Vector3.right * offset); //Offset/60*BPM=偏移拍数p,p*3=偏移坐标
						break;
					case "#END": GenerateNote(0); break;
				}
			}
		}
	}

	GameObject GenerateNote(int index)
	{
		Vector2 pos = new Vector2(9.09f, 1.29f);
		return Instantiate(note[index], pos + Vector2.right * notePos, Quaternion.identity, noteParent);
	}

	void DestroyNote()
	{
		RaycastHit2D[] hit = Physics2D.LinecastAll(Vector2.left * 5, Vector2.left * 6);
		if (hit != null)
			foreach (RaycastHit2D i in hit)
				Destroy(i.collider.gameObject);
	}
}
