using System;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Scripts.States
{
    [CreateAssetMenu(fileName = "ObjectsToSpawn", menuName = "Scriptable Objects/ObjectsToSpawn")]
    public class ObjectsToSpawn : ScriptableObject
    {
        public List<ObjTransformInfo> transforms;
    }
    
    [Serializable]
    public class ObjTransformInfo
    {
        public GameObject gameObject;
        public Vector3 position;
        public Quaternion rotation;
    }
}