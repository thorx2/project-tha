using Unity.VisualScripting;
using UnityEngine;

namespace ProjTha
{
    //TODO Another ugly class which would need rewrite
    public class ChunkTrigger : MonoBehaviour
    {
        [SerializeField]
        private MapController mapControllerRef;

        public Transform topAnchor;
        public Transform leftAnchor;
        public Transform rightAnchor;
        public Transform bottomAnchor;
        public Transform topLeftAnchor;
        public Transform topRightAnchor;
        public Transform bottomLeftAnchor;
        public Transform bottomRightAnchor;

        protected void Start()
        {
            mapControllerRef = FindObjectOfType<MapController>();
        }

        protected void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                mapControllerRef.CurrentChunk = this;
            }
        }

        protected void OnTriggerExit2D(Collider2D other)
        {
            if (mapControllerRef.CurrentChunk == this)
            {
                mapControllerRef.CurrentChunk = null;
            }
        }
    }
}