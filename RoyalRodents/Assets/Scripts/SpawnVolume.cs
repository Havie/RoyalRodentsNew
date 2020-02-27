using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVolume : MonoBehaviour
{
    
    private bool _timeToSpawn;
    // private List<eRodentTypes> _AvailableRodents = new List<eRodentTypes>();
    private ArrayList _AvailableRodents = new ArrayList();

    private bool _occupied;

    GameObject Rat;



    public enum eRodentTypes { Rat };

    // Start is called before the first frame update
    void Start()
    {
        _timeToSpawn = true;
        AddType("Rat");

        Rat = Resources.Load<GameObject>("Rodent/FatRat/RatPreFab");

    }

    // Update is called once per frame
    void Update()
    {
        if(_timeToSpawn)
        {
            //Spawn random rodent based on prefab
            int index = Random.Range(0, _AvailableRodents.Count - 1);
            eRodentTypes selected=(eRodentTypes)_AvailableRodents[index];

            SpawnRodent(selected);


            StartCoroutine(SpawnCountDown());
        }
    }

    IEnumerator SpawnCountDown()
    {
        //Need some other constraints on an overall limit to how many spawn

        float _startTime = Time.time;
        _timeToSpawn = false;
        yield return new WaitForSeconds(5f);
        while(_startTime>0)
        {
            _startTime -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        _timeToSpawn = true;
    }

    public void AddType(string s)
    {
        // Add a type to be available to spawn
        if (s.Equals("Rat"))
            _AvailableRodents.Add(eRodentTypes.Rat);
    }

    private void SpawnRodent(eRodentTypes type)
    {
        if (!_occupied)
        {
            if (type == eRodentTypes.Rat)
            {
                _occupied = true;
                GameObject _spawnedRat = GameObject.Instantiate(Rat, this.transform.position, this.transform.rotation);

                //parent this thing to this obj keep heirarchy cleaner? Might end up negatively affecting the subject Script?
                _spawnedRat.transform.SetParent(this.transform);

                // Set Team Neutral
                _spawnedRat.tag = "NeutralRodent";
                // Ensure Sprite is Neutral
                // Increase some kind of count
            }
        }
    }
}
