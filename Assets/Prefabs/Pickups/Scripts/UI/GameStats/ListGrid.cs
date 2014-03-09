using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerComparer : IComparer<PhotonPlayer>
{
	public int Compare(PhotonPlayer player1, PhotonPlayer player2)
	{

		int compare;
		if (PlayerHelper.Get<int>(player1,"Score",0) < PlayerHelper.Get<int>(player2,"Score",0))
			compare = 1;
		else
			compare = -1;


		return compare;
	}
	
}

public class ListGrid : MonoBehaviour {

	public string Title;
	public GameObject ListLabelPrefab;
	
	protected SortedList <PhotonPlayer,string> sortedList = new SortedList<PhotonPlayer, string>(new PlayerComparer());


	// Use this for initialization
	IEnumerator StartUpdating () {

		while (true)
		{
			UpdateList();

			yield return new WaitForSeconds(1);

		}
	}

	
	void OnEnable()
	{
		StartCoroutine(StartUpdating());
	}
		
	protected virtual List<string> GetList()
	{
		// override this

		return null;
	}

	void UpdateList()
	{

		int targetNumChildren = GetList().Count+1;

		if (targetNumChildren > transform.childCount)
		{
			int numToAdd = targetNumChildren - transform.childCount;

			for (int i=0; i < numToAdd; i++)
			{
				GameObject labelObj = NGUITools.AddChild(gameObject,ListLabelPrefab);

				labelObj.GetComponent<UILabel>().MakePixelPerfect();
		
			}
			GetComponent<UIGrid>().Reposition();
		}
		else if (targetNumChildren < transform.childCount)
		{
			int numToRemove = targetNumChildren - GetList().Count;

			for (int i = 0; i < numToRemove; i ++)
			{
				DestroyImmediate( transform.GetChild(i).gameObject );

			}

		}

		for (int i=0; i < transform.childCount; i++)
		{
			if (i==0)
				transform.GetChild(i).GetComponent<UILabel>().text = Title;
			else
				transform.GetChild(i).GetComponent<UILabel>().text = GetList()[i-1];

		}

	}
}
