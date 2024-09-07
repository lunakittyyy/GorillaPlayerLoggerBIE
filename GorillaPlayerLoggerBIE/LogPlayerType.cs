using ExitGames.Client.Photon;
using System;

namespace GorillaPlayerLoggerBIE
{
    public class LoggedPlayer
    {
        public string? PhotonId;
        public string? UserName;
        public DateTime? AccountCreationDate;
        public Hashtable? PlayerProps;
        public string[]? AllowedCosmetics;
    }
}
