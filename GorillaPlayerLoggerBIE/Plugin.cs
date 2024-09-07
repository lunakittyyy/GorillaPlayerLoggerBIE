using BepInEx;
using PlayFab.ClientModels;
using PlayFab;
using System;
using UnityEngine;
using Utilla;
using HarmonyLib;
using System.Threading.Tasks;
using UnityEngine.LowLevel;
using GorillaPlayerLoggerBIE.Utils;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Photon.Realtime;

namespace GorillaPlayerLoggerBIE
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {

        void Start()
        {
            HarmonyPatches.ApplyHarmonyPatches();
        }
        public static void CreatePlayerAndUpdateDisplayName(string playFabId, Action<GetAccountInfoResult> result, Action<PlayFabError> error)
        {
            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest()
            {
                PlayFabId = playFabId,
            },
            result,
            error);
        }


        [HarmonyPatch(typeof(VRRig), "OnEnable")]
        public static class RigInitPatch
        {
            public static async void Postfix(VRRig __instance)
            {
                if (__instance.isOfflineVRRig) return;

                await Task.Delay(5000);

                var id = __instance.OwningNetPlayer.UserId;
                var name = __instance.OwningNetPlayer.NickName;
                var cosmetics = __instance.concatStringOfCosmeticsAllowed;

                Plugin.CreatePlayerAndUpdateDisplayName(id, (GetAccountInfoResult result) =>
                {
#if !NOTABIRD
                    __instance.playerText.text += $"\n{result.AccountInfo.Created.ToShortDateString()}";
#endif
                    var player = new LoggedPlayer
                    {
                        PhotonId = __instance.OwningNetPlayer.UserId,
                        UserName = StringTool.NormalizeName(__instance.OwningNetPlayer.NickName),
                        AccountCreationDate = result.AccountInfo.Created,
                        PlayerProps = __instance.OwningNetPlayer.GetPlayerRef().CustomProperties,
                        AllowedCosmetics = __instance.concatStringOfCosmeticsAllowed.Split('.')
                    };
                    var filePath = $"{BepInEx.Paths.GameRootPath}\\PlayerDB\\{StringTool.NormalizeName(name)}-{id}.json";

                    using (FileStream fs = File.Create(filePath))
                    {
                        byte[] json = new UTF8Encoding(true).GetBytes(JsonConvert.SerializeObject(player));
                        fs.Write(json, 0, json.Length);
                    }
                },
                (PlayFabError error) =>
                {
                    Debug.LogWarning("Failed to get join date for player");
                });
            }
        }
    }
}
