/*
 * Created :    Spring 2023
 * Author :     SeungGeon Kim (keithrek@hanmail.net)
 * Project :    FogWar
 * Filename :   csRevealerSpawner.cs (non-static monobehaviour module)
 * 
 * All Content (C) 2022 Unlimited Fischl Works, all rights reserved.
 */



using UnityEngine;          // Monobehaviour



namespace FischlWorks_FogWar
{



    public class csRevealerSpawner : MonoBehaviour
    {
        [SerializeField]
        private csFogWar fogWar = null;

        [SerializeField]
        private GameObject exampleRevealer = null;



        private void Awake()
        {
            if (fogWar == null)
                Debug.LogError("Fog reference is missing!");
        }



        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Vector3 randomPoint = new Vector3(
                    Random.Range(-fogWar.levelData.levelDimensionX / 2.5f, fogWar.levelData.levelDimensionX / 2.5f),
                    fogWar._LevelMidPoint.position.y + 0.5f,
                    Random.Range(-fogWar.levelData.levelDimensionY / 2.5f, fogWar.levelData.levelDimensionY / 2.5f));

                // Instantiating & fetching the revealer Transform
                Transform randomTransform = Instantiate(exampleRevealer, randomPoint, Quaternion.identity).GetComponent<Transform>();

                // Utilizing the constructor, setting updateOnlyOnMove to true will not update the fog texture immediately
                int index = fogWar.AddFogRevealer(new csFogWar.FogRevealer(randomTransform, 3, false));
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (fogWar._FogRevealers.Count > 2)
                {
                    fogWar.RemoveFogRevealer(fogWar._FogRevealers.Count - 1);
                }
            }
        }
    }



}