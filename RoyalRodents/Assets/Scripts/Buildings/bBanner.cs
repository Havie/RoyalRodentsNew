using System.Collections;
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


    //unique to banner
    private float _hpBonusLvl1 = 1.10f;
    private float _hpBonusLvl2 = 1.15f;
    private float _hpBonusLvl3 = 1.25f;


    private float _gatherBonusLvl1 = 1.05f;
    private float _gatherBonusLvl2 = 1.08f;
    private float _gatherBonusLvl3 = 1.13f;


    private static bool _isSet;

	//create strucutre costs (costLevel1 is used to BUILD TO level 1, not ON level 1)
	public static Dictionary<ResourceManagerScript.ResourceType, int> _costLevel1 = new Dictionary<ResourceManagerScript.ResourceType, int>();
    public static Dictionary<ResourceManagerScript.ResourceType, int> _costLevel2 = new Dictionary<ResourceManagerScript.ResourceType, int>();
    public static Dictionary<ResourceManagerScript.ResourceType, int> _costLevel3 = new Dictionary<ResourceManagerScript.ResourceType, int>();

    // Start is called before the first frame update
    void Start()
    {
		SetUpComponent();
        StartCoroutine(EnemyCheckDelay());
	}


	private static void SetUpComponent()
	{
		if (!_isSet)
		{
			//Set Upgrade/Build Costs (1-3 levels)
			_costLevel1.Add(ResourceManagerScript.ResourceType.Trash, 6);

			_costLevel2.Add(ResourceManagerScript.ResourceType.Trash, 9);
			_costLevel2.Add(ResourceManagerScript.ResourceType.Wood, 6);

			_costLevel3.Add(ResourceManagerScript.ResourceType.Trash, 14);
			_costLevel3.Add(ResourceManagerScript.ResourceType.Wood, 9);
			_costLevel3.Add(ResourceManagerScript.ResourceType.Stone, 6);

			_builtSpriteLevel1 = Resources.Load<Sprite>("Buildings/Banner/trash_banner");
			_builtSpriteLevel2 = Resources.Load<Sprite>("Buildings/Banner/wood_banner");
			_builtSpriteLevel3 = Resources.Load<Sprite>("Buildings/Banner/stone_banner");

			_isSet = true;
		}
	}

    IEnumerator EnemyCheckDelay()
    {
        yield return new WaitForSeconds(1);
        CheckEnemy();
    }

    public void CheckEnemy()
    {
        BuildableObject b = this.GetComponent<BuildableObject>();
        if (b)
        {
            if (b.getTeam() == 2)
            {
                b.SetType("Banner");
                b.SetLevel(1);
                b.BuildComplete();
            }
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

    public float getHPBonus()
    {
        BuildableObject building = GetComponent<BuildableObject>();
        int level = building.getLevel();

        if (level == 1)
            return _hpBonusLvl1;
        else if (level == 2)
            return _hpBonusLvl2;
        else if (level == 3)
            return _hpBonusLvl3;

        return 0;
    }
    public float getGatherBonus()
    {
        BuildableObject building = GetComponent<BuildableObject>();
        int level = building.getLevel();

        if (level == 1)
            return _gatherBonusLvl1;
        else if (level == 2)
            return _gatherBonusLvl2;
        else if (level == 3)
            return _gatherBonusLvl3;

        return 0;
    }
}
