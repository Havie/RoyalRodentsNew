﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bBanner : MonoBehaviour
{
	private static Sprite _builtSpriteLevel1;
	private static Sprite _builtSpriteLevel2;
	private static Sprite _builtSpriteLevel3;
	private static int maxLevel = 3;

	private float _hitpoints = 50;
	private float _hitPointGrowth = 10;

	private static bool _isSet;

	//create strucutre costs (costLevel1 is used to BUILD TO level 1, not ON level 1)
	public static Dictionary<ResourceManagerScript.ResourceType, int> _costLevel1 = new Dictionary<ResourceManagerScript.ResourceType, int>();
    public static Dictionary<ResourceManagerScript.ResourceType, int> _costLevel2 = new Dictionary<ResourceManagerScript.ResourceType, int>();
    public static Dictionary<ResourceManagerScript.ResourceType, int> _costLevel3 = new Dictionary<ResourceManagerScript.ResourceType, int>();

    // Start is called before the first frame update
    void Start()
    {
		SetUpComponent();
	}

    // Update is called once per frame
    void Update()
    {

    }

	private static void SetUpComponent()
	{
		if (!_isSet)
		{
			//Set Upgrade/Build Costs (1-3 levels)
			_costLevel1.Add(ResourceManagerScript.ResourceType.Trash, 2);

			_costLevel2.Add(ResourceManagerScript.ResourceType.Trash, 4);
			_costLevel2.Add(ResourceManagerScript.ResourceType.Wood, 2);

			_costLevel3.Add(ResourceManagerScript.ResourceType.Trash, 6);
			_costLevel3.Add(ResourceManagerScript.ResourceType.Wood, 4);
			_costLevel3.Add(ResourceManagerScript.ResourceType.Stone, 2);

			_builtSpriteLevel1 = Resources.Load<Sprite>("Buildings/Banner/trash_banner");
			_builtSpriteLevel2 = Resources.Load<Sprite>("Buildings/Banner/wood_banner");
			_builtSpriteLevel3 = Resources.Load<Sprite>("Buildings/Banner/stone_banner");

			_isSet = true;
		}
	}

	public float BuildingComplete(int level)
	{
		if (!_isSet)
			SetUpComponent();

		if (level == 1)
			this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel1;
		else if (level == 2)
			this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel2;
		else if (level == 3)
			this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel3;

		return (_hitpoints + (_hitPointGrowth * level));
	}

	public static Dictionary<ResourceManagerScript.ResourceType, int> getCost(int level)
	{

		if (_costLevel1.Count == 0)
		{
			SetUpComponent();
		}

		if (level == 1)
		{
			return _costLevel1;
		}
		else if (level == 2)
		{
			return _costLevel2;
		}
		else if (level == 3)
		{
			return _costLevel3;
		}
		else
			return null;
	}

	public static int getMaxLevel()
	{
		return maxLevel;
	}
}