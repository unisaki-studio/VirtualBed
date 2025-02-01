using UnityEngine;
using VRC.SDKBase;

namespace jp.unisakistudio.virtualbed
{
    public class VirtualBed : MonoBehaviour, IEditorOnly
    {
        [HideInInspector]
        public bool isVirtualLoveBoyLicensed = false;

        [HideInInspector]
        public bool isVirtualLoveGirlLicensed = false;

        [HideInInspector]
        public bool isVirtualBedLicensed = false;

        [HideInInspector]
        public bool isBoy;
    }
}