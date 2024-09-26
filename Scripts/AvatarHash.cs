using JetBrains.Annotations;
using System;
using System.Text;
using UnityEngine;
using VRC.SDKBase;

namespace JetDog.AvatarHash
{
    public static class AvatarHash
    {
        
        [PublicAPI]
        ///<summary
        ///Get unique string to identify avatar.
        ///</summary
        ///<param name="player">Player reference.</param>
        ///<returns>Hash as a string.</returns>
        public static string GetAvatarHash(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player)) return string.Empty;

            short[] hashData = GetHashData(player);
            byte[] hashBytes = new byte[Buffer.ByteLength(hashData)];
            Buffer.BlockCopy(hashData, 0, hashBytes, 0, hashBytes.Length);
            return Convert.ToBase64String(hashBytes);
        }

        [PublicAPI]
        ///<summary
        ///Get string of limb calculations for Debug.
        ///</summary
        ///<param name="player">Player reference.</param>
        ///<returns>Limb values string</returns>
        public static string GetAvatarValuesDebug(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player)) return string.Empty;

            short[] hashData = GetHashData(player);

            StringBuilder stringBuilder = new StringBuilder();
            foreach (short value in hashData)
            {
                stringBuilder.Append(value);
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }        

        private static short[] GetHashData(VRCPlayerApi player)
        {
            float scale = player.GetAvatarEyeHeightAsMeters();
            short[] hashData = new short[12];

            //fullbody can compress/expand spine so only grab limb bones to measure.
            hashData[0] = LimbDistance(player, HumanBodyBones.Hips, HumanBodyBones.RightUpperLeg, scale);
            hashData[1] = LimbDistance(player, HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg, scale);
            hashData[2] = LimbDistance(player, HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot, scale);
            hashData[3] = LimbDistance(player, HumanBodyBones.RightFoot, HumanBodyBones.RightToes, scale);
            hashData[4] = LimbDistance(player, HumanBodyBones.RightShoulder, HumanBodyBones.RightUpperArm, scale);
            hashData[5] = LimbDistance(player, HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm, scale);
            hashData[6] = LimbDistance(player, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand, scale);
            hashData[7] = LimbDistance(player, HumanBodyBones.RightHand, HumanBodyBones.RightThumbProximal, scale);
            hashData[8] = LimbDistance(player, HumanBodyBones.RightHand, HumanBodyBones.RightIndexProximal, scale);
            hashData[9] = LimbDistance(player, HumanBodyBones.RightHand, HumanBodyBones.RightMiddleProximal, scale);
            hashData[10] = LimbDistance(player, HumanBodyBones.RightHand, HumanBodyBones.RightRingProximal, scale);
            hashData[11] = LimbDistance(player, HumanBodyBones.RightHand, HumanBodyBones.RightLittleProximal, scale);

            return hashData;
        }

        //only right side bones should be used
        private static short LimbDistance(VRCPlayerApi player, HumanBodyBones bone1, HumanBodyBones bone2, float Scale)
        {
            float decimalpoint = 10000f;

            Vector3 point1 = player.GetBonePosition(bone1);
            Vector3 point2 = player.GetBonePosition(bone2);
            if (point1 == Vector3.zero || point2 == Vector3.zero) return 0;
            short rShort = (short)(Mathf.FloorToInt(Vector3.Distance(point1, point2) / Scale * decimalpoint) & 0x7FFF);

            //get opposite side
            point1 = player.GetBonePosition((HumanBodyBones)Mathf.Clamp((((int)bone1) < 23) ? (((int)bone1) - 1) : (((int)bone1) - 15), 0, 55));
            point2 = player.GetBonePosition((HumanBodyBones)Mathf.Clamp((((int)bone2) < 23) ? (((int)bone2) - 1) : (((int)bone2) - 15), 0, 55));
            if (point1 == Vector3.zero || point2 == Vector3.zero) return 0;
            short lShort = (short)(Mathf.FloorToInt(Vector3.Distance(point1, point2) / Scale * decimalpoint) & 0x7FFF);

            //check if avatar is not symetrical
            if (rShort != lShort)
            {
                //invert if  not symetrical
                rShort *= -1;
            }

            return rShort;
        }

    }
}