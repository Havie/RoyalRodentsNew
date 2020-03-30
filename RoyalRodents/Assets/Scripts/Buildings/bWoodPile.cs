using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bWoodPile : MonoBehaviour
{
	private static Sprite _builtSpriteLevel1;
	private static int maxLevel = 1;

	private static float _hitpoints = 50;
	private float _hitPointGrowth = 10;

	private static bool _isSet;

	//create strucutre costs (costLevel1 is used to BUILD TO level 1, not ON level 1)
	public static Dictionary<ResourceManagerScript.ResourceType, int> _costLevel1 = new Dictionary<ResourceManagerScript.ResourceType, int>();

	// Start is called before the first frame update
	void Start()
	{
		SetUpComponent();
		StartingBuildComplete();
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
			_costLevel1.Add(ResourceManagerScript.ResourceType.Trash, 1);

			_builtSpriteLevel1 = Resources.Load<Sprite>("Buildings/WoodPile/wood_pile");

			_isSet = true;
		}
	}

	public float BuildingComplete(int level)
	{
		if (!_isSet)
			SetUpComponent();

		if (level == 1)
			this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel1;

		return (_hitpoints + (_hitPointGrowth * level));
	}

	public void StartingBuildComplete()
	{
		if (!_builtSpriteLevel1)
		{
			_builtSpriteLevel1 = Resources.Load<Sprite>("Buildings/GarbageCan/garbagecan");
		}
		this.transform.GetComponent<BuildableObject>().SetType("GarbageCan");
		this.transform.GetComponent<BuildableObject>().SetLevel(1);
		this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel1;

		//Add Searchable Component
		Searchable s = this.gameObject.AddComponent<Searchable>();
		s.setGatherResource(ResourceManagerScript.ResourceType.Wood, 1);

		//MEGA hack of all Hacks
		// this.transform.GetComponentInChildren<Employee>().gameObject.GetComponentInChildren<eWorkerOBJ>().gameObject.GetComponent<SpriteRenderer>().sprite = null;
		BuildableObject bo = this.GetComponent<BuildableObject>();
		foreach (Employee e in bo._Workers)
		{
			GameObject go = e.gameObject;
			if (go)
			{
				eWorkerOBJ worker = go.GetComponentInChildren<eWorkerOBJ>();
				if (worker)
				{
					go = worker.gameObject;
					if (go)
					{
						SpriteRenderer sp = go.GetComponent<SpriteRenderer>();
						if (sp)
							sp.sprite = null;
					}
				}
			}
		}
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
		else
			return null;
	}

	public static int getMaxLevel()
	{
		return maxLevel;
	}

	public static float getHPStats()
	{
		return _hitpoints;
	}
}
