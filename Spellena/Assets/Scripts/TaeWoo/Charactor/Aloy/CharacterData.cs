using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObject/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        [Header("ĳ���� ������")]
        public float sightAngle;
        public float sightDistance;
        public float moveSpeed;
        public float rotateSpeed;
        public float avoidSpeed;
        public float avoidTiming;

        public float[] coolTimes;
    }
}