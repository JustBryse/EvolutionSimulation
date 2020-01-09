using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour {

    public static int foodCount = 0;

    SimulationManager sm;

	// Use this for initialization
	void Start ()
    {
        sm = SimulationManager.FindObjectOfType<SimulationManager>();

        foodCount++;
	}

    // the food object will inform an organism that it has found food instead of an organism informing itself
    // this is because two organisms may touch the same food object at the same time before the food object can destroy itself
    // this way, one organism collider interaction can be handled at a time so the first organism to arrive at this food object gets it
    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<OrganismFoodCollider>())
        {
            // only give this food object if the organism needs it
            if (col.transform.parent.GetComponent<Organism>().getFoundFood() == false)
            {
                col.transform.parent.GetComponent<Organism>().setFoundFood(true);
                die();
            }
        }
    }

    // destroys this object
    private void die()
    {
        foodCount--;

        if (foodCount < 1)
        {
            // trigger the end of this round
            sm.endRound();
        }

        // self destruct
        Destroy(gameObject);
    }
}
