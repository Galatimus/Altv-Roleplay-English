using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Altv_Roleplay.models
{
    public partial class AccountsCharacters
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int charId { get; set; }
        public int accountId { get; set; }
        public string charname { get; set; }
        public bool death { get; set; }
        public int accState { get; set; }
        public bool firstJoin { get; set; }
        public string firstSpawnPlace { get; set; }
        public DateTime firstJoinTimestamp { get; set; }
        public bool gender { get; set; }
        public string birthdate { get; set; }
        public string birthplace { get; set; }
        public int health { get; set; }
        public int armor { get; set; }
        public int hunger { get; set; }
        public int thirst { get; set; }
        public string address { get; set; }
        public int phonenumber { get; set; }
        public bool isCrime { get; set; }
        public int paydayTime { get; set; }
        public string job { get; set; }
        public int jobHourCounter { get; set; }
        public DateTime lastJobPaycheck { get; set; }
        public string weapon_Primary { get; set; }
        public int weapon_Primary_Ammo { get; set; }
        public string weapon_Secondary { get; set; }
        public int weapon_Secondary_Ammo { get; set; }
        public string weapon_Secondary2 { get; set; }
        public int weapon_Secondary2_Ammo { get; set; }
        public string weapon_Fist { get; set; }
        public int weapon_Fist_Ammo { get; set; }
        public bool isUnconscious { get; set; }
        public int unconsciousTime { get; set; }
        public bool isFastFarm { get; set; }
        public int fastFarmTime { get; set; }
        public DateTime lastLogin { get; set; }
        public bool isPhoneEquipped { get; set; }
        public int playtimeHours { get; set; }
        public bool isInJail { get; set; }
        public int jailTime { get; set; }
        public string pedName { get; set; }
        public int isAnimalPed { get; set; }
        public bool isLaptopEquipped { get; set; }

        [NotMapped]
        public bool isPhoneFlyModeActivated { get; set; } = false; //Flugmodus

        [NotMapped]
        public int targetNumber { get; set; } = 0;

        [NotMapped]
        public int CurrentlyRecieveCaller { get; set; } = 0;

        [NotMapped]
        public string currentFunkFrequence { get; set; } = null;
    }
}
