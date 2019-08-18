using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public void DestroyObj()
	{
		Destroy(gameObject);
	}
	public void DisableObj()
	{
		gameObject.SetActive(false);
	}
}
