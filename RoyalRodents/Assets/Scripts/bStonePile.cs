using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bStonePile : MonoBehaviour
{
	private Sprite _builtSpriteLevel1;
	private static int maxLevel = 1;

	private float _hitpoints = 50;
	private float _hitPointGrowth = 10;

	private static bool _isSet;

	//create strucutre costs (costLevel1 is used to BUILD TO level 1, not ON level 1)
	public static Dictionary<string, int> _costLevel1 = new Dictionary<string, int>();

	// Start is called before the first frame update
	void Start()
	{
		_builtSpriteLevel1 = Resources.Load<Sprite>("Buildings/Banner/trash_banner");
	}

	// Update is called once per frame
	void Update()
	{

	}

	private static void setupCosts()
	{
		if (!_isSet)
		{
			//Set Upgrade/Build Costs (1-3 levels)
			_costLevel1.Add("Trash", 1);

			_isSet = true;
		}
	}

	public float BuildingComplete(int level)
	{
		if (level == 1)
			this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel1;

		return (_hitpoints + (_hitPointGrowth * level));
	}

	public static Dictionary<string, int> getCost(int level)
	{

		if (_costLevel1.Count == 0)
		{
			setupCosts();
		}

		if (level == 1)
		{
			return _costLevel1;
		}
		else
			return null;
	}

	public static int getMaxLevel()
	{
		return maxLevel;
	}
}
