using System.Collections.Generic;

namespace ACCess.Model
{
    public class FavouriteSettings
    {
        public List<SavedServer> FavouriteServers { get; set; }

        public FavouriteSettings() 
        {
            FavouriteServers = new List<SavedServer>();
        }
    }
}
