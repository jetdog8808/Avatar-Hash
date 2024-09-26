
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using TMPro;
using VRC.SDK3.Data;

namespace JetDog.AvatarHash
{
    public class AvatarHashExample : UdonSharpBehaviour
    {
        [SerializeField]
        private TextMeshPro debugLimbValues;
        [SerializeField]
        private TMP_InputField avatarHashOutput;
        [SerializeField]
        private TextMeshPro avatarName;

        [SerializeField]
        private TextAsset localAvatarNameDatabase;
        private DataDictionary avatarNameDictionary;

        public void Start()
        {
            if (localAvatarNameDatabase != null)
            {
                //example of loading a list of avatar hashes into a data dictionary
                if (VRCJson.TryDeserializeFromJson(localAvatarNameDatabase.text, out DataToken jsonDeserialized))
                {
                    avatarNameDictionary = jsonDeserialized.DataDictionary;
                    Object.Destroy(localAvatarNameDatabase);
                }
            }
        }

        public override void OnAvatarChanged(VRCPlayerApi player)
        {
            if (!player.isLocal) return;
            //wait a few frames so eye height gets updated after avatar changed.
            SendCustomEventDelayedFrames(nameof(DelayChange), 3, VRC.Udon.Common.Enums.EventTiming.Update);
        }

        public void DelayChange()
        {
            //Geting avatar hash example
            string avatarHash = AvatarHash.GetAvatarHash(Networking.LocalPlayer);

            if (avatarHashOutput != null) avatarHashOutput.text = avatarHash;

            if (avatarName != null)
            {
                //check if avatar hash is in database, and display its name if so.
                if (avatarNameDictionary != null && avatarNameDictionary.TryGetValue(avatarHash, TokenType.String, out DataToken avatarNameOut))
                {
                    avatarName.text = avatarNameOut.String;
                }
                else
                {
                    avatarName.text = string.Empty;
                }

            }
            //Getting limb debug values example.
            if (debugLimbValues != null) debugLimbValues.text = AvatarHash.GetAvatarValuesDebug(Networking.LocalPlayer);
        }

        


    }
}