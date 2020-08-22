using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public class Location
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Item ItemRequiredToEnter { get; set; }
        public Quest QuestAvailableHere { get; set; }
        public Munster MunsterLivingHere { get; set; }
        public Location LocationToTheNorth { get; set; }
        public Location LocationToTheEast { get; set; }
        public Location LocationToTheSouth { get; set; }
        public Location LocationToTheWest { get; set; }

        //Constructor
        public Location(int id, string name, string description,
        Item itemRequiredToEnter = null,
            Quest questAvailableHere = null,
                Munster munsterLivingHere = null)
        {
            ID = id;
            Name = name;
            Description = description;
            Item ItemRequiredToEnter = itemRequiredToEnter;
            Quest QuestAvailableHere = questAvailableHere;
            Munster MunsterLivingHere = munsterLivingHere;
        }

    }
}
