using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine;
namespace SuperAdventure
{
    public partial class SuperAdventure : Form
    {
        private Player _player;
        private Munster _currentMunster;
        public SuperAdventure()
        {
            InitializeComponent();
            _player = new Player(10, 10, 20, 20, 1);
            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            _player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));

            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text = _player.Level.ToString();
        }
        private void label1_Click(object sender, EventArgs e)
        {
        }
        private void SuperAdventure_Load(object sender, EventArgs e)
        {
        }
        private void btnNorth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToTheNorth);
        }
        private void btnEast_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToTheEast);
        }
        private void btnSouth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToTheSouth);
        }
        private void btnWest_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToTheWest);
        }
        private void MoveTo(Location newLocation)
        {
            //Does the location have any required items
            if (!_player.HasRequiredItemToEnterThisLocation(newLocation))
            {
                rtbMessages.Text += "You must have a " + newLocation.ItemRequiredToEnter.Name + " to enter this location." + Environment.NewLine;
                return;
            }
            //Update the players current location
            _player.CurrentLocation = newLocation;

            //Show/Hide Movement Buttons
            btnNorth.Visible = (newLocation.LocationToTheNorth != null);
            btnEast.Visible = (newLocation.LocationToTheEast != null);
            btnSouth.Visible = (newLocation.LocationToTheSouth != null);
            btnWest.Visible = (newLocation.LocationToTheWest != null);

            //Display current location name and description
            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text += newLocation.Description + Environment.NewLine;

            //Completely Heal the Player
            _player.CurrentHitPoints = _player.MaximumHitPoints;

            //Update Hit Points in the UI
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();

            //Does the location have a quest
            if (newLocation.QuestAvailableHere != null)
            {
                //See if the player already has the quest and if they're completed it
                bool playerAlreadyHasQuest = _player.HasThisQuest(newLocation.QuestAvailableHere);
                bool playerAlreadyCompletedQuest = _player.CompletedThisQuest(newLocation.QuestAvailableHere);
                //See if the player already has the quest
                if (playerAlreadyHasQuest)
                {
                    //If the player has not completed the quest yet
                    if (!playerAlreadyCompletedQuest)
                    {
                        //See if the player has all the items needed to complete the quest
                        bool playerHasAllItemsToCompleteQuest = _player.HasAllQuestCompletionItems(newLocation.QuestAvailableHere);
                        //The player has all items required to complete the quest
                        if (playerHasAllItemsToCompleteQuest)
                        {
                            //Display message
                            rtbMessages.Text += Environment.NewLine;
                            rtbMessages.Text += "You complete the " + newLocation.QuestAvailableHere.Name + " quest." + Environment.NewLine;

                            //Remove Quest items from Inventory
                            _player.RemoveQuestCompletionItems((newLocation.QuestAvailableHere));
                            //Give quest rewards
                            rtbMessages.Text += "You receive: " + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardGold.ToString() + " gold" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
                            rtbMessages.Text += Environment.NewLine;

                            _player.ExperiencePoints += newLocation.QuestAvailableHere.RewardExperiencePoints;
                            _player.Gold += newLocation.QuestAvailableHere.RewardGold;

                            //Add the reward item to the players inventory
                            _player.AddItemToInventory(newLocation.QuestAvailableHere.RewardItem);
                            //Mark the quest as completed
                            _player.MarkQuestAsCompleted((newLocation.QuestAvailableHere));
                        }
                    }
                }
                else
                {
                    //The player does not already have the quest

                    //Display the messages
                    rtbMessages.Text += "You receive the " + newLocation.QuestAvailableHere.Name + " quest." + Environment.NewLine;
                    rtbMessages.Text += newLocation.QuestAvailableHere.Description + Environment.NewLine;
                    rtbMessages.Text += "To complete it, return with: " + Environment.NewLine;
                    foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                    {
                        if (qci.Quantity == 1)
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.Name + Environment.NewLine;
                        }
                        else
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.NamePlural + Environment.NewLine;
                        }
                    }
                    rtbMessages.Text += Environment.NewLine;

                    //Add the quest to the players quest list
                    _player.Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
                }
            }

            //Does the location have a munster?
            if (newLocation.MunsterLivingHere != null)
            {
                rtbMessages.Text += "You see a " + newLocation.MunsterLivingHere.Name + Environment.NewLine;

                //Make a new Munster using the values from the standard munster in the World.Munster list
                Munster standardMunster = World.MunsterByID(newLocation.MunsterLivingHere.ID);

                _currentMunster = new Munster(standardMunster.CurrentHitPoints, standardMunster.MaximumHitPoints, standardMunster.ID, standardMunster.Name, standardMunster.MaximumDamage, standardMunster.RewardExperiencePoints, standardMunster.RewardGold);


                foreach (LootItem lootItem in standardMunster.LootTable)
                {
                    _currentMunster.LootTable.Add(lootItem);
                }

                cboWeapons.Visible = true;
                cboPotions.Visible = true;
                btnUseWeapon.Visible = true;
                btnUsePotion.Visible = true;
            }
            else
            {
                _currentMunster = null;

                cboWeapons.Visible = false;
                cboPotions.Visible = false;
                btnUseWeapon.Visible = false;
                btnUsePotion.Visible = false;
            }

            //Refresh players inventory list
            UpdateInventoryListInUI();
            //Refresh players quest list
            UpdateQuestListInUI();
            //Refresh players weapons combobox
            UpdateWeaponListInUI();
            //Refresh players potions combobox
            UpdatePotionListInUI();
        }

        //Update Inventory List in UI
        private void UpdateInventoryListInUI()
        {
            dgvInventory.RowHeadersVisible = false;

            dgvInventory.ColumnCount = 2;
            dgvInventory.Columns[0].Name = "Name";
            dgvInventory.Columns[0].Width = 197;
            dgvInventory.Columns[1].Name = "Quantity";

            dgvInventory.Rows.Clear();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Quantity > 0)
                {
                    dgvInventory.Rows.Add(new[] {inventoryItem.Details.Name, inventoryItem.Details.Name.ToString()});
                }
            }
        }

        //Update Quest List in UI
        private void UpdateQuestListInUI()
        {
            dgvQuests.RowHeadersVisible = false;

            dgvQuests.ColumnCount = 2;
            dgvQuests.Columns[0].Name = "Name";
            dgvQuests.Columns[0].Width = 197;
            dgvQuests.Columns[1].Name = "Done?";

            dgvQuests.Rows.Clear();

            foreach (PlayerQuest playerQuest in _player.Quests)
            {
                dgvQuests.Rows.Add(new[] {playerQuest.Details.Name, playerQuest.IsCompleted.ToString()});
            }
        }
        //Update Weapon List in UI
        private void UpdateWeaponListInUI()
        {
            List<Weapon> weapons = new List<Weapon>();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Details is Weapon)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        weapons.Add((Weapon) inventoryItem.Details);
                    }
                }
            }

            if (weapons.Count == 0)
            {
                //The player doesnt have any weapons so hide the combobox and use button in the UI
                cboWeapons.Visible = false;
                btnUseWeapon.Visible = false;
            }
            else
            {
                cboWeapons.DataSource = weapons;
                cboWeapons.DisplayMember = "Name";
                cboWeapons.ValueMember = "ID";

                cboWeapons.SelectedIndex = 0;
            }
        }

        //Update Potion List in UI
        private void UpdatePotionListInUI()
        {
            List<HealingPotion> healingPotions = new List<HealingPotion>();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Details is HealingPotion)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        healingPotions.Add((HealingPotion)inventoryItem.Details);
                    }
                }
            }

            if (healingPotions.Count == 0)
            {
                //The player doesnt have any healing potions so hide the combobox and use button in the UI
                cboPotions.Visible = false;
                btnUsePotion.Visible = false;
            }
            else
            {
                cboPotions.DataSource = healingPotions;
                cboPotions.DisplayMember = "Name";
                cboPotions.ValueMember = "ID";

                cboPotions.SelectedIndex = 0;
            }
        }
        private void btnUseWeapon_Click(object sender, EventArgs e)
        {
            //Get the currently selected weapon from the cboWeapons combobox
            Weapon currentWeapon = (Weapon) cboWeapons.SelectedItem;

            //Determine the amount of damage to do to a monster
            int damageToMunster = Engine.RandomNumberGenerator.NumberBetween(currentWeapon.MinimumDamage, currentWeapon.MaximumDamage);

            //Apply the damage to the current munsters hitpoints
            _currentMunster.CurrentHitPoints -= damageToMunster;

            //Display message
            rtbMessages.Text += "You hit the " + _currentMunster.Name + " for " + damageToMunster.ToString() + " points." + Environment.NewLine;

            //Check if the munster is dead
            if (_currentMunster.CurrentHitPoints <= 0)
            {
                //Munster is dead
                rtbMessages.Text += Environment.NewLine;
                rtbMessages.Text += "You defeated the " + _currentMunster.Name + Environment.NewLine;

                //Give the player experience points for killing the munster
                _player.ExperiencePoints += _currentMunster.RewardExperiencePoints;
                rtbMessages.Text += "You receive " + _currentMunster.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine;
                //Give the player gold for killing the munster
                _player.Gold += _currentMunster.RewardGold;
                rtbMessages.Text += "You receive " + _currentMunster.RewardGold.ToString() + " gold." + Environment.NewLine;

                //Get random loot items from the munster
                List<InventoryItem> lootedItems = new List<InventoryItem>();

                //Add items to the lootedItems list, comparing a random number to the drop percentage
                foreach (LootItem lootItem in _currentMunster.LootTable)
                {
                    if (Engine.RandomNumberGenerator.NumberBetween(1, 100) <= lootItem.DropPercentage)
                    {
                        lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                    }
                }
                //If no items were randomly selected, then add the default loot items
                if (lootedItems.Count == 0)
                {
                    foreach (LootItem lootItem in _currentMunster.LootTable)
                    {
                        if (lootItem.IsDefaultItem)
                        {
                            lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                        }
                    }
                }

                //Add the looted items to the players inventory
                foreach (InventoryItem inventoryItem in lootedItems)
                {
                    _player.AddItemToInventory(inventoryItem.Details);

                    if (inventoryItem.Quantity == 1)
                    {
                        rtbMessages.Text += "You loot " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.Name + Environment.NewLine;
                    }
                    else
                    {
                        rtbMessages.Text += "You loot " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.NamePlural + Environment.NewLine;
                    }
                }
                //Refresh player information and inventory controls
                lblHitPoints.Text = _player.CurrentHitPoints.ToString();
                lblGold.Text = _player.Gold.ToString();
                lblExperience.Text = _player.ExperiencePoints.ToString();
                lblLevel.Text = _player.Level.ToString();

                UpdateInventoryListInUI();
                UpdateWeaponListInUI();
                UpdatePotionListInUI();

                //Add a blank line to the messages box, just for appearance
                rtbMessages.Text = Environment.NewLine;

                //Move the player to current location (to heal the player and create a new munster to fight)
                MoveTo(_player.CurrentLocation);
            }
            else
            {
                //Munster is still alive, the prick

                //Determine the amount of damage the munster does to the player
                int damageToPlayer = Engine.RandomNumberGenerator.NumberBetween(0, _currentMunster.MaximumDamage);

                //Display message
                rtbMessages.Text += "The " + _currentMunster.Name + " did " + damageToPlayer.ToString() +
                                    " points of damage." + Environment.NewLine;

                //Subtract damage from player
                _player.CurrentHitPoints -= damageToPlayer;

                //Refresh player data in the UI
                lblHitPoints.Text = _player.CurrentHitPoints.ToString();

                if (_player.CurrentHitPoints <= 0)
                {
                    //Display message
                    rtbMessages.Text += "The " + _currentMunster.Name + " killed yo ass. Get good." + Environment.NewLine;

                    //Move the player to home
                    MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
                }
            }
        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {
            //Get the currently selected potion from the combobox
            HealingPotion potion = (HealingPotion) cboPotions.SelectedItem;

            //Add healing amount to the players hitpoints
            _player.CurrentHitPoints = (_player.CurrentHitPoints + potion.AmountToHeal);

            //Current hit points cannot exceed players maximum hit points
            if (_player.CurrentHitPoints > _player.MaximumHitPoints)
            {
                _player.CurrentHitPoints = _player.MaximumHitPoints;
            }

            //Remove the potion from the players inventory
            foreach (InventoryItem ii in _player.Inventory)
            {
                if (ii.Details.ID == potion.ID)
                {
                    ii.Quantity--;
                    break;
                }
            }

            //Display Message
            rtbMessages.Text += "You drank a " + potion.Name + Environment.NewLine;

            //Monster gets their turn to attack

            //Determine the amount of damage done to the player
            int damageToPlayer = Engine.RandomNumberGenerator.NumberBetween(0, _currentMunster.MaximumDamage);

            //Display message
            rtbMessages.Text += "The " + _currentMunster.Name + " did " + damageToPlayer.ToString() + " points of damage." + Environment.NewLine;

            //Subtract the damage done from the player
            _player.CurrentHitPoints -= damageToPlayer;

            if (_player.CurrentHitPoints <= 0)
            {
                //Display message
                rtbMessages.Text += "The " + _currentMunster.Name + " fucked you up. Welcome to the proletariat." + Environment.NewLine;

                //Move player to "Home"
                MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            }
            //Refresh player data in the UI
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
            UpdateInventoryListInUI();
            UpdatePotionListInUI();
        }
        
    }
}
