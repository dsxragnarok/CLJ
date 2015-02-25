using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DifficultyProgress : Entity {

	// Object containing level upgrade parameters
	[Serializable]
	public class LevelChange
	{
		public int levelRequirement;
		public float changeSpawnInterval;
		public int modifyBlueAmount;
		public int modifyRedAmount;
		public int modifyBlackAmount;
	}

	// List of level upgrade parameters
	public List<LevelChange> levelChanges;

	// Level tracker
	public int level = 0;

	// Use this for initialization
	public override void Start () {
	
	}
	
	// Update is called once per frame
	public override void Update () {
	
	}

	// Called incrementing level and checking to see if the next level requirements are reached.
	// If so, we modify the game difficulty state.
	public void LevelUp() {
		level++;

		while (levelChanges.Count > 0 && levelChanges[0].levelRequirement <= level)
		{
			if (gameMaster.BirdSpawner)
			{
				// Extract level change parameters
				float changeSpawnInterval = levelChanges[0].changeSpawnInterval;
				int modifyBlueAmount = levelChanges[0].modifyBlueAmount;
				int modifyRedAmount = levelChanges[0].modifyRedAmount;
				int modifyBlackAmount = levelChanges[0].modifyBlackAmount;

				// Change game spawn interval
				gameMaster.BirdSpawner.spawnInterval += changeSpawnInterval;
				gameMaster.BirdSpawner.spawnInterval = UnityEngine.Mathf.Max (gameMaster.BirdSpawner.spawnInterval, 0.01f);

				// Modify spawn chances if set in this level change criteria
				bool modifiedAmount = false;
				if (System.Math.Abs (modifyBlueAmount) > 0)
				{
					modifiedAmount = true;
					SpawnBirds.SpawnCriteria criteria = gameMaster.BirdSpawner.FindSpawnCriteria(BirdController.BirdType.BLUE);
					if (criteria != null) criteria.amount += modifyBlueAmount;
				}
				if (System.Math.Abs (modifyRedAmount) > 0)
				{
					modifiedAmount = true;
					SpawnBirds.SpawnCriteria criteria = gameMaster.BirdSpawner.FindSpawnCriteria(BirdController.BirdType.RED);
					if (criteria != null) criteria.amount += modifyRedAmount;
				}
				if (System.Math.Abs (modifyBlackAmount) > 0)
				{
					modifiedAmount = true;
					SpawnBirds.SpawnCriteria criteria = gameMaster.BirdSpawner.FindSpawnCriteria(BirdController.BirdType.BLACK);
					if (criteria != null) criteria.amount += modifyBlackAmount;
				}
				if (modifiedAmount)
					gameMaster.BirdSpawner.UpdateCriteriaTotalAmount();
			}

			// Remove the current level requirement since we just applied its changes
			levelChanges.RemoveAt(0);
		}
	}

}
