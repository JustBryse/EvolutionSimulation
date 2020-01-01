using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SimulationManager : MonoBehaviour {

    public TextAsset simData;
    public bool simIsActive = false;

    public Food food;
    public Organism organism;
    public float simSpeed = 1f;
    public int rounds = 20;

    public int startFoodCount = 20;
    public int startOrganismCount = 10;

    private int roundCounter = 0;

    private UIManager uim;

    // Use this for initialization
    void Start()
    {
        uim = UIManager.FindObjectOfType<UIManager>();
        clearSimData();
        startFirstRound();
    }

    // spawns all the food for the current round
    private void spawnFood()
    {
        GameObject environment = GameObject.Find("Environment");
        float xScale = environment.transform.lossyScale.x;
        float zScale = environment.transform.lossyScale.z;

        for (int i = 0; i < startFoodCount; i++)
        {
            float x = Random.Range(-xScale/2 + 3, xScale/2 - 3);
            float z = Random.Range(-zScale/2 + 3, zScale/2 - 3);

            Vector3 spawnPos = new Vector3(x, 1, z);
            Instantiate(food.gameObject, spawnPos, Quaternion.identity);
        }
    }

    // spawns the offspring of the surviving organisms for this round, and resets the survivors
    private void spawnAndResetOrganisms()
    {
        // destroy leftover organisms who did not find food
        Organism[] organisms = Organism.FindObjectsOfType<Organism>();
        for (int i = 0; i < organisms.Length; i++)
        {
            if (organisms[i].getFoundFood() == false)
            {
                organisms[i].die();
            }
            else
            {
                organisms[i].makeOffspring();
            }
        }
   
        // reset organisms and move them to there start positions for the next round
        foreach (Organism org in Organism.FindObjectsOfType<Organism>())
        {
            org.reset();
            org.transform.position = getOrganismStartPos();
        }
    }

    private void turnOrganismsOff()
    {
        Organism[] organisms = Organism.FindObjectsOfType<Organism>();
        foreach (Organism survivor in organisms)
        {
            survivor.gameObject.SetActive(false);
        }

        StartCoroutine(turnOrganismsOn(0.1f, organisms));
    }

    IEnumerator turnOrganismsOn(float delay, Organism[] organisms)
    {
        yield return new WaitForSeconds(delay);

        foreach (Organism survivor in organisms)
        {
            survivor.gameObject.SetActive(true);
        }
    }

    private void startFirstRound()
    {
        roundCounter++;
        uim.setRoundText("Round " + roundCounter);
        spawnFood();

        for (int i = 0; i < startOrganismCount; i++)
        {
            Vector3 spawnPos = getOrganismStartPos();
            GameObject newOrganism = (GameObject) Instantiate(organism.gameObject, spawnPos, Quaternion.identity);
        }

        writeRoundData();

        simIsActive = true;

        Invoke("turnOrganismsOff", 0.1f);
    }

    private Vector3 getOrganismStartPos()
    {
        Vector3 spawnPos = Vector3.zero;

        GameObject environment = GameObject.Find("Environment");

        float xScale = environment.transform.lossyScale.x;
        float zScale = environment.transform.lossyScale.z;

        int quarter = (int)Mathf.Round(Random.value * 4);

        if (quarter == 1) // spawn on -z perimeter
        {
            float x = Random.Range(-xScale / 2, xScale / 2);
            float z = -zScale / 2;

            spawnPos = new Vector3(x, 1.5f, z);
        }
        else if (quarter == 2) // spawn on +z perimeter
        {
            float x = Random.Range(-xScale / 2, xScale / 2);
            float z = zScale / 2;

            spawnPos = new Vector3(x, 1.5f, z);
        }
        else if (quarter == 3) // spawn on -x perimeter
        {
            float x = -xScale / 2;
            float z = Random.Range(-zScale / 2, zScale / 2);

            spawnPos = new Vector3(x, 1.5f, z);
        }
        else // spawn on +x perimeter
        {
            float x = xScale / 2;
            float z = Random.Range(-zScale / 2, zScale / 2);

            spawnPos = new Vector3(x, 1.5f, z);
        }

        return spawnPos;
    }

    
    private void startRound()
    {
        roundCounter++;
        uim.setRoundText("Round " + roundCounter);

        // remove all food objects that are still in the environment
        foreach (Food food in Food.FindObjectsOfType<Food>())
        {
            Destroy(food.gameObject);
        }

        Food.foodCount = 0; // reset the foodCount because there is no food at the start of a round

        // instantiate everything
        spawnFood();
        spawnAndResetOrganisms();

        // write results of the round here
        writeRoundData();

        simIsActive = true;

        Invoke("turnOrganismsOff", 0.1f);
    }
    
    public void endRound()
    {
        simIsActive = false;

        if (roundCounter == rounds)
        {
            // end the simulation
            return;
        }
        startRound();
    }

    // clears the contents of the simulation data text file
    private void clearSimData()
    {
        string path = "Assets/simData.txt";
        File.WriteAllText(path, System.String.Empty);
    }

    // allows additions to the simulation data text file
    private void writeData(string data)
    {
        string path = "Assets/simData.txt";
        //print(path);
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(data);
        writer.Close();
    }

    // writes the data about the organisms at the start of the round
    private void writeRoundData()
    {
        Organism[] roundOrgs = Organism.FindObjectsOfType<Organism>();

        for (int i = 0; i < roundOrgs.Length; i++)
        {
            Organism org = roundOrgs[i];
            string orgData = roundCounter + "/" + (org.GetInstanceID()*-1) + "/" + org.startSpeed + "/" + org.startAwareness;
            writeData(orgData);
        }
    }
}
