using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Lean.Pool;
using Unity.VisualScripting;
using UnityEngine;


namespace ProjTha
{
    //TODO: Ugly class needs refactoring
    public class MapController : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> terrainChunks;

        [SerializeField]
        private float loadRadius;

        [SerializeField]
        private LayerMask terrainMask;
        private Movement playerMovementRef;
        private Vector3 checkTerrainPosition;
        private readonly Collider2D[] hitResults = new Collider2D[50];
        public ChunkTrigger CurrentChunk;

        protected void Update()
        {
            MapUpdateScan();
        }

        public void SetPlayerMovementRef(Movement playerMovementRef)
        {
            this.playerMovementRef = playerMovementRef;
            checkTerrainPosition = playerMovementRef.transform.position;
            SpawnChunk();
        }

        private void MapUpdateScan()
        {
            if (CurrentChunk == null || playerMovementRef == null)
            {
                return;
            }

            if (playerMovementRef.GetInputValue.x > 0 && playerMovementRef.GetInputValue.y > 0)
            {
                checkTerrainPosition = CurrentChunk.topRightAnchor.position;
            }
            else if (playerMovementRef.GetInputValue.x < 0 && playerMovementRef.GetInputValue.y > 0)
            {
                checkTerrainPosition = CurrentChunk.topLeftAnchor.position;
            }
            else if (playerMovementRef.GetInputValue.x < 0 && playerMovementRef.GetInputValue.y < 0)
            {
                checkTerrainPosition = CurrentChunk.bottomLeftAnchor.position;
            }
            else if (playerMovementRef.GetInputValue.x > 0 && playerMovementRef.GetInputValue.y < 0)
            {
                checkTerrainPosition = CurrentChunk.bottomRightAnchor.position;
            }
            else if (playerMovementRef.GetInputValue.x > 0 && playerMovementRef.GetInputValue.y == 0)
            {
                checkTerrainPosition = CurrentChunk.rightAnchor.position;
            }
            else if (playerMovementRef.GetInputValue.x < 0 && playerMovementRef.GetInputValue.y == 0)
            {
                checkTerrainPosition = CurrentChunk.leftAnchor.position;
            }
            else if (playerMovementRef.GetInputValue.x == 0 && playerMovementRef.GetInputValue.y > 0)
            {
                checkTerrainPosition = CurrentChunk.topAnchor.position;
            }
            else if (playerMovementRef.GetInputValue.x == 0 && playerMovementRef.GetInputValue.y < 0)
            {
                checkTerrainPosition = CurrentChunk.bottomAnchor.position;
            }

            if (playerMovementRef.GetInputValue.x != 0 || playerMovementRef.GetInputValue.y != 0)
            {
                var count = Physics2D.OverlapCircleNonAlloc(checkTerrainPosition, loadRadius, hitResults, terrainMask);
                if (count == 0)
                {
                    SpawnChunk();
                }
            }
        }



        private void SpawnChunk()
        {
            int rand = Random.Range(0, terrainChunks.Count);
            LeanPool.Spawn(terrainChunks[rand], checkTerrainPosition, Quaternion.identity);
        }
    }
}