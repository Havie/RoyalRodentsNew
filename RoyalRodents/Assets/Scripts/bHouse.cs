using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bHouse :MonoBehaviour
{
    private Sprite _builtSpriteLevel1;
	private Sprite _builtSpriteLevel2;
	private Sprite _builtSpriteLevel3;

	private float _hitpoints = 50;
    private float _hitPointGrowth = 10;

	//create structure costs (costLevel1 is used to BUILD TO level 1, not ON level 1)
	public static Dictionary<string, int> _costLevel1 = new Dictionary<string, int>();
	public static Dictionary<string, int> _costLevel2 = new Dictionary<string, int>();
	public static Dictionary<string, int> _costLevel3 = new Dictionary<string, int>();

	//create Housing Capacity amounts per level
	private int[] capacityIncrementAmount = new int[4];

	private static bool _isSet;

	void Start()
    {
        _builtSpriteLevel1 = Resources.Load<Sprite>("Buildings/House/trash_house");
		_builtSpriteLevel2 = Resources.Load<Sprite>("Buildings/House/wood_house");
		_builtSpriteLevel3 = Resources.Load<Sprite>("Buildings/House/stone_house");

		//how much total each level contributes to the population capacity
		capacityIncrementAmount[0] = 0;
		capacityIncrementAmount[1] = 2;
		capacityIncrementAmount[2] = 5;
		capacityIncrementAmount[3] = 8;
	}

	private static void setupCosts()
	{
		if (!_isSet)
		//Set Upgrade/Build Costs (1-3 levels)
		{
			_costLevel1.Add("Trash", 2);

			_costLevel2.Add("Trash", 4);
			_costLevel2.Add("Wood", 2);

			_costLevel3.Add("Trash", 6);
			_costLevel3.Add("Wood", 4);
			_costLevel3.Add("Metal", 2);

			_isSet = true;
		}
	}

    public float BuildingComplete(int level)
    {
		//Set new structure sprite
		if (level == 1)
		{
			this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel1;
		}
        else if (level == 2)
		{
			this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel2;
		}
	    else if (level == 3)
		{
			this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel3;
		}
		//increment Population Capacity
		ResourceManagerScript.Instance.incrementPopulationCapacity(capacityIncrementAmount[level] - capacityIncrementAmount[level-1]);

		return (_hitpoints + (_hitPointGrowth*level));
    }

	public void DemolishAction(int level)
	{
		ResourceManagerScript.Instance.incrementPopulationCapacity(-capacityIncrementAmount[level]);
	}
   
	public static Dictionary<string, int> getCost(int level)
	{

		if(_costLevel1.Count == 0)
		{
			setupCosts();
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

}
