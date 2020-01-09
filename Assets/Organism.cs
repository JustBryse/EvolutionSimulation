using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Organism : MonoBehaviour {

    public float startSpeed;
    public float startAwareness;
    public float startEnergy; // this variable cannot change by mutations
    public float mutationValue = 0.05f;

    private float speed;
    private float awareness;
    private float energy;
    private bool foundFood = false;
    private float energyConsumption; // this variable controls the rate of energy consumption and is calculated at the start

    private List<NavTile> unseenNavTiles; // stores all the navTiles that have not passed inside the awareness range of this organism. This is for when the organism has to wander to find food

    private NavMeshAgent nma;
    private CapsuleCollider capCollider;
    private SimulationManager sm;

	// Use this for initialization
	void Start ()
    {
        speed = startSpeed;
        awareness = startAwareness;
        energy = startEnergy;
        energyConsumption = Mathf.Pow(speed, 2) + awareness;

        nma = GetComponent<NavMeshAgent>();
        nma.speed = speed;

        capCollider = GetComponent<CapsuleCollider>();
        capCollider.radius = awareness-1f;

        unseenNavTiles = new List<NavTile>();
        unseenNavTiles.AddRange(NavTile.FindObjectsOfType<NavTile>());

        sm = SimulationManager.FindObjectOfType<SimulationManager>();
        setColor();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (foundFood == false && sm.simIsActive == true)
        {
            findFood();
        }
        else if(sm.simIsActive == true)
        {
            Organism[] livingOrganisms = Organism.FindObjectsOfType<Organism>();

            for (int i = 0; i < livingOrganisms.Length; i++)
            {
                if (livingOrganisms[i].getFoundFood() == false)
                {
                    // if there are other organisms still alive and searching for food then do nothing
                    return;
                }
            }

            // if after the loop every living organism has found food than end the round
            sm.endRound();
            //print("called sm.endRound() from organism");
        }

        //print("Unseen navtiles: " + unseenNavTiles.Count);
        //print("Energy: " + energy);
	}

    private void findFood()
    {
        Food[] food = Food.FindObjectsOfType<Food>();

        // only keep looking for food if there is food left on the map and this organism still has energy
        if (food.Length <= 0 || energy <= 0)
        {
            // destroy self
            die();
        }

        Food closestFood = null;
        float closestDistance = 0f;

        for (int i = 0; i < food.Length; i++)
        {
            Food otherFood = food[i];
            float otherDistance = Vector3.Distance(otherFood.transform.position, transform.position);

            // continue to next index if the current food object is out of awareness range
            if (otherDistance > awareness) 
            {
                continue;
            }

            // if the current food item is in awareness range and closest food is not yet initialized then assign closestFood to the current food item, then continue
            if (closestFood == null)
            {
                closestFood = otherFood;
                closestDistance = Vector3.Distance(transform.position, closestFood.transform.position);
                continue;
            }

            // if the otherFood is closer than make it the new closestFood
            if (otherDistance < closestDistance)
            {
                closestFood = otherFood;
                closestDistance = otherDistance;
            }
        }

        // if closestFood is still null than there is food still in play but not within awareness range
        // this organism must now search for food by intelligently wandering around
        if (closestFood == null)
        {
            NavTile closestTile = unseenNavTiles[0];
            float closestTileDistance = Vector3.Distance(closestTile.transform.position, transform.position);

            foreach (NavTile tile in unseenNavTiles)
            {
                float tileDistance = Vector3.Distance(tile.transform.position, transform.position);

                if (tileDistance < closestTileDistance)
                {
                    closestTile = tile;
                    closestTileDistance = tileDistance;
                }
            }

            move(closestTile.transform.position);
        }
        else // if closestFood is not null then travel to its location
        {
            move(closestFood.transform.position);
        }

        // decrement this organism's energy supply
        energy -= energyConsumption * Time.deltaTime;
    }

    // sets the color of this organism according to its species traits
    private void setColor()
    {
        float r = (speed - 2f) / 5f;
        float g = (speed + awareness - 4f) / 10f;
        float b = (awareness - 2f) / 5f;

        Color color = new Color(r, g, b, 1f);
        GetComponent<MeshRenderer>().material.color = color;
    }

    // called when this organism is supposed to die
    public void die()
    {   
        Destroy(gameObject);
    }

    // resets this organism for the next round
    public void reset()
    {
        foundFood = false;
        energy = startEnergy;
        unseenNavTiles = new List<NavTile>();
        unseenNavTiles.AddRange(NavTile.FindObjectsOfType<NavTile>());
    }

    // used for moving the organism 
    private void move(Vector3 destination) 
    {
        destination.y = transform.position.y;
        nma.SetDestination(destination);
    }

    public void setFoundFood(bool flag)
    {
        foundFood = flag;
    }

    public bool getFoundFood()
    {
        return foundFood;
    }

    public void makeOffspring()
    {
        float childSpeed = startSpeed + (startSpeed * Random.Range(-mutationValue, mutationValue));
        float childAwareness = startAwareness + (startAwareness * Random.Range(-mutationValue, mutationValue));

        GameObject child = (GameObject)Instantiate(gameObject, new Vector3(transform.position.x, 3f, transform.position.z), Quaternion.identity);
        Organism childOrganism = child.GetComponent<Organism>();
        childOrganism.startSpeed = childSpeed;
        childOrganism.startAwareness = childAwareness;
        childOrganism.updateTraits(childSpeed, childAwareness);
    }

    // called when offspring are created because offspring initially have the same trait values as their parents
    // this function updates the offspring with the mutated trait values
    private void updateTraits(float speed, float awareness)
    {
        this.speed = speed;
        this.awareness = awareness;
        setColor();
    }

    // called when this object's collider interacts with another collider
    void OnTriggerEnter(Collider col)
    {
        //print("check from ontriggerenter");
        // when a navTile is seen, remove it from the unseen navTile list
        if (col.GetComponent<NavTile>())
        {
            //col.GetComponent<MeshRenderer>().enabled = true;
            unseenNavTiles.Remove(col.GetComponent<NavTile>());
        }
    }

}
