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
        ///Get unique string to identify local avatar.
        ///</summary
        ///<param name="player">Player reference.</param>
        ///<returns>Hash as a string.</returns>
        public static string GetAvatarHash()
        {
            short[] hashData = GetHashData();
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
        public static string GetAvatarValuesDebug()
        {
            short[] hashData = GetHashData();

            StringBuilder stringBuilder = new StringBuilder();
            foreach (short value in hashData)
            {
                stringBuilder.Append(value);
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }        

        private static short[] GetHashData()
        {
            VRCPlayerApi player = Networking.LocalPlayer;
            short[] hashData = new short[9];
            float scale = Vector3.Distance(player.GetBonePosition(HumanBodyBones.Spine), player.GetBonePosition(HumanBodyBones.Chest));
            
            if (Mathf.Approximately(scale, 0f)) return hashData;

            hashData[0] = LimbDistance(player, HumanBodyBones.Neck, HumanBodyBones.Head, scale, false);
            hashData[1] = LimbDistance(player, HumanBodyBones.Hips, HumanBodyBones.RightUpperLeg, scale);
            hashData[2] = LimbDistance(player, HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg, scale);
            hashData[3] = LimbDistance(player, HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot, scale);
            hashData[4] = LimbDistance(player, HumanBodyBones.RightFoot, HumanBodyBones.RightToes, scale);
            hashData[5] = LimbDistance(player, HumanBodyBones.RightShoulder, HumanBodyBones.RightUpperArm, scale);
            hashData[6] = LimbDistance(player, HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm, scale);
            hashData[7] = LimbDistance(player, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand, scale);
            hashData[8] = LimbDistance(player, HumanBodyBones.RightHand, HumanBodyBones.RightThumbProximal, scale);

            return hashData;
        }

        //only right side bones should be used
        private static short LimbDistance(VRCPlayerApi player, HumanBodyBones bone1, HumanBodyBones bone2, float Scale, bool symetrical = true)
        {
            float decimalpoint = 1000f;

            Vector3 point1 = player.GetBonePosition(bone1);
            Vector3 point2 = player.GetBonePosition(bone2);
            if (point1 == Vector3.zero || point2 == Vector3.zero) return 0;
            short rShort = (short)(Mathf.FloorToInt(Vector3.Distance(point1, point2) / Scale * decimalpoint) & 0x7FFF);

            if (symetrical)
            {
                //get opposite side
                point1 = player.GetBonePosition(BoneOtherSide(bone1));
                point2 = player.GetBonePosition(BoneOtherSide(bone2));
                if (point1 == Vector3.zero || point2 == Vector3.zero) return 0;
                short lShort = (short)(Mathf.FloorToInt(Vector3.Distance(point1, point2) / Scale * decimalpoint) & 0x7FFF);

                //check if avatar is not symetrical
                if (rShort != lShort)
                {
                    //invert if  not symetrical
                    rShort *= -1;
                }
            }

            return rShort;
        }

        private static HumanBodyBones BoneOtherSide(HumanBodyBones bone)
        {
            switch (bone)
            {
                case HumanBodyBones.Hips:
                    return HumanBodyBones.Hips;
                case HumanBodyBones.LeftUpperLeg:
                    return HumanBodyBones.RightUpperLeg;
                case HumanBodyBones.RightUpperLeg:
                    return HumanBodyBones.LeftUpperLeg;
                case HumanBodyBones.LeftLowerLeg:
                    return HumanBodyBones.RightLowerLeg;
                case HumanBodyBones.RightLowerLeg:
                    return HumanBodyBones.LeftLowerLeg;
                case HumanBodyBones.LeftFoot:
                    return HumanBodyBones.RightFoot;
                case HumanBodyBones.RightFoot:
                    return HumanBodyBones.LeftFoot;
                case HumanBodyBones.Spine:
                    return HumanBodyBones.Spine;
                case HumanBodyBones.Chest:
                    return HumanBodyBones.Chest;
                case HumanBodyBones.UpperChest:
                    return HumanBodyBones.UpperChest;
                case HumanBodyBones.Neck:
                    return HumanBodyBones.Neck;
                case HumanBodyBones.Head:
                    return HumanBodyBones.Head;
                case HumanBodyBones.LeftShoulder:
                    return HumanBodyBones.RightShoulder;
                case HumanBodyBones.RightShoulder:
                    return HumanBodyBones.LeftShoulder;
                case HumanBodyBones.LeftUpperArm:
                    return HumanBodyBones.RightUpperArm;
                case HumanBodyBones.RightUpperArm:
                    return HumanBodyBones.LeftUpperArm;
                case HumanBodyBones.LeftLowerArm:
                    return HumanBodyBones.RightLowerArm;
                case HumanBodyBones.RightLowerArm:
                    return HumanBodyBones.LeftLowerArm;
                case HumanBodyBones.LeftHand:
                    return HumanBodyBones.RightHand;
                case HumanBodyBones.RightHand:
                    return HumanBodyBones.LeftHand;
                case HumanBodyBones.LeftToes:
                    return HumanBodyBones.RightToes;
                case HumanBodyBones.RightToes:
                    return HumanBodyBones.LeftToes;
                case HumanBodyBones.LeftEye:
                    return HumanBodyBones.RightEye;
                case HumanBodyBones.RightEye:
                    return HumanBodyBones.LeftEye;
                case HumanBodyBones.Jaw:
                    return HumanBodyBones.Jaw;
                case HumanBodyBones.LeftThumbProximal:
                    return HumanBodyBones.RightThumbProximal;
                case HumanBodyBones.LeftThumbIntermediate:
                    return HumanBodyBones.RightThumbIntermediate;
                case HumanBodyBones.LeftThumbDistal:
                    return HumanBodyBones.RightThumbDistal;
                case HumanBodyBones.LeftIndexProximal:
                    return HumanBodyBones.RightIndexProximal;
                case HumanBodyBones.LeftIndexIntermediate:
                    return HumanBodyBones.RightIndexIntermediate;
                case HumanBodyBones.LeftIndexDistal:
                    return HumanBodyBones.RightIndexDistal;
                case HumanBodyBones.LeftMiddleProximal:
                    return HumanBodyBones.RightMiddleProximal;
                case HumanBodyBones.LeftMiddleIntermediate:
                    return HumanBodyBones.RightMiddleIntermediate;
                case HumanBodyBones.LeftMiddleDistal:
                    return HumanBodyBones.RightMiddleDistal;
                case HumanBodyBones.LeftRingProximal:
                    return HumanBodyBones.RightRingProximal;
                case HumanBodyBones.LeftRingIntermediate:
                    return HumanBodyBones.RightRingIntermediate;
                case HumanBodyBones.LeftRingDistal:
                    return HumanBodyBones.RightRingDistal;
                case HumanBodyBones.LeftLittleProximal:
                    return HumanBodyBones.RightLittleProximal;
                case HumanBodyBones.LeftLittleIntermediate:
                    return HumanBodyBones.RightLittleIntermediate;
                case HumanBodyBones.LeftLittleDistal:
                    return HumanBodyBones.RightLittleDistal;
                case HumanBodyBones.RightThumbProximal:
                    return HumanBodyBones.LeftThumbProximal;
                case HumanBodyBones.RightThumbIntermediate:
                    return HumanBodyBones.LeftThumbIntermediate;
                case HumanBodyBones.RightThumbDistal:
                    return HumanBodyBones.LeftThumbDistal;
                case HumanBodyBones.RightIndexProximal:
                    return HumanBodyBones.LeftIndexProximal;
                case HumanBodyBones.RightIndexIntermediate:
                    return HumanBodyBones.LeftIndexIntermediate;
                case HumanBodyBones.RightIndexDistal:
                    return HumanBodyBones.LeftIndexDistal;
                case HumanBodyBones.RightMiddleProximal:
                    return HumanBodyBones.LeftMiddleProximal;
                case HumanBodyBones.RightMiddleIntermediate:
                    return HumanBodyBones.LeftMiddleIntermediate;
                case HumanBodyBones.RightMiddleDistal:
                    return HumanBodyBones.LeftMiddleDistal;
                case HumanBodyBones.RightRingProximal:
                    return HumanBodyBones.LeftRingProximal;
                case HumanBodyBones.RightRingIntermediate:
                    return HumanBodyBones.LeftRingIntermediate;
                case HumanBodyBones.RightRingDistal:
                    return HumanBodyBones.LeftRingDistal;
                case HumanBodyBones.RightLittleProximal:
                    return HumanBodyBones.LeftLittleProximal;
                case HumanBodyBones.RightLittleIntermediate:
                    return HumanBodyBones.LeftLittleIntermediate;
                case HumanBodyBones.RightLittleDistal:
                    return HumanBodyBones.LeftLittleDistal;
                case HumanBodyBones.LastBone:
                    return HumanBodyBones.LastBone;
                default:
                    return HumanBodyBones.LastBone;
            }
        }

    }
}