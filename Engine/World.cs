using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Engine
{
    public static class World
    {
        public static readonly List<Item> Items = new List<Item>();
        public static readonly List<Munster> Munsters = new List<Munster>();
        public static readonly List<Quest> Quests = new List<Quest>();
        public static readonly List<Location> Locations = new List<Location>();

        public const int ITEM_ID_RUSTY_SWORD = 1;
        public const int ITEM_ID_RAT_TAIL = 2;
        public const int ITEM_ID_PIECE_OF_FUR = 3;
        public const int ITEM_ID_SNAKE_FANG = 4;
        public const int ITEM_ID_SNAKESKIN = 5;
        public const int ITEM_ID_CLUB = 6;
        public const int ITEM_ID_HEALING_POTION = 7;
        public const int ITEM_ID_SPIDER_FANG = 8;
        public const int ITEM_ID_SPIDER_SILK = 9;
        public const int ITEM_ID_ADVENTURERS_PASS = 10;

        public const int MUNSTER_ID_RAT = 1;
        public const int MUNSTER_ID_SNAKE = 2;
        public const int MUNSTER_ID_GIANT_SPIDER = 3;

        public const int QUEST_ID_CLEAR_ALCHEMISTS_GARDEN = 1;
        public const int QUEST_ID_CLEAR_FARMERS_FIELDS = 2;

        public const int LOCATION_ID_HOME = 1;
        public const int LOCATION_ID_TOWN_SQUARE = 2;
        public const int LOCATION_ID_GUARD_POST = 3;
        public const int LOCATION_ID_ALCHEMIST_HUT = 4;
        public const int LOCATION_ID_ALCHEMIST_GARDEN = 5;
        public const int LOCATION_ID_FARMHOUSE = 6;
        public const int LOCATION_ID_FARM_FIELD = 7;
        public const int LOCATION_ID_BRIDGE = 8;
        public const int LOCATION_ID_SPIDER_FIELD = 9;

        static World()
        {
            PopulateItems();
            PopulateMunsters();
            PopulateQuests();
            PopulateLocations();
        }

        private static void PopulateItems()
        {
            Items.Add(new Weapon(ITEM_ID_RUSTY_SWORD, "Rusty Sword", "Rusty Swords",0, 5));
            Items.Add(new Item(ITEM_ID_RAT_TAIL, "Rat Tail", "Rat Tails"));
            Items.Add(new Item(ITEM_ID_PIECE_OF_FUR, "Piece of fur", "Pieces of fur"));
            Items.Add(new Item(ITEM_ID_SNAKE_FANG, "Snake Fang", "Snake Fangs"));
            Items.Add(new Item(ITEM_ID_SNAKESKIN, "Snakeskin", "Snakeskins"));
            Items.Add(new Weapon(ITEM_ID_CLUB, "Club", "Clubs", 3, 10));
            Items.Add(new HealingPotion(ITEM_ID_HEALING_POTION, "Healing Potion", "Healing Potions", 5));
            Items.Add(new Item(ITEM_ID_SPIDER_FANG, "Spider Fang", "Spider Fangs"));
            Items.Add(new Item(ITEM_ID_SPIDER_SILK, "Spider Silk", "Spider Silks"));
            Items.Add(new Item(ITEM_ID_ADVENTURERS_PASS, "Adventurers Pass", "Adventurers Passes"));
        }
        
        private static void PopulateMunsters()
        {
            Munster rat = new Munster(3, 3, MUNSTER_ID_RAT, "Rat", 5, 3, 10);
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_RAT_TAIL), 75, false));
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_PIECE_OF_FUR), 75, true));

            Munster snake = new Munster(3, 3, MUNSTER_ID_SNAKE, "Snake", 3 , 10, 5);
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKE_FANG), 75, false));
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKESKIN), 75, true));

            Munster giantSpider = new Munster(10, 10, MUNSTER_ID_GIANT_SPIDER, "Big Spook", 5, 20, 5);
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_FANG), 75, true));
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_SILK), 25, false));

            Munsters.Add(rat);
            Munsters.Add(snake);
            Munsters.Add(giantSpider);

        }

        private static void PopulateQuests()
        {
            Quest clearAlchemistGarden = new Quest(QUEST_ID_CLEAR_ALCHEMISTS_GARDEN, "Clear the Alchemists' Garden", "Kill rats in the Alchemists' Garden and bring back 3 rat tails. You will receive a healing potion and 10 gold pieces, big boi", 20, 10);
            
            clearAlchemistGarden.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_RAT_TAIL), 3));
            
            clearAlchemistGarden.RewardItem = ItemByID(ITEM_ID_HEALING_POTION);


            Quest clearFarmersField = new Quest(QUEST_ID_CLEAR_FARMERS_FIELDS, "Clear the Farmers Field", "Kill Snakes in the Farmers' Field and bring back 3 Snake Fangs. You will receive an adventurers pass and 20 gold pieces", 20, 20);
            
            clearFarmersField.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_SNAKE_FANG), 3));
            
            clearFarmersField.RewardItem = ItemByID(ITEM_ID_ADVENTURERS_PASS);

            Quests.Add(clearFarmersField);
            Quests.Add(clearAlchemistGarden);

        }

        private static void PopulateLocations()
        {
            //Create Each Location
            Location home = new Location(LOCATION_ID_HOME, "This is Your Home", "Welcome to the hood, this place is rank");

            Location townSquare = new Location(LOCATION_ID_TOWN_SQUARE, "Town Square", "You see some broken down fountain. Where's the Water?");

            Location alchemistHut = new Location(LOCATION_ID_ALCHEMIST_HUT, "Some old guys hut", "Plenty of weird vials and organisms within them. what a quack");
            alchemistHut.QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_ALCHEMISTS_GARDEN);

            Location alchemistGarden = new Location(LOCATION_ID_ALCHEMIST_GARDEN, "The Alchemists' Garden", "Did something just touch my leg?");
            alchemistGarden.MunsterLivingHere = MunsterByID(MUNSTER_ID_RAT);

            Location farmhouse = new Location(LOCATION_ID_FARMHOUSE, "This is a Farmhouse", "plenty of ole piggies ere' yee haw");
            farmhouse.QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_FARMERS_FIELDS);

            Location farmersField = new Location(LOCATION_ID_FARM_FIELD, "This is the Farmers Field. Looks dull.",
                "ah holy shit is that a snake");
            farmersField.MunsterLivingHere = MunsterByID(MUNSTER_ID_SNAKE);

            Location guardPost = new Location(LOCATION_ID_GUARD_POST, "Guard post. Never been one for authority.",
                "Some chad looking fuck over there. Must be the titular 'guard'", ItemByID(ITEM_ID_ADVENTURERS_PASS));

            Location bridge = new Location(LOCATION_ID_BRIDGE, "This is a bridge.", "If this bridge wasnt here, we'd have to swim!");

            Location spiderField = new Location(LOCATION_ID_SPIDER_FIELD, "Oh wow, ok this is a forest filled to the brim with spiders. That is one thing you do not want filled to the brim of.", "Plenty of, uh, spiderwebs here. Can we just leave");
            spiderField.MunsterLivingHere = MunsterByID(MUNSTER_ID_GIANT_SPIDER);

            //Linking the locations together. Yes, very tedious.
            home.LocationToTheNorth = townSquare;

            townSquare.LocationToTheNorth = alchemistHut;
            townSquare.LocationToTheSouth = home;
            townSquare.LocationToTheEast = guardPost;
            townSquare.LocationToTheWest = farmhouse;

            farmhouse.LocationToTheEast = townSquare;
            farmhouse.LocationToTheWest = farmersField;

            farmersField.LocationToTheEast = farmhouse;

            alchemistHut.LocationToTheSouth = townSquare;
            alchemistHut.LocationToTheNorth = alchemistGarden;

            alchemistGarden.LocationToTheSouth = alchemistHut;

            guardPost.LocationToTheEast = bridge;
            guardPost.LocationToTheWest = townSquare;

            bridge.LocationToTheWest = guardPost;
            bridge.LocationToTheEast = spiderField;

            spiderField.LocationToTheWest = bridge;

            //Adding the locations to the static list. pretty boring stuff eh? you still wanna be a programmer? you still wanna make games pussy boy ??
            Locations.Add(home);
            Locations.Add(townSquare);
            Locations.Add(guardPost);
            Locations.Add(alchemistHut);
            Locations.Add(alchemistGarden);
            Locations.Add(farmhouse);
            Locations.Add(farmersField);
            Locations.Add(bridge);
            Locations.Add(spiderField);
        }

        public static Item ItemByID(int id)
        {
            foreach(Item item in Items)
            {
                if (item.ID == id)
                {
                    continue;
                }
                return item;
            }

            return null;
        }

        public static Munster MunsterByID(int id)
        {
            foreach (Munster munster in Munsters)
            {
                if (munster.ID == id)
                {
                    return munster;
                }
            }

            return null;
        }

        public static Quest QuestByID(int id)
        {
            foreach (Quest quest in Quests)
            {
                if (quest.ID == id)
                {
                    return quest;
                }
            }

            return null;
        }

        public static Location LocationByID(int id)
        {
            foreach (Location location in Locations)
            {
                return location;
            }

            return null;
        }
    }
}