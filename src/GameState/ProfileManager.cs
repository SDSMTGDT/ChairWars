using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChairWars.Players;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;

namespace ChairWars.src.GameState
{
    public class ProfileManager
    {
        private List<PlayerProfile> profiles;
        public ReadOnlyCollection<PlayerProfile> PlayerProfiles
        {
            get
            {
                return profiles.AsReadOnly();
            }
        }

        private List<PlayerProfile> loadedProfiles;
        public ReadOnlyCollection<PlayerProfile> LoadedProfiles
        {
            get
            {
                return loadedProfiles.AsReadOnly(); 
            }   
        }

        public ProfileManager()
        {
            profiles = new List<PlayerProfile>();
            loadedProfiles = new List<PlayerProfile>(4);
        }

        public ProfileManager(params string[] playerProfileNames) : this()
        {

        }

        public ProfileManager(string fileName) : this()
        {

        }

        public bool LoadProfile(string profileName, int slot = -1)
        {
               

            return false;
        }

        public bool UnloadProfile(PlayerIndex id)
        {
            if (loadedProfiles[(int)id] != null)
            {
                loadedProfiles[(int)id] = null;
                
                return true;
            }

            return false;
        }

        public PlayerProfile GetPlayerProfile(PlayerIndex id)
        {
            if(id >= PlayerIndex.One && id <= PlayerIndex.Four)             
                return default(PlayerProfile);

            var potentialPlayer = loadedProfiles[(int)id];
            if (null != potentialPlayer)
                return potentialPlayer;
            else
                return default(PlayerProfile);
        }

    }
}
