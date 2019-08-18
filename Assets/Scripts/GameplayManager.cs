using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
	public GameObject[] note;
	public GameObject noteParent;
	public Transform parent;
	public Transform hitPoint;
	public Animator donchan;
	public GameObject[] dancer;
	public Animator[] soulAnimation;
	public Animator[] interact_Control;
	public GameObject gogoTimebg;
	public RectTransform soulGauge;//全长866，合格线683，剩余183
	public Text[] title;
	public Text scoreText;
	public Text comboText;
	public Text difficultyText;
	public SpriteRenderer difficultySymbol;
	public Sprite[] difficultySprite;
	public GameObject[] backGrounds;
	public GameObject[] hitExcellent;
	public GameObject[] hitGood;
	public GameObject[] hitBad;
	public GameObject[] comboObjects;
	public GameObject[] rendaBalloon;
	public GameObject resultScreen;
	public Sprite[] resultScreenSprite;
	public RectTransform resultSoulGauge;
	public Image[] resultImage;
	public Text[] resultText;

	int score = 0;
	int soulPoint = 0;
	int[] count;
	int currentCombo = 0;
	int currentRenda = 0;
	int hitScore;

	GameManager gameManager;
	LayerMask mask;
	float[] hitTime;
	int[] basicpoint;
	int[] requireSoulPoint;
	bool gogoTime = false;
	bool miss = false;
	bool loseFullCombo = false;
	bool soulFull = false;
	float notePos = 0;

	void OnEnable()
    {
		if (FindObjectOfType<GameManager>() != null) gameManager = FindObjectOfType<GameManager>();
		donchan.speed = 0.5f;
		mask = LayerMask.GetMask("Note");
		hitTime = new float[4];
		basicpoint = new int[5];
		requireSoulPoint = new int[4] { 6000, 7000, 7000, 8000 };
		count = new int[5] { 0, 0, 0, 0, 0 };
		DisplayInformation();
		StartCoroutine(GenerateGame());
	}

    void Update()
    {
		if (!GameManager.fading && GameManager.state == GameManager.GameState.Gameplay) 
		{
			OnControl();
			CheckGameState();
		}
    }

	void LateUpdate()
	{
		if (GameManager.state == GameManager.GameState.Gameplay) DestroyNote();
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
					case "#START": GameManager.songManager.Play(); break;
					case "#GOGOSTART": gogoTime = true; break; 
					case "#GOGOEND": gogoTime = false; break;
					case "#RENDAEND": StartCoroutine(DisableRendaBalloon()); break;
					case "#END": StartCoroutine(ShowResult()); break;
				}
				Destroy(i.gameObject);
			}
		gogoTimebg.SetActive(gogoTime);
		donchan.SetBool("GogoTime", gogoTime);
		backGrounds[2].SetActive(miss);
		scoreText.text = score.ToString();
		if (currentCombo >= 10)
		{
			comboText.text = currentCombo.ToString();
			if (currentCombo < 30) hitScore = basicpoint[0] + basicpoint[1];
			else if (currentCombo < 50) hitScore = basicpoint[0] + basicpoint[1] * 2;
			else if (currentCombo < 100) hitScore = basicpoint[0] + basicpoint[1] * 4;
			else { comboText.fontSize = 72; hitScore = basicpoint[0] + basicpoint[1] * 8; }
		}
		else hitScore = basicpoint[0];
		if (soulPoint >= requireSoulPoint[GameManager.currentcourse.difficulty])
		{
			float width = 683 + 183 * (soulPoint - requireSoulPoint[GameManager.currentcourse.difficulty]) / (10000 - requireSoulPoint[GameManager.currentcourse.difficulty]);
			soulGauge.sizeDelta = new Vector2(width, soulGauge.sizeDelta.y);
			backGrounds[3].SetActive(true);
			backGrounds[4].SetActive(true);
			if (soulPoint >= 10000) soulFull = true;
			else soulFull = false;
			donchan.SetBool("Gold", soulFull);
			soulAnimation[0].SetBool("Gold", soulFull);
			soulAnimation[1].SetBool("Gold", soulFull);
			backGrounds[5].SetActive(soulFull);
		}
		else
		{
			float width = 683 * soulPoint / requireSoulPoint[GameManager.currentcourse.difficulty];
			soulGauge.sizeDelta = new Vector2(width, soulGauge.sizeDelta.y);
			backGrounds[3].SetActive(false);
			backGrounds[4].SetActive(false);
		}
	}

	void OnControl()
	{
		if (Input.GetButtonDown("DonL1"))
		{
			hitTime[0] = Time.time;
			if (hitTime[0] - hitTime[1] <= 0.03f) { HitNote(true, true); interact_Control[2].Play("Active"); }
			else HitNote(true, false);
			interact_Control[0].Play("Active");
			interact_Control[1].Play("Active");
		}
		if (Input.GetButtonDown("DonR1"))
		{
			hitTime[1] = Time.time;
			if (hitTime[1] - hitTime[0] <= 0.03f) { HitNote(true, true); interact_Control[1].Play("Active"); }
			else HitNote(true, false);
			interact_Control[0].Play("Active");
			interact_Control[2].Play("Active");
		}
		if (Input.GetButtonDown("KaL1"))
		{
			hitTime[2] = Time.time;
			if (hitTime[2] - hitTime[3] <= 0.03f) { HitNote(false, true); interact_Control[5].Play("Active"); }
			else HitNote(false, false);
			interact_Control[3].Play("Active");
			interact_Control[4].Play("Active");
		}
		if (Input.GetButtonDown("KaR1"))
		{
			hitTime[3] = Time.time;
			if (hitTime[3] - hitTime[2] <= 0.03f) { HitNote(false, true); interact_Control[4].Play("Active"); }
			else HitNote(false, false);
			interact_Control[3].Play("Active");
			interact_Control[5].Play("Active");
		}
		if (Input.GetButtonDown("Cancel")) StartCoroutine(gameManager.LoadScene("Song_Selection"));
	}

	void DisplayInformation()
	{
		title[0].text = GameManager.currentSong.title;
		title[1].text = GameManager.currentSong.subtitle;
		difficultySymbol.sprite = difficultySprite[GameManager.currentcourse.difficulty];
		string[] str = new string[4] { "简单", "普通", "困难", "魔王" };
		difficultyText.text = str[GameManager.currentcourse.difficulty];
	}

	IEnumerator DisableRendaBalloon()
	{
		currentRenda = 0;
		yield return new WaitForSeconds(0.8f);
		if (currentRenda == 0)
		{
			rendaBalloon[0].SetActive(false);
			rendaBalloon[1].GetComponent<Text>().text = "";
		}
	}

	IEnumerator GenerateGame() //每拍(四分音符)长度3，每小节4拍总长12，每拍秒数=60/BPM，每小节秒数=240/BPM
	{
		float measure = 4;
		int maxCombo = 0;
		int rendaState = 0;
		bool inCourse = false;
		foreach (string i in GameManager.currentcourse.course)
		{
			if (i == "#START") inCourse = true;
			if (inCourse)
			{
				if (!i.StartsWith("#"))
				{
					GenerateNote(0);
					foreach (char c in i.ToCharArray())
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
								else if (num >= 5 && num <= 6)
								{
									rendaState = num - 4;
									GenerateNote(num);
									GameObject obj = GenerateNote(num + 2);
									obj.transform.localScale = new Vector3(2.0f * measure / (i.Length - 1), 1, 1);
								}
								else if (num == 7 || num == 9)
								{
									GenerateNote(5);
									GameObject slide = GenerateNote(7);
									GameObject end = GenerateNote(9);
									GameObject obj = GenerateNote(11);
									slide.transform.localScale = new Vector3(2.0f * measure / (i.Length - 1), 1, 1);
									end.transform.Translate(Vector3.right * 3.0f * measure / (i.Length - 1));
									obj.name = "#RENDAEND";
									obj.transform.Translate(Vector3.right * 3.0f * measure / (i.Length - 1));
									obj.transform.Translate(Vector3.right * 0.5f);
								}
							}
							else
							{
								if (num == 8)
								{
									GenerateNote(rendaState + 8);
									GameObject obj = GenerateNote(11);
									obj.name = "#RENDAEND";
									obj.transform.Translate(Vector3.right * 0.5f);
									rendaState = 0;
								}
								else
								{
									GameObject obj = GenerateNote(rendaState + 6);
									obj.transform.localScale = new Vector3(2.0f * measure / (i.Length - 1), 1, 1);
								}
							}
							notePos += 3.0f * measure / (i.Length - 1);
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
		basicpoint[0] = 300; basicpoint[1] = 100;
		basicpoint[2] = (int)(10000 / 0.7f / maxCombo);
		if (GameManager.currentcourse.difficulty == 3) basicpoint[3] = basicpoint[2] / 2;
		else basicpoint[3] = (int)(basicpoint[2] * 3 / 4f);
		basicpoint[4] = (int)(basicpoint[2] / 2f * (GameManager.currentcourse.difficulty + 1));
		yield return new WaitForSeconds(2);
		noteParent.GetComponent<Rigidbody2D>().velocity = Vector2.left * GameManager.currentSong.bpm * 0.05f;
	}

	GameObject GenerateNote(int index)
	{
		Vector2 pos = new Vector2(21.09f, 1.29f);
		return Instantiate(note[index], pos + Vector2.right * notePos, Quaternion.identity, noteParent.transform);
	}

	void DestroyNote()
	{
		RaycastHit2D[] hit = Physics2D.LinecastAll(new Vector2(-3.75f, 1.3f), new Vector2(-6, 1.3f));
		if (hit != null)
			foreach (RaycastHit2D i in hit)
			{
				if (i.collider.OverlapPoint(new Vector2(-3.75f, 1.3f)) && (i.collider.tag == "Don" || i.collider.tag == "Ka"))
				{
					Destroy(i.collider);
					Destroy(i.collider.gameObject, 1);
					miss = true;
					donchan.SetBool("Miss", true);
					if (!gogoTime) donchan.Play("Miss");
					count[2]++;
					currentCombo = 0;
					loseFullCombo = true;
					comboText.text = "";
					comboText.fontSize = 54;
					soulPoint -= basicpoint[4];
					soulPoint = System.Math.Max(soulPoint, 0);
				}
				if (i.collider.OverlapPoint(new Vector2(-4.5f, 1.3f)) && !i.collider.OverlapPoint(new Vector2(-3.6f, 1.3f))) Destroy(i.collider.gameObject);
			}
				
	}

	void HitNote(bool don, bool big)
	{
		Collider2D[] hit = Physics2D.OverlapPointAll(hitPoint.position, mask);
		if (hit != null)
			foreach(Collider2D i in hit)
			{
				if ((i.tag == "Don" && don) || (i.tag == "Ka" && !don))
				{
					float offset = i.transform.position.x - hitPoint.position.x;
					if (offset < -0.35f || offset > 0.4f)
					{
						miss = true;
						donchan.SetBool("Miss", true);
						if (!gogoTime) donchan.Play("Miss");
						count[2]++;
						currentCombo = 0;
						loseFullCombo = true;
						comboText.text = "";
						comboText.fontSize = 54;
						soulPoint -= basicpoint[4];
						soulPoint = System.Math.Max(soulPoint, 0);
						Instantiate(hitBad[0], parent);
					}
					else
					{
						miss = false;
						donchan.SetBool("Miss", false);
						currentCombo++;
						currentCombo = System.Math.Min(currentCombo, 9999);
						if (count[4] < currentCombo) count[4] = currentCombo;
						if (currentCombo % 10 == 0)
						{
							if (soulPoint >= 10000) donchan.Play("Jump_G");
							else donchan.Play("Jump");
						}
						if (currentCombo % 100 == 0)
						{
							comboObjects[0].SetActive(true);
						}
						interact_Control[6].Play("Active");
						if (offset >= -0.2f && offset <= 0.2f) 
						{
							count[0]++;
							Instantiate(hitExcellent[0], parent);
							soulPoint += basicpoint[2];
							if (big && i.name.Contains("L"))
							{
								hitScore *= 2;
							}
							else
							{
								hitScore *= 1;
							}
						}
						else
						{
							count[1]++;
							Instantiate(hitGood[0], parent);
							soulPoint += basicpoint[3];
							if (big && i.name.Contains("L"))
							{
								hitScore *= 1;
							}
							else
							{
								hitScore = hitScore / 2 - ((hitScore / 2) % 10);
							}
						}
						if (gogoTime) hitScore = (int)(hitScore * 1.1f) - ((int)(hitScore * 1.1f) % 10);
						score += hitScore;
						soulPoint = System.Math.Min(soulPoint, 10000);
					}
					Destroy(i.gameObject);
				}
				else if (i.tag == "Renda")
				{
					count[3]++;
					currentRenda++;
					currentRenda = System.Math.Min(currentRenda, 9999);
					rendaBalloon[0].SetActive(true);
					rendaBalloon[1].GetComponent<Text>().text = currentRenda.ToString();
					int rendaScore = 100;
					if (i.name.Contains("L")) rendaScore *= 2;
					interact_Control[6].Play("Active");
					score += rendaScore;
				}
				score = System.Math.Min(score, 999999999);
			}
	}
	IEnumerator ShowResult()
	{
		yield return new WaitForSeconds(1f);
		GameManager.fading = true;
		GameManager.fadingScreen.Play("Fading");
		yield return new WaitForSeconds(1.5f);
		GameManager.state = GameManager.GameState.Result;
		resultScreen.SetActive(true);
		if (soulPoint >= requireSoulPoint[GameManager.currentcourse.difficulty])
		{
			resultImage[0].sprite = resultScreenSprite[0];
			if(!loseFullCombo) resultImage[1].sprite = resultScreenSprite[2];
			else resultImage[1].sprite = resultScreenSprite[1];
			float width = 448 + 183 * (soulPoint - requireSoulPoint[GameManager.currentcourse.difficulty]) / (10000 - requireSoulPoint[GameManager.currentcourse.difficulty]);
			resultSoulGauge.sizeDelta = new Vector2(width, soulGauge.sizeDelta.y);
		}
		else
		{
			float width = 448 * soulPoint / requireSoulPoint[GameManager.currentcourse.difficulty];
			resultSoulGauge.sizeDelta = new Vector2(width, soulGauge.sizeDelta.y);
		}
		for (int i = 0; i <= 4; i++) resultText[i].text = count[i].ToString();
		string[] str = new string[4] { "简单", "普通", "困难", "魔王" };
		resultText[5].text = str[GameManager.currentcourse.difficulty];
		resultText[6].text = score.ToString();
		GameManager.fadingScreen.Play("Fadeout");
		yield return new WaitForSeconds(1);
		GameManager.fading = false;
	}
}
