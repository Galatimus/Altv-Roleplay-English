using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Factories;
using Altv_Roleplay.Handler;
using Altv_Roleplay.models;
using Altv_Roleplay.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Altv_Roleplay.Model
{
    class Characters : IScript
    {
        public static List<AccountsCharacters> PlayerCharacters = new List<AccountsCharacters>();
        public static List<Characters_Skin> CharactersSkin = new List<Characters_Skin>();
        public static List<Characters_LastPos> CharactersLastPos = new List<Characters_LastPos>();
        public static List<Characters_Permissions> CharactersPermissions = new List<Characters_Permissions>();

        public static void CreatePlayerCharacter(IPlayer client, string charname, string birthdate, bool gender, string facefeaturesarray, string headblendsdataarray, string headoverlaysarray)
        {
            try
            {
                if (client == null || !client.Exists) return;
                var CharData = new AccountsCharacters
                {
                    accountId = User.GetPlayerAccountId(client),
                    charname = charname,
                    death = false,
                    accState = 0,
                    firstJoin = true,
                    firstSpawnPlace = "unset",
                    firstJoinTimestamp = DateTime.Now,
                    gender = gender,
                    birthdate = birthdate,
                    birthplace = "None",
                    health = 100,
                    armor = 0,
                    hunger = 100,
                    thirst = 100,
                    address = "Obdachlos",
                    phonenumber = 0,
                    isCrime = false,
                    paydayTime = 0,
                    job = "None",
                    jobHourCounter = 0,
                    lastJobPaycheck = DateTime.Now,
                    weapon_Primary = "None",
                    weapon_Primary_Ammo = 0,
                    weapon_Secondary = "None",
                    weapon_Secondary_Ammo = 0,
                    weapon_Secondary2 = "None",
                    weapon_Secondary2_Ammo = 0,
                    weapon_Fist = "None",
                    weapon_Fist_Ammo = 0,
                    isUnconscious = false,
                    unconsciousTime = 0,
                    isFastFarm = false,
                    fastFarmTime = 0,
                    lastLogin = DateTime.Now,
                    isPhoneEquipped = false,
                    playtimeHours = 0,
                    isInJail = false,
                    jailTime = 0,
                    pedName = "none",
                    isAnimalPed = 0
                };
                PlayerCharacters.Add(CharData);

                using (gtaContext db = new gtaContext())
                {
                    db.AccountsCharacters.Add(CharData);
                    db.SaveChanges();
                }

                GenerateCharacterPhonenumber(client, CharData.charId);

                CreateCharacterSkin(charname, facefeaturesarray, headblendsdataarray, headoverlaysarray);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void CreateCharacterSkin(string charname, string facefeaturesarray, string headblendsdataarray, string headoverlaysarray)
        {
            int charId = GetCharacterIdFromCharName(charname);
            var CharSkinData = new Characters_Skin
            {
                charId = charId,
                facefeatures = facefeaturesarray,
                headblendsdata = headblendsdataarray,
                headoverlays = headoverlaysarray,
                clothesTop = -2,
                clothesTorso = -2,
                clothesLeg = -2,
                clothesFeet = -2,
                clothesHat = -2,
                clothesGlass = -2,
                clothesEarring = -2,
                clothesNecklace = -2,
                clothesMask = -2,
                clothesArmor = -2,
                clothesUndershirt = -2,
                clothesBracelet = -2,
                clothesWatch = -2,
                clothesBag = -2,
                clothesDecal = -2
            };

            CharactersLicenses.CreateCharacterLicensesEntry(charId, false, false, false, false, false, false, false, false);
            CharactersTablet.CreateCharacterTabletAppEntry(charId, false, false, false, false, false, false, false, false);
            CharactersTablet.CreateCharacterTabletTutorialAppEntry(charId);

            try
            {
                CharactersSkin.Add(CharSkinData);
                using (gtaContext db = new gtaContext())
                {
                    db.Characters_Skin.Add(CharSkinData);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void SetCharacterCurrentFunkFrequence(int charId, string frequence)
        {
            try
            {
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars == null) return;
                chars.currentFunkFrequence = frequence;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static bool IsCharacterInJail(int charId)
        {
            try
            {
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars != null) return chars.isInJail;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static int GetCharacterJailTime(int charId)
        {
            try
            {
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars != null) return chars.jailTime;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static void SetCharacterJailTime(int charId, bool isInJail, int jt)
        {
            try
            {
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars == null) return;
                if (jt < 0) jt = 0;
                chars.isInJail = isInJail;
                chars.jailTime = jt;
                using (var db = new gtaContext())
                {
                    db.AccountsCharacters.Update(chars);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static string GetCharacterPedName(int charId)
        {
            try
            {
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars != null) return chars.pedName;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return "none";
        }

        public static bool IsCharacterAnimal(int charId)
        {
            try
            {
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars != null) return Convert.ToBoolean(chars.isAnimalPed);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static string GetCharacterCurrentFunkFrequence(int charId)
        {
            try
            {
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars != null) return chars.currentFunkFrequence;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return null;
        }

        public static int GetCharacterAccountId(int charId)
        {
            try
            {
                if (charId <= 0) return 0;
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars != null) return chars.accountId;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static int GetCharacterCurrentlyRecieveCaller(int charId)
        {
            try
            {
                var chars = PlayerCharacters.ToList().FirstOrDefault(x => x.charId == charId);
                if (chars != null) return chars.CurrentlyRecieveCaller;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static void SetCharacterCurrentlyRecieveCallState(int charId, int CurrentlyRecieveCaller)
        {
            try
            {
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars != null) chars.CurrentlyRecieveCaller = CurrentlyRecieveCaller;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static int GetCharacterPhoneTargetNumber(int charId)
        {
            try
            {
                var chars = PlayerCharacters.ToList().FirstOrDefault(x => x.charId == charId);
                if (chars != null) return chars.targetNumber;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static void SetCharacterTargetPhoneNumber(int charId, int targetNumber)
        {
            try
            {
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars != null) chars.targetNumber = targetNumber;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static bool IsCharacterPhoneFlyModeEnabled(int charId)
        {
            try
            {
                var chars = PlayerCharacters.ToList().FirstOrDefault(x => x.charId == charId);
                if (chars != null) return chars.isPhoneFlyModeActivated;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static void SetCharacterPhoneFlyModeEnabled(int charId, bool isEnabled)
        {
            try
            {
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars != null) chars.isPhoneFlyModeActivated = isEnabled;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static bool IsCharacterPhoneEquipped(int charId)
        {
            try
            {
                var chars = PlayerCharacters.ToList().FirstOrDefault(x => x.charId == charId);
                if (chars != null) return chars.isPhoneEquipped;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }
        
        public static bool IsCharacterLaptopEquipped(int charId)
        {
            try
            {
                var chars = PlayerCharacters.ToList().FirstOrDefault(x => x.charId == charId);
                if (chars != null) return chars.isLaptopEquipped;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static void SetCharacterPhoneEquipped(int charId, bool isPhoneEquipped)
        {
            try
            {
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars == null) return;
                chars.isPhoneEquipped = isPhoneEquipped;
                using (var db = new gtaContext())
                {
                    db.AccountsCharacters.Update(chars);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
        public static void SetCharacterLaptopEquipped(int charId, bool isLaptopEquipped)
        {
            try
            {
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars == null) return;
                chars.isLaptopEquipped = isLaptopEquipped;
                using (var db = new gtaContext())
                {
                    db.AccountsCharacters.Update(chars);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void SetCharacterHeadOverlays(int charid, string headoverlays)
        {
            if (charid == 0 || headoverlays == "") return;
            var chars = CharactersSkin.FirstOrDefault(x => x.charId == charid);
            if (chars != null)
            {
                chars.headoverlays = headoverlays;

                using (gtaContext db = new gtaContext())
                {
                    db.Characters_Skin.Update(chars);
                    db.SaveChanges();
                }
            }
        }

        public static bool ExistPhoneNumber(int phoneNumber)
        {
            try
            {
                var charData = PlayerCharacters.ToList().FirstOrDefault(x => x.phonenumber == phoneNumber);
                if (charData != null) return true;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static int GetCharacterPhonenumber(int charId)
        {
            try
            {
                var chars = PlayerCharacters.ToList().FirstOrDefault(x => x.charId == charId);
                if (chars != null) return chars.phonenumber;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static void GenerateCharacterPhonenumber(IPlayer player, int charId)
        {
            try
            {
                if (player == null || !player.Exists) return;
                int generatedNumber = new Random().Next(1000000, 9999999);
                if (ExistPhoneNumber(generatedNumber))
                {
                    GenerateCharacterPhonenumber(player, charId);
                    return;
                }

                var charData = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (charData == null) return;
                charData.phonenumber = generatedNumber;
                using (var db = new gtaContext())
                {
                    db.AccountsCharacters.Update(charData);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static string GetCharacterHeadOverlays(int charid)
        {
            if (charid == 0) return "";
            var chars = CharactersSkin.FirstOrDefault(x => x.charId == charid);
            if (chars != null)
            {
                return chars.headoverlays;
            }

            return "";
        }

        public static void CreateCharacterLastPos(int charid, Position pos, short dimension)
        {
            var CharLastPosData = new Characters_LastPos
            {
                charId = charid,
                lastPosX = pos.X,
                lastPosY = pos.Y,
                lastPosZ = pos.Z,
                lastDimension = (int)dimension
            };

            try
            {
                CharactersLastPos.Add(CharLastPosData);

                using (gtaContext db = new gtaContext())
                {
                    db.Characters_LastPos.Add(CharLastPosData);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Charselector:KillCharacter")]
        public async Task KillCharacter(IPlayer client, int charId)
        {
            try
            {
                if (client == null || !client.Exists) return;
                var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);

                if (chars != null)
                {
                    chars.death = true;
                    using (gtaContext db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }

                    if (client.Exists && client != null) LoginHandler.SendDataToCharselectorArea((ClassicPlayer)client);
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static int GetCharacterPaydayTime(int charId)
        {
            if (charId == 0) return 0;
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
            if (chars != null)
            {
                return chars.paydayTime;
            }
            return 0;
        }

        public static void IncreaseCharacterPaydayTime(int charId)
        {
            try
            {
                if (charId == 0) return;
                var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
                if (chars != null)
                {
                    chars.paydayTime += 1;

                    using (gtaContext db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e) { Alt.Log($"{e}"); }
        }

        public static void ResetCharacterJobHourCounter(int charId)
        {
            try
            {
                if (charId == 0) return;
                var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
                if (chars != null)
                {
                    chars.jobHourCounter = 0;
                    using (gtaContext db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e) { Alt.Log($"{e}"); }
        }

        public static void IncreaseCharacterPlayTimeHours(int charId)
        {
            try
            {
                if (charId <= 0) return;
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars != null)
                {
                    chars.playtimeHours += 1;
                    using (var db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void IncreaseCharacterJobHourCounter(int charId)
        {
            try
            {
                if (charId == 0) return;
                var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
                if (chars != null)
                {
                    chars.jobHourCounter += 1;
                    using (gtaContext db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e) { Alt.Log($"{e}"); }
        }

        public static void ResetCharacterPaydayTime(int charId)
        {
            try
            {
                if (charId <= 0) return;
                var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
                if (chars != null)
                {
                    chars.paydayTime = 0;
                    using (gtaContext db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e) { Alt.Log($"{e}"); }
        }

        public static string GetCharacterName(int charId)
        {
            try
            {
                if (charId <= 0) return "";
                var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
                if (chars != null)
                {
                    return chars.charname;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return "SYSTEM";
        }

        public static string GetCharacterBirthdate(int charId)
        {
            try
            {
                if (charId <= 0) return "";
                var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
                if (chars != null)
                {
                    return chars.birthdate;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return "";
        }

        public static string GetCharacterBirthplace(int charId)
        {
            try
            {
                if (charId <= 0) return "None";
                var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
                if (chars != null)
                {
                    return chars.birthplace;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return "None";
        }

        public static int GetCharacterAccState(int charId)
        {
            try
            {
                if (charId <= 0) return 0;
                var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
                if (chars != null)
                {
                    return chars.accState;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static string GetCharacterStreet(int charId)
        {
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
            if (chars != null)
            {
                return chars.address;
            }
            return "Boulevard Del Perro 2a";
        }

        public static void SetCharacterStreet(int charId, string st)
        {
            try
            {
                if (charId <= 0 || st == "") return;
                var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
                if (chars != null)
                {
                    chars.address = st;
                    using (gtaContext db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static object GetCharacterWeapon(IPlayer player, string type)
        {
            object obj = null;
            if (player == null || !player.Exists) return obj;
            int charId = User.GetPlayerOnline(player);
            if (charId == 0) return obj;
            var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
            if (chars != null)
            {
                switch (type)
                {
                    case "PrimaryWeapon": obj = chars.weapon_Primary; break;
                    case "PrimaryAmmo": obj = chars.weapon_Primary_Ammo; break;
                    case "SecondaryWeapon": obj = chars.weapon_Secondary; break;
                    case "SecondaryAmmo": obj = chars.weapon_Secondary_Ammo; break;
                    case "SecondaryWeapon2": obj = chars.weapon_Secondary2; break;
                    case "SecondaryAmmo2": obj = chars.weapon_Secondary2_Ammo; break;
                    case "FistWeapon": obj = chars.weapon_Fist; break;
                }
            }
            return obj;
        }

        public static void SetCharacterWeapon(IPlayer player, string type, object weaponValue)
        {
            try
            {
                if (player == null || !player.Exists || type == "" || weaponValue == null) return;
                int charId = User.GetPlayerOnline(player);
                if (charId == 0) return;
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars != null)
                {
                    switch (type)
                    {
                        case "PrimaryWeapon": chars.weapon_Primary = (string)weaponValue; break;
                        case "PrimaryAmmo": chars.weapon_Primary_Ammo = (int)weaponValue; break;
                        case "SecondaryWeapon": chars.weapon_Secondary = (string)weaponValue; break;
                        case "SecondaryAmmo": chars.weapon_Secondary_Ammo = (int)weaponValue; break;
                        case "SecondaryWeapon2": chars.weapon_Secondary2 = (string)weaponValue; break;
                        case "SecondaryAmmo2": chars.weapon_Secondary2_Ammo = (int)weaponValue; break;
                        case "FistWeapon": chars.weapon_Fist = (string)weaponValue; break;
                        case "FistWeaponAmmo": chars.weapon_Fist_Ammo = (int)weaponValue; break;
                    }

                    using (gtaContext db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static bool ExistCharacterName(string charName)
        {
            var chars = PlayerCharacters.FirstOrDefault(p => p.charname == charName);

            if (chars != null)
            {
                return true;
            }

            return false;
        }

        public static bool GetCharacterGender(int charid)
        {
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charid);

            if (chars != null)
            {
                return chars.gender;
            }

            return false;
        }

        public static bool GetCharacterFirstJoin(int charId)
        {
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);

            if (chars != null)
            {
                return chars.firstJoin;
            }
            return false;
        }

        public static int GetCharacterIdFromCharName(string charname)
        {
            var chars = PlayerCharacters.FirstOrDefault(p => p.charname == charname);

            if (chars != null)
            {
                return chars.charId;
            }

            return 0;
        }
        public static int GetCharacterIdFromAccId(int accid)
        {
            var chars = PlayerCharacters.FirstOrDefault(p => p.accountId == accid);

            if (chars != null)
            {
                return chars.charId;
            }

            return 0;
        }

        public static string GetCharacterSkin(string type, int charid)
        {
            var chars = CharactersSkin.FirstOrDefault(p => p.charId == charid);

            if (chars != null)
            {
                switch (type)
                {
                    case "facefeatures":
                        return chars.facefeatures;
                    case "headblendsdata":
                        return chars.headblendsdata;
                    case "headoverlays":
                        return chars.headoverlays;
                }
            }
            return "";
        }

        public static string GetPlayerCharacters(IPlayer player)
        {
            if (player == null || !player.Exists) return "";

            var items = PlayerCharacters.Where(x => x.accountId == User.GetPlayerAccountId(player)).Select(x => new
            {
                accountId = x.accountId,
                charId = x.charId,
                charname = x.charname,
                death = x.death,
                firstjoin = x.firstJoin,
                gender = x.gender,
            }).ToList();

            return JsonConvert.SerializeObject(items);
        }

        public static string GetCharacterInformations(int charId)
        {
            try
            {
                if (charId <= 0) return "[]";
                var items = PlayerCharacters.Where(x => x.charId == charId).Select(x => new
                {
                    x.charId,
                    x.charname,
                    x.birthdate,
                    x.birthplace,
                    address = $"{x.address}",
                    firstJoin = x.firstJoinTimestamp.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                }).ToList();
                return JsonConvert.SerializeObject(items);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
                return "[]";
            }
        }

        public static string GetCharacterFactionInformations(int charId)
        {
            try
            {
                if (charId <= 0) return "[]";
                var items = PlayerCharacters.Where(x => x.charId == charId).Select(x => new
                {
                    x.charId,
                    x.charname,
                    x.birthdate,
                    factionId = ServerFactions.GetCharacterFactionId(charId),
                    factionShort = ServerFactions.GetFactionShortName(ServerFactions.GetCharacterFactionId(charId)),
                    servicenumber = ServerFactions.GetCharacterFactionServiceNumber(charId),
                    rankname = ServerFactions.GetFactionRankName(ServerFactions.GetCharacterFactionId(charId), ServerFactions.GetCharacterFactionRank(charId)),
                    firstJoin = x.firstJoinTimestamp.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                }).ToList();
                return JsonConvert.SerializeObject(items);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
                return "[]";
            }
        }

        public static string GetCharacterFirstSpawnPlace(IPlayer player, int charId)
        {
            if (player == null || !player.Exists || charId == 0) return "";
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId && p.accountId == User.GetPlayerAccountId(player));

            if (chars != null)
            {
                return chars.firstSpawnPlace;
            }

            return "";
        }

        public static Position GetCharacterLastPosition(int charId)
        {
            var chars = CharactersLastPos.FirstOrDefault(p => p.charId == charId);

            if (chars != null)
            {
                return new Position(chars.lastPosX, chars.lastPosY, chars.lastPosZ);
            }

            return new Position(0, 0, 0);
        }

        public static void SetCharacterLastPosition(int charId, Position pos, int dimension)
        {
            var chars = CharactersLastPos.FirstOrDefault(p => p.charId == charId);

            if (chars != null)
            {
                chars.lastPosX = pos.X;
                chars.lastPosY = pos.Y;
                chars.lastPosZ = pos.Z;
                chars.lastDimension = dimension;

                try
                {
                    using (gtaContext db = new gtaContext())
                    {
                        db.Characters_LastPos.Update(chars);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Alt.Log($"{e}");
                }
            }
        }

        public static int GetCharacterLastDimension(int charId)
        {
            var chars = CharactersLastPos.FirstOrDefault(p => p.charId == charId);

            if (chars != null)
            {
                return chars.lastDimension;
            }

            return 0;
        }

        public static bool IsCharacterCrimeFlagged(int charId)
        {
            if (charId == 0) return false;
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
            if (chars != null)
            {
                return chars.isCrime;
            }
            return false;
        }

        public static bool HasCharacterPermission(int charId, string permission)
        {
            if (charId == 0 || permission == "") return false;
            var chars = CharactersPermissions.FirstOrDefault(p => p.charId == charId && p.permissionName == permission);
            if (chars != null)
            {
                return true;
            }
            return false;
        }

        public static void AddCharacterPermission(int charId, string permission)
        {
            if (charId == 0 || permission == "") return;
            var chars = CharactersPermissions.FirstOrDefault(p => p.charId == charId && p.permissionName == permission);
            if (chars == null)
            {
                var permissionData = new Characters_Permissions
                {
                    charId = charId,
                    permissionName = permission
                };
                CharactersPermissions.Add(permissionData);

                using (gtaContext db = new gtaContext())
                {
                    db.Characters_Permissions.Add(permissionData);
                    db.SaveChanges();
                }
            }
        }

        public static void RemoveCharacterPermission(int charId, string permission)
        {
            if (charId == 0 || permission == "") return;
            var chars = CharactersPermissions.FirstOrDefault(p => p.charId == charId && p.permissionName == permission);
            if (chars != null)
            {
                CharactersPermissions.Remove(chars);
                using (gtaContext db = new gtaContext())
                {
                    db.Characters_Permissions.Remove(chars);
                    db.SaveChanges();
                }
            }
        }

        public static void SetCharacterCrimeFlagged(int charId, bool state)
        {
            if (charId == 0) return;
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
            if (chars == null) return;
            chars.isCrime = state;
            try
            {
                using (gtaContext db = new gtaContext())
                {
                    db.AccountsCharacters.Update(chars);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static int GetCharacterHealth(int charId)
        {
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);

            if (chars != null)
            {
                return chars.health;
            }

            return 0;
        }

        public static int GetCharacterArmor(int charId)
        {
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);

            if (chars != null)
            {
                return chars.armor;
            }

            return 0;
        }

        public static int GetCharacterHunger(int charId)
        {
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);

            if (chars != null)
            {
                return chars.hunger;
            }

            return 0;
        }

        public static int GetCharacterThirst(int charId)
        {
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);

            if (chars != null)
            {
                return chars.thirst;
            }

            return 0;
        }

        public static int GetCharacterClothes(int charId, string clothesType)
        {
            if (charId == 0) return -2;
            var chars = CharactersSkin.FirstOrDefault(p => p.charId == charId);
            if (chars != null)
            {
                switch (clothesType)
                {
                    case "Top":
                        return chars.clothesTop;
                    case "Torso":
                        return chars.clothesTorso;
                    case "Leg":
                        return chars.clothesLeg;
                    case "Feet":
                        return chars.clothesFeet;
                    case "Hat":
                        return chars.clothesHat;
                    case "Glass":
                        return chars.clothesGlass;
                    case "Necklace":
                        return chars.clothesNecklace;
                    case "Mask":
                        return chars.clothesMask;
                    case "Armor":
                        return chars.clothesArmor;
                    case "Undershirt":
                        return chars.clothesUndershirt;
                    case "Decal":
                        return chars.clothesDecal;
                    case "Bracelet":
                        return chars.clothesBracelet;
                    case "Watch":
                        return chars.clothesWatch;
                    case "Earring":
                        return chars.clothesEarring;

                }
            }
            return -2;
        }

        public static int GetCharacterBackpack(int charId)
        {
            if (charId == 0) return -2;
            var chars = CharactersSkin.FirstOrDefault(p => p.charId == charId);
            if (chars != null)
            {
                return chars.clothesBag;
            }
            return -2;
        }

        //66 = Rucksack
        //45 = Tasche
        //86 = jedes Armypack
        public static float GetCharacterBackpackSize(int BackpackDrawId)
        {
            switch (BackpackDrawId)
            {
                case 66:
                    return 15f;
                case 45:
                    return 25f;
                case 86:
                    return 35f;
            }
            return 0f;
        }

        public static string GetCharacterJob(int charId)
        {
            if (charId == 0) return "None";
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
            if (chars != null) { return chars.job; }
            return "None";
        }

        public static int GetCharacterJobHourCounter(int charId)
        {
            if (charId == 0) return 0;
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
            if (chars != null) { return chars.jobHourCounter; }
            return 0;
        }

        public static void SetCharacterJob(int charId, string job)
        {
            try
            {
                if (charId == 0 || job == "") return;
                var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
                if (chars != null)
                {
                    chars.job = job;
                    using (gtaContext db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e) { Alt.Log($"{e}"); }
        }

        public static void SetCharacterLastJobPaycheck(int charId, DateTime dt)
        {
            try
            {
                if (charId == 0) return;
                var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
                if (chars != null)
                {
                    chars.lastJobPaycheck = dt;
                    using (gtaContext db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e) { Alt.Log($"{e}"); }
        }

        public static void SetCharacterLastLogin(int charId, DateTime dt)
        {
            try
            {
                if (charId <= 0) return;
                var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
                if (chars != null)
                {
                    chars.lastLogin = dt;
                    using (gtaContext db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static DateTime GetCharacterLastLogin(int charId)
        {
            DateTime dt = new DateTime();
            try
            {
                var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
                if (chars != null) dt = Convert.ToDateTime(chars.lastLogin);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return dt;
        }

        public static DateTime GetCharacterLastJobPaycheck(int charId)
        {
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
            return chars.lastJobPaycheck;
        }

        public static DateTime GetCharacterFirstJoinDate(int charId)
        {
            DateTime dt = new DateTime(0001, 01, 01);
            if (charId <= 0) return dt;
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
            if (chars != null)
            {
                dt = chars.firstJoinTimestamp;
            }
            return dt;
        }

        public static void SetCharacterHealth(int charId, int health)
        {
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);

            if (chars != null)
            {
                chars.health = health - 100;

                try
                {
                    using (gtaContext db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Alt.Log($"{e}");
                }
            }
        }

        public static void SetCharacterBirthplace(int charId, string place)
        {
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
            if (chars != null)
            {
                chars.birthplace = place;
                try
                {
                    using (gtaContext db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Alt.Log($"{e}");
                }
            }
        }

        public static void setCharacterAccState(int charId, int state)
        {
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);
            if (chars != null)
            {
                chars.accState = state;
                try
                {
                    using (gtaContext db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Alt.Log($"{e}");
                }
            }
        }

        public static void SetCharacterArmor(int charId, int armor)
        {
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);

            if (chars != null)
            {
                chars.armor = armor;

                try
                {
                    using (gtaContext db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Alt.Log($"{e}");
                }
            }
        }
        
        public static void SetCharacterAmmo(int charId, int ammo, int slot)
        {
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);

            if (chars != null)
            {
               if (slot == 1)
                {
                    chars.weapon_Primary_Ammo = ammo;

                    try
                    {
                        using (gtaContext db = new gtaContext())
                        {
                            db.AccountsCharacters.Update(chars);
                            db.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    {
                        Alt.Log($"{e}");
                    }
                } else if (slot == 2)
                {
                    chars.weapon_Secondary_Ammo = ammo;

                    try
                    {
                        using (gtaContext db = new gtaContext())
                        {
                            db.AccountsCharacters.Update(chars);
                            db.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    {
                        Alt.Log($"{e}");
                    }
                }
                else if (slot == 3)
                {
                    chars.weapon_Secondary2_Ammo = ammo;

                    try
                    {
                        using (gtaContext db = new gtaContext())
                        {
                            db.AccountsCharacters.Update(chars);
                            db.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    {
                        Alt.Log($"{e}");
                    }
                }
            }
        }

        public static void SetCharacterHunger(int charId, int hunger)
        {
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);

            if (chars != null)
            {
                chars.hunger = hunger;
                if (chars.hunger > 100)
                {
                    chars.hunger = 100;
                }

                try
                {
                    using (gtaContext db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Alt.Log($"{e}");
                }
            }
        }

        public static void SetCharacterThirst(int charId, int thirst)
        {
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId);

            if (chars != null)
            {
                chars.thirst = thirst;
                if (chars.thirst > 100)
                {
                    chars.thirst = 100;
                }
                try
                {
                    using (gtaContext db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Alt.Log($"{e}");
                }
            }
        }

        public static void SetCharacterFirstSpawnPlace(IPlayer player, int charId, string spawnplace)
        {
            if (player == null || !player.Exists || charId == 0) return;
            var chars = PlayerCharacters.FirstOrDefault(p => p.charId == charId && p.accountId == User.GetPlayerAccountId(player));

            if (chars != null)
            {
                chars.firstSpawnPlace = spawnplace;
                chars.firstJoin = false;

                try
                {
                    using (gtaContext db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Alt.Log($"{e}");
                }
            }
        }



        public static void SetCharacterBackpack(IPlayer player, string backpack)
        {
            if (player == null || !player.Exists) return;
            var charId = User.GetPlayerOnline(player);
            if (charId == 0) return;
            var chars = CharactersSkin.FirstOrDefault(p => p.charId == charId);
            if (chars != null)
            {
                int BackpackDrawId = -2;
                switch (backpack)
                {
                    case "-2":
                        player.EmitLocked("Client:SpawnArea:setCharClothes", 5, 0, 0);
                        break;
                    case "Rucksack":
                        player.EmitLocked("Client:SpawnArea:setCharClothes", 5, 66, 0);
                        BackpackDrawId = 66;
                        break;
                    case "Tasche":
                        player.EmitLocked("Client:SpawnArea:setCharClothes", 5, 45, 0);
                        BackpackDrawId = 45;
                        break;
                    case "Armytasche":
                        player.EmitLocked("Client:SpawnArea:setCharClothes", 5, 86, 2);
                        BackpackDrawId = 86;
                        break;
                }

                try
                {
                    chars.clothesBag = BackpackDrawId;
                    using (gtaContext db = new gtaContext())
                    {
                        db.Characters_Skin.Update(chars);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Alt.Log($"{e}");
                }
            }
        }

        public static void SetCharacterCorrectTorso(IPlayer player, int topID)
        {
            if (player == null || !player.Exists) return;
            int charId = User.GetPlayerOnline(player);
            if (charId == 0) return;
            int CTorso = 0;
            if (GetCharacterGender(charId) == false)
            {
                switch (topID)
                {
                    case 0: CTorso = 0; break;
                    case 1: CTorso = 0; break;
                    case 2: CTorso = 2; break;
                    case 3: CTorso = 14; break;
                    case 4: CTorso = 14; break;
                    case 5: CTorso = 5; break;
                    case 6: CTorso = 14; break;
                    case 7: CTorso = 14; break;
                    case 8: CTorso = 8; break;
                    case 9: CTorso = 0; break;
                    case 10: CTorso = 14; break;
                    case 11: CTorso = 15; break;
                    case 12: CTorso = 12; break;
                    case 13: CTorso = 11; break;
                    case 14: CTorso = 12; break;
                    case 15: CTorso = 15; break;
                    case 16: CTorso = 0; break;
                    case 17: CTorso = 5; break;
                    case 18: CTorso = 0; break;
                    case 19: CTorso = 14; break;
                    case 20: CTorso = 14; break;
                    case 21: CTorso = 15; break;
                    case 22: CTorso = 0; break;
                    case 23: CTorso = 14; break;
                    case 24: CTorso = 14; break;
                    case 25: CTorso = 15; break;
                    case 26: CTorso = 11; break;
                    case 27: CTorso = 14; break;
                    case 28: CTorso = 14; break;
                    case 29: CTorso = 14; break;
                    case 30: CTorso = 14; break;
                    case 31: CTorso = 12; break;
                    case 32: CTorso = 14; break;
                    case 33: CTorso = 0; break;
                    case 34: CTorso = 0; break;
                    case 35: CTorso = 14; break;
                    case 36: CTorso = 5; break;
                    case 37: CTorso = 14; break;
                    case 38: CTorso = 8; break;
                    case 39: CTorso = 0; break;
                    case 40: CTorso = 15; break;
                    case 41: CTorso = 12; break;
                    case 42: CTorso = 11; break;
                    case 43: CTorso = 11; break;
                    case 44: CTorso = 0; break;
                    case 45: CTorso = 15; break;
                    case 46: CTorso = 14; break;
                    case 47: CTorso = 0; break;
                    case 48: CTorso = 1; break;
                    case 49: CTorso = 1; break;
                    case 50: CTorso = 17; break;
                    case 51: CTorso = 1; break;
                    case 52: CTorso = 2; break;
                    case 53: CTorso = 0; break;
                    case 54: CTorso = 1; break;
                    case 55: CTorso = 0; break;
                    case 56: CTorso = 0; break;
                    case 57: CTorso = 0; break;
                    case 58: CTorso = 14; break;
                    case 59: CTorso = 14; break;
                    case 60: CTorso = 15; break;
                    case 61: CTorso = 0; break;
                    case 62: CTorso = 14; break;
                    case 63: CTorso = 5; break;
                    case 64: CTorso = 14; break;
                    case 65: CTorso = 14; break;
                    case 66: CTorso = 15; break;
                    case 67: CTorso = 1; break;
                    case 68: CTorso = 14; break;
                    case 69: CTorso = 14; break;
                    case 70: CTorso = 0; break;
                    case 71: CTorso = 14; break;
                    case 72: CTorso = 0; break;
                    case 73: CTorso = 14; break;
                    case 74: CTorso = 14; break;
                    case 75: CTorso = 14; break;
                    case 76: CTorso = 14; break;
                    case 77: CTorso = 14; break;
                    case 78: CTorso = 14; break;
                    case 79: CTorso = 14; break;
                    case 80: CTorso = 0; break;
                    case 81: CTorso = 0; break;
                    case 82: CTorso = 0; break;
                    case 83: CTorso = 0; break;
                    case 84: CTorso = 1; break;
                    case 85: CTorso = 1; break;
                    case 86: CTorso = 1; break;
                    case 87: CTorso = 1; break;
                    case 88: CTorso = 14; break;
                    case 89: CTorso = 14; break;
                    case 90: CTorso = 14; break;
                    case 91: CTorso = 15; break;
                    case 92: CTorso = 6; break;
                    case 93: CTorso = 0; break;
                    case 94: CTorso = 0; break;
                    case 95: CTorso = 11; break;
                    case 96: CTorso = 11; break;
                    case 97: CTorso = 0; break;
                    case 98: CTorso = 0; break;
                    case 99: CTorso = 14; break;
                    case 100: CTorso = 14; break;
                    case 101: CTorso = 14; break;
                    case 102: CTorso = 14; break;
                    case 103: CTorso = 14; break;
                    case 104: CTorso = 14; break;
                    case 105: CTorso = 11; break;
                    case 106: CTorso = 14; break;
                    case 107: CTorso = 14; break;
                    case 108: CTorso = 14; break;
                    case 109: CTorso = 5; break;
                    case 110: CTorso = 1; break;
                    case 111: CTorso = 4; break;
                    case 112: CTorso = 14; break;
                    case 113: CTorso = 6; break;
                    case 114: CTorso = 14; break;
                    case 115: CTorso = 14; break;
                    case 116: CTorso = 14; break;
                    case 117: CTorso = 6; break;
                    case 118: CTorso = 14; break;
                    case 119: CTorso = 14; break;
                    case 120: CTorso = 15; break;
                    case 121: CTorso = 14; break;
                    case 122: CTorso = 14; break;
                    case 123: CTorso = 11; break;
                    case 124: CTorso = 14; break;
                    case 125: CTorso = 14; break;
                    case 126: CTorso = 1; break;
                    case 127: CTorso = 14; break;
                    case 128: CTorso = 0; break;
                    case 129: CTorso = 0; break;
                    case 130: CTorso = 14; break;
                    case 131: CTorso = 0; break;
                    case 132: CTorso = 0; break;
                    case 133: CTorso = 0; break;
                    case 134: CTorso = 0; break;
                    case 135: CTorso = 0; break;
                    case 136: CTorso = 14; break;
                    case 137: CTorso = 15; break;
                    case 138: CTorso = 14; break;
                    case 139: CTorso = 12; break;
                    case 140: CTorso = 14; break;
                    case 141: CTorso = 6; break;
                    case 142: CTorso = 14; break;
                    case 143: CTorso = 14; break;
                    case 144: CTorso = 6; break;
                    case 145: CTorso = 14; break;
                    case 146: CTorso = 0; break;
                    case 147: CTorso = 4; break;
                    case 148: CTorso = 4; break;
                    case 149: CTorso = 14; break;
                    case 150: CTorso = 14; break;
                    case 151: CTorso = 14; break;
                    case 152: CTorso = 14; break;
                    case 153: CTorso = 14; break;
                    case 154: CTorso = 14; break;
                    case 155: CTorso = 14; break;
                    case 156: CTorso = 14; break;
                    case 157: CTorso = 15; break;
                    case 158: CTorso = 15; break;
                    case 159: CTorso = 15; break;
                    case 160: CTorso = 15; break;
                    case 161: CTorso = 14; break;
                    case 162: CTorso = 15; break;
                    case 163: CTorso = 14; break;
                    case 164: CTorso = 0; break;
                    case 165: CTorso = 0; break;
                    case 166: CTorso = 14; break;
                    case 167: CTorso = 14; break;
                    case 168: CTorso = 14; break;
                    case 169: CTorso = 14; break;
                    case 170: CTorso = 15; break;
                    case 171: CTorso = 1; break;
                    case 172: CTorso = 14; break;
                    case 173: CTorso = 15; break;
                    case 174: CTorso = 14; break;
                    case 175: CTorso = 15; break;
                    case 176: CTorso = 15; break;
                    case 177: CTorso = 15; break;
                    case 178: CTorso = 1; break;
                    case 179: CTorso = 15; break;
                    case 180: CTorso = 15; break;
                    case 181: CTorso = 15; break;
                    case 182: CTorso = 1; break;
                    case 183: CTorso = 14; break;
                    case 184: CTorso = 14; break;
                    case 185: CTorso = 14; break;
                    case 186: CTorso = 14; break;
                    case 187: CTorso = 14; break;
                    case 188: CTorso = 14; break;
                    case 189: CTorso = 14; break;
                    case 190: CTorso = 14; break;
                    case 191: CTorso = 14; break;
                    case 192: CTorso = 14; break;
                    case 193: CTorso = 0; break;
                    case 194: CTorso = 1; break;
                    case 195: CTorso = 1; break;
                    case 196: CTorso = 1; break;
                    case 197: CTorso = 1; break;
                    case 198: CTorso = 1; break;
                    case 199: CTorso = 1; break;
                    case 200: CTorso = 1; break;
                    case 201: CTorso = 3; break;
                    case 202: CTorso = 2; break;
                    case 203: CTorso = 1; break;
                    case 204: CTorso = 6; break;
                    case 205: CTorso = 5; break;
                    case 206: CTorso = 5; break;
                    case 207: CTorso = 5; break;
                    case 208: CTorso = 0; break;
                    case 209: CTorso = 0; break;
                    case 210: CTorso = 0; break;
                    case 211: CTorso = 0; break;
                    case 212: CTorso = 14; break;
                    case 213: CTorso = 15; break;
                    case 214: CTorso = 14; break;
                    case 215: CTorso = 14; break;
                    case 216: CTorso = 15; break;
                    case 217: CTorso = 14; break;
                    case 218: CTorso = 14; break;
                    case 219: CTorso = 15; break;
                    case 220: CTorso = 14; break;
                    case 221: CTorso = 14; break;
                    case 222: CTorso = 11; break;
                    case 223: CTorso = 5; break;
                    case 224: CTorso = 1; break;
                    case 225: CTorso = 8; break;
                    case 226: CTorso = 0; break;
                    case 227: CTorso = 4; break;
                    case 228: CTorso = 4; break;
                    case 229: CTorso = 14; break;
                    case 230: CTorso = 14; break;
                    case 231: CTorso = 4; break;
                    case 232: CTorso = 14; break;
                    case 233: CTorso = 14; break;
                    case 234: CTorso = 11; break;
                    case 235: CTorso = 0; break;
                    case 236: CTorso = 0; break;
                    case 237: CTorso = 5; break;
                    case 238: CTorso = 2; break;
                    case 239: CTorso = 2; break;
                    case 240: CTorso = 4; break;
                    case 241: CTorso = 0; break;
                    case 242: CTorso = 0; break;
                    case 243: CTorso = 4; break;
                    case 244: CTorso = 14; break;
                    case 245: CTorso = 6; break;
                    case 246: CTorso = 3; break;
                    case 247: CTorso = 5; break;
                    case 248: CTorso = 6; break;
                    case 249: CTorso = 6; break;
                    case 250: CTorso = 0; break;
                    case 251: CTorso = 4; break;
                    case 252: CTorso = 15; break;
                    case 253: CTorso = 4; break;
                    case 333: CTorso = 30; break;
                }
            }
            else
            {
                switch (topID)
                {
                    case 0: CTorso = 0; break;
                    case 1: CTorso = 5; break;
                    case 2: CTorso = 2; break;
                    case 3: CTorso = 3; break;
                    case 4: CTorso = 4; break;
                    case 5: CTorso = 4; break;
                    case 6: CTorso = 5; break;
                    case 7: CTorso = 5; break;
                    case 8: CTorso = 5; break;
                    case 9: CTorso = 0; break;
                    case 10: CTorso = 5; break;
                    case 11: CTorso = 4; break;
                    case 12: CTorso = 12; break;
                    case 13: CTorso = 15; break;
                    case 14: CTorso = 14; break;
                    case 15: CTorso = 15; break;
                    case 16: CTorso = 15; break;
                    case 17: CTorso = 0; break;
                    case 18: CTorso = 15; break;
                    case 19: CTorso = 15; break;
                    case 20: CTorso = 5; break;
                    case 21: CTorso = 4; break;
                    case 22: CTorso = 4; break;
                    case 23: CTorso = 4; break;
                    case 24: CTorso = 5; break;
                    case 25: CTorso = 5; break;
                    case 26: CTorso = 12; break;
                    case 27: CTorso = 0; break;
                    case 28: CTorso = 15; break;
                    case 29: CTorso = 9; break;
                    case 30: CTorso = 2; break;
                    case 31: CTorso = 5; break;
                    case 32: CTorso = 4; break;
                    case 33: CTorso = 4; break;
                    case 34: CTorso = 6; break;
                    case 35: CTorso = 5; break;
                    case 36: CTorso = 4; break;
                    case 37: CTorso = 4; break;
                    case 38: CTorso = 2; break;
                    case 39: CTorso = 1; break;
                    case 40: CTorso = 2; break;
                    case 41: CTorso = 5; break;
                    case 42: CTorso = 5; break;
                    case 43: CTorso = 18; break;
                    case 44: CTorso = 3; break;
                    case 45: CTorso = 3; break;
                    case 46: CTorso = 3; break;
                    case 47: CTorso = 3; break;
                    case 48: CTorso = 14; break;
                    case 49: CTorso = 14; break;
                    case 50: CTorso = 14; break;
                    case 51: CTorso = 6; break;
                    case 52: CTorso = 6; break;
                    case 53: CTorso = 5; break;
                    case 54: CTorso = 5; break;
                    case 55: CTorso = 5; break;
                    case 56: CTorso = 14; break;
                    case 57: CTorso = 5; break;
                    case 58: CTorso = 5; break;
                    case 59: CTorso = 5; break;
                    case 60: CTorso = 14; break;
                    case 61: CTorso = 3; break;
                    case 62: CTorso = 5; break;
                    case 63: CTorso = 5; break;
                    case 64: CTorso = 5; break;
                    case 65: CTorso = 5; break;
                    case 66: CTorso = 6; break;
                    case 67: CTorso = 2; break;
                    case 68: CTorso = 0; break;
                    case 69: CTorso = 0; break;
                    case 70: CTorso = 0; break;
                    case 71: CTorso = 0; break;
                    case 72: CTorso = 0; break;
                    case 73: CTorso = 14; break;
                    case 74: CTorso = 15; break;
                    case 75: CTorso = 9; break;
                    case 76: CTorso = 9; break;
                    case 77: CTorso = 9; break;
                    case 78: CTorso = 9; break;
                    case 79: CTorso = 9; break;
                    case 80: CTorso = 9; break;
                    case 81: CTorso = 9; break;
                    case 82: CTorso = 15; break;
                    case 83: CTorso = 9; break;
                    case 84: CTorso = 14; break;
                    case 85: CTorso = 14; break;
                    case 86: CTorso = 9; break;
                    case 87: CTorso = 9; break;
                    case 88: CTorso = 0; break;
                    case 89: CTorso = 0; break;
                    case 90: CTorso = 6; break;
                    case 91: CTorso = 6; break;
                    case 92: CTorso = 5; break;
                    case 93: CTorso = 5; break;
                    case 94: CTorso = 5; break;
                    case 95: CTorso = 5; break;
                    case 96: CTorso = 4; break;
                    case 97: CTorso = 5; break;
                    case 98: CTorso = 5; break;
                    case 99: CTorso = 5; break;
                    case 100: CTorso = 15; break;
                    case 101: CTorso = 15; break;
                    case 102: CTorso = 3; break;
                    case 103: CTorso = 3; break;
                    case 104: CTorso = 5; break;
                    case 105: CTorso = 4; break;
                    case 106: CTorso = 6; break;
                    case 107: CTorso = 6; break;
                    case 108: CTorso = 6; break;
                    case 109: CTorso = 6; break;
                    case 110: CTorso = 6; break;
                    case 111: CTorso = 4; break;
                    case 112: CTorso = 4; break;
                    case 113: CTorso = 4; break;
                    case 114: CTorso = 4; break;
                    case 115: CTorso = 4; break;
                    case 116: CTorso = 4; break;
                    case 117: CTorso = 11; break;
                    case 118: CTorso = 11; break;
                    case 119: CTorso = 11; break;
                    case 120: CTorso = 6; break;
                    case 121: CTorso = 6; break;
                    case 122: CTorso = 2; break;
                    case 123: CTorso = 2; break;
                    case 124: CTorso = 15; break;
                    case 125: CTorso = 14; break;
                    case 126: CTorso = 14; break;
                    case 127: CTorso = 14; break;
                    case 128: CTorso = 14; break;
                    case 129: CTorso = 14; break;
                    case 130: CTorso = 0; break;
                    case 131: CTorso = 3; break;
                    case 132: CTorso = 2; break;
                    case 133: CTorso = 5; break;
                    case 134: CTorso = 15; break;
                    case 135: CTorso = 3; break;
                    case 136: CTorso = 3; break;
                    case 137: CTorso = 5; break;
                    case 138: CTorso = 6; break;
                    case 139: CTorso = 5; break;
                    case 140: CTorso = 5; break;
                    case 141: CTorso = 14; break;
                    case 142: CTorso = 9; break;
                    case 143: CTorso = 5; break;
                    case 144: CTorso = 3; break;
                    case 145: CTorso = 3; break;
                    case 146: CTorso = 7; break;
                    case 147: CTorso = 1; break;
                    case 148: CTorso = 5; break;
                    case 149: CTorso = 5; break;
                    case 150: CTorso = 0; break;
                    case 151: CTorso = 0; break;
                    case 152: CTorso = 7; break;
                    case 153: CTorso = 5; break;
                    case 154: CTorso = 15; break;
                    case 155: CTorso = 15; break;
                    case 156: CTorso = 15; break;
                    case 157: CTorso = 15; break;
                    case 158: CTorso = 15; break;
                    case 159: CTorso = 15; break;
                    case 160: CTorso = 15; break;
                    case 161: CTorso = 11; break;
                    case 162: CTorso = 0; break;
                    case 163: CTorso = 5; break;
                    case 164: CTorso = 5; break;
                    case 165: CTorso = 5; break;
                    case 166: CTorso = 5; break;
                    case 167: CTorso = 15; break;
                    case 168: CTorso = 15; break;
                    case 169: CTorso = 15; break;
                    case 170: CTorso = 15; break;
                    case 171: CTorso = 15; break;
                    case 172: CTorso = 14; break;
                    case 173: CTorso = 15; break;
                    case 174: CTorso = 15; break;
                    case 175: CTorso = 15; break;
                    case 176: CTorso = 15; break;
                    case 177: CTorso = 15; break;
                    case 178: CTorso = 15; break;
                    case 179: CTorso = 11; break;
                    case 180: CTorso = 3; break;
                    case 181: CTorso = 15; break;
                    case 182: CTorso = 15; break;
                    case 183: CTorso = 15; break;
                    case 184: CTorso = 14; break;
                    case 185: CTorso = 6; break;
                    case 186: CTorso = 6; break;
                    case 187: CTorso = 6; break;
                    case 188: CTorso = 6; break;
                    case 189: CTorso = 6; break;
                    case 190: CTorso = 6; break;
                    case 191: CTorso = 6; break;
                    case 192: CTorso = 5; break;
                    case 193: CTorso = 5; break;
                    case 194: CTorso = 5; break;
                    case 195: CTorso = 4; break;
                    case 196: CTorso = 1; break;
                    case 197: CTorso = 1; break;
                    case 198: CTorso = 1; break;
                    case 199: CTorso = 1; break;
                    case 200: CTorso = 1; break;
                    case 201: CTorso = 1; break;
                    case 202: CTorso = 1; break;
                    case 203: CTorso = 8; break;
                    case 204: CTorso = 4; break;
                    case 205: CTorso = 0; break;
                    case 206: CTorso = 1; break;
                    case 207: CTorso = 4; break;
                    case 208: CTorso = 11; break;
                    case 209: CTorso = 11; break;
                    case 210: CTorso = 11; break;
                    case 211: CTorso = 11; break;
                    case 212: CTorso = 0; break;
                    case 213: CTorso = 1; break;
                    case 214: CTorso = 1; break;
                    case 215: CTorso = 1; break;
                    case 216: CTorso = 5; break;
                    case 217: CTorso = 4; break;
                    case 218: CTorso = 0; break;
                    case 219: CTorso = 5; break;
                    case 220: CTorso = 15; break;
                    case 221: CTorso = 15; break;
                    case 222: CTorso = 15; break;
                    case 223: CTorso = 15; break;
                    case 224: CTorso = 14; break;
                    case 225: CTorso = 15; break;
                    case 226: CTorso = 11; break;
                    case 227: CTorso = 3; break;
                    case 228: CTorso = 3; break;
                    case 229: CTorso = 4; break;
                    case 230: CTorso = 0; break;
                    case 231: CTorso = 0; break;
                    case 232: CTorso = 0; break;
                    case 233: CTorso = 11; break;
                    case 234: CTorso = 6; break;
                    case 235: CTorso = 1; break;
                    case 236: CTorso = 14; break;
                    case 237: CTorso = 3; break;
                    case 238: CTorso = 3; break;
                    case 239: CTorso = 3; break;
                    case 240: CTorso = 5; break;
                    case 241: CTorso = 3; break;
                    case 242: CTorso = 6; break;
                    case 243: CTorso = 6; break;
                    case 244: CTorso = 9; break;
                    case 245: CTorso = 14; break;
                    case 246: CTorso = 14; break;
                    case 247: CTorso = 4; break;
                    case 248: CTorso = 5; break;
                    case 249: CTorso = 0; break;
                    case 250: CTorso = 0; break;
                    case 251: CTorso = 3; break;
                    case 252: CTorso = 5; break;
                    case 253: CTorso = 1; break;
                    case 254: CTorso = 8; break;
                    case 255: CTorso = 11; break;
                    case 256: CTorso = 9; break;
                    case 257: CTorso = 6; break;
                    case 258: CTorso = 0; break;
                    case 259: CTorso = 3; break;
                    case 260: CTorso = 4; break;
                    case 261: CTorso = 3; break;
                    case 262: CTorso = 7; break;
                    case 349: CTorso = 44; break;
                }
            }

            player.EmitLocked("Client:SpawnArea:setCharClothes", 3, CTorso, 0);
        }

        [AsyncClientEvent("Server:ClothesShop:RequestCurrentSkin")]
        public static async Task SetCharacterCorrectClothes(IPlayer player)
        {
            if (player == null || !player.Exists) return;
            int charid = User.GetPlayerOnline(player);
            if (charid == 0) return;
            bool gender = GetCharacterGender(charid);

            if (GetCharacterBackpack(charid) == 0) SetCharacterBackpack(player, "None");
            else if (GetCharacterBackpack(charid) == 31) SetCharacterBackpack(player, "Rucksack");
            else if (GetCharacterBackpack(charid) == 45) SetCharacterBackpack(player, "Tasche");

            if (GetCharacterClothes(charid, "Top") == -2) player.EmitLocked("Client:SpawnArea:setCharClothes", 11, 15, 0);
            else player.EmitLocked("Client:SpawnArea:setCharClothes", 11, ServerClothes.GetClothesDraw(GetCharacterClothes(charid, "Top"), Convert.ToInt32(gender)), ServerClothes.GetClothesTexture(GetCharacterClothes(charid, "Top"), Convert.ToInt32(gender)));

            if (GetCharacterClothes(charid, "Torso") == -2) player.EmitLocked("Client:SpawnArea:setCharClothes", 3, 15, 0);
            else player.EmitLocked("Client:SpawnArea:setCharClothes", 3, ServerClothes.GetClothesDraw(GetCharacterClothes(charid, "Torso"), Convert.ToInt32(gender)), ServerClothes.GetClothesTexture(GetCharacterClothes(charid, "Torso"), Convert.ToInt32(gender)));

            if (GetCharacterClothes(charid, "Leg") == -2)
            {
                if (gender) player.EmitLocked("Client:SpawnArea:setCharClothes", 4, 15, 0);
                else player.EmitLocked("Client:SpawnArea:setCharClothes", 4, 21, 0);
            }
            else player.EmitLocked("Client:SpawnArea:setCharClothes", 4, ServerClothes.GetClothesDraw(GetCharacterClothes(charid, "Leg"), Convert.ToInt32(gender)), ServerClothes.GetClothesTexture(GetCharacterClothes(charid, "Leg"), Convert.ToInt32(gender)));

            if (GetCharacterClothes(charid, "Feet") == -2)
            {
                if (gender) player.EmitLocked("Client:SpawnArea:setCharClothes", 6, 35, 0);
                else player.EmitLocked("Client:SpawnArea:setCharClothes", 6, 34, 0);
            }
            else player.EmitLocked("Client:SpawnArea:setCharClothes", 6, ServerClothes.GetClothesDraw(GetCharacterClothes(charid, "Feet"), Convert.ToInt32(gender)), ServerClothes.GetClothesTexture(GetCharacterClothes(charid, "Feet"), Convert.ToInt32(gender)));

            if (GetCharacterClothes(charid, "Mask") == -2) player.EmitLocked("Client:SpawnArea:setCharClothes", 1, 0, 0);
            else player.EmitLocked("Client:SpawnArea:setCharClothes", 1, ServerClothes.GetClothesDraw(GetCharacterClothes(charid, "Mask"), Convert.ToInt32(gender)), ServerClothes.GetClothesTexture(GetCharacterClothes(charid, "Mask"), Convert.ToInt32(gender)));

            if (GetCharacterClothes(charid, "Necklace") == -2) player.EmitLocked("Client:SpawnArea:setCharClothes", 7, 0, 0);
            else player.EmitLocked("Client:SpawnArea:setCharClothes", 7, ServerClothes.GetClothesDraw(GetCharacterClothes(charid, "Necklace"), Convert.ToInt32(gender)), ServerClothes.GetClothesTexture(GetCharacterClothes(charid, "Necklace"), Convert.ToInt32(gender)));

            if (GetCharacterClothes(charid, "Armor") == -2) player.EmitLocked("Client:SpawnArea:setCharClothes", 9, 0, 0);
            else player.EmitLocked("Client:SpawnArea:setCharClothes", 9, ServerClothes.GetClothesDraw(GetCharacterClothes(charid, "Armor"), Convert.ToInt32(gender)), ServerClothes.GetClothesTexture(GetCharacterClothes(charid, "Armor"), Convert.ToInt32(gender)));

            if (GetCharacterClothes(charid, "Hat") == -2) player.EmitLocked("Client:SpawnArea:clearCharAccessory", 0);
            else player.EmitLocked("Client:SpawnArea:setCharAccessory", 0, ServerClothes.GetClothesDraw(GetCharacterClothes(charid, "Hat"), Convert.ToInt32(gender)), ServerClothes.GetClothesTexture(GetCharacterClothes(charid, "Hat"), Convert.ToInt32(gender)));

            if (GetCharacterClothes(charid, "Glass") == -2) player.EmitLocked("Client:SpawnArea:clearCharAccessory", 1);
            else player.EmitLocked("Client:SpawnArea:setCharAccessory", 1, ServerClothes.GetClothesDraw(GetCharacterClothes(charid, "Glass"), Convert.ToInt32(gender)), ServerClothes.GetClothesTexture(GetCharacterClothes(charid, "Glass"), Convert.ToInt32(gender)));

            if (GetCharacterClothes(charid, "Earring") == -2) player.EmitLocked("Client:SpawnArea:clearCharAccessory", 2);
            else player.EmitLocked("Client:SpawnArea:setCharAccessory", 2, ServerClothes.GetClothesDraw(GetCharacterClothes(charid, "Earring"), Convert.ToInt32(gender)), ServerClothes.GetClothesTexture(GetCharacterClothes(charid, "Earring"), Convert.ToInt32(gender)));

            if (GetCharacterClothes(charid, "Watch") == -2) player.EmitLocked("Client:SpawnArea:clearCharAccessory", 6);
            else player.EmitLocked("Client:SpawnArea:setCharAccessory", 6, ServerClothes.GetClothesDraw(GetCharacterClothes(charid, "Watch"), Convert.ToInt32(gender)), ServerClothes.GetClothesTexture(GetCharacterClothes(charid, "Watch"), Convert.ToInt32(gender)));

            if (GetCharacterClothes(charid, "Bracelet") == -2) player.EmitLocked("Client:SpawnArea:clearCharAccessory", 7);
            else player.EmitLocked("Client:SpawnArea:setCharAccessory", 7, ServerClothes.GetClothesDraw(GetCharacterClothes(charid, "Bracelet"), Convert.ToInt32(gender)), ServerClothes.GetClothesTexture(GetCharacterClothes(charid, "Bracelet"), Convert.ToInt32(gender)));

            if (GetCharacterClothes(charid, "Undershirt") == -2)
            {
                if (!gender) player.EmitLocked("Client:SpawnArea:setCharClothes", 8, 57, 0);
                else if (gender) player.EmitLocked("Client:SpawnArea:setCharClothes", 8, 34, 0);
            }
            else player.EmitLocked("Client:SpawnArea:setCharClothes", 8, ServerClothes.GetClothesDraw(GetCharacterClothes(charid, "Undershirt"), Convert.ToInt32(gender)), ServerClothes.GetClothesTexture(GetCharacterClothes(charid, "Undershirt"), Convert.ToInt32(gender)));

            if (GetCharacterClothes(charid, "Decal") == -2) player.EmitLocked("Client:SpawnArea:setCharClothes", 10, 0, 0);
            else player.EmitLocked("Client:SpawnArea:setCharClothes", 10, ServerClothes.GetClothesDraw(GetCharacterClothes(charid, "Decal"), Convert.ToInt32(gender)), ServerClothes.GetClothesTexture(GetCharacterClothes(charid, "Decal"), Convert.ToInt32(gender)));

            string primaryWeapon = (string)GetCharacterWeapon(player, "PrimaryWeapon");
            int primaryAmmo = (int)GetCharacterWeapon(player, "PrimaryAmmo");
            string SecWeapon = (string)GetCharacterWeapon(player, "SecondaryWeapon");
            int SecAmmo = (int)GetCharacterWeapon(player, "SecondaryAmmo");
            string Sec2Weapon = (string)GetCharacterWeapon(player, "SecondaryWeapon2");
            int Sec2Ammo = (int)GetCharacterWeapon(player, "SecondaryAmmo2");
            string FistWeapon = (string)GetCharacterWeapon(player, "FistWeapon");
            if (primaryWeapon != "None") { player.GiveWeapon(WeaponHandler.GetWeaponModelByName(primaryWeapon), primaryAmmo, false); }
            if (SecWeapon != "None") { player.GiveWeapon(WeaponHandler.GetWeaponModelByName(SecWeapon), SecAmmo, false); }
            if (Sec2Weapon != "None") { player.GiveWeapon(WeaponHandler.GetWeaponModelByName(Sec2Weapon), Sec2Ammo, false); }
            if (FistWeapon != "None") { player.GiveWeapon(WeaponHandler.GetWeaponModelByName(FistWeapon), 1, false); }
        }

        /*public static void SwitchCharacterClothesItem(IPlayer player, string ClothesName, string Type)
        {
            if (player == null || !player.Exists) return;
            bool ClothesGender = false;
            int charid = User.GetPlayerOnline(player);
            int clothesId = 0;
            if (charid == 0) return;
            if (ClothesName.Contains("-W-")) { ClothesGender = true; } 
            else if(ClothesName.Contains("-M-")) { ClothesGender = false; }
            else { ClothesGender = GetCharacterGender(charid); }

            if (GetCharacterClothes(charid, Type) == -2)
            {
                if (ClothesGender == GetCharacterGender(charid))
                {
                    SetCharacterClothes(charid, Type, ClothesName);
                    if (Type == "Top")
                    {
                        clothesId = 11;
                        var sItem = ServerItems.ServerItems_.FirstOrDefault(i => i.itemName == ClothesName);
                        if (sItem != null)
                        {
                            player.EmitLocked("Client:SpawnArea:setCharClothes", clothesId, sItem.ClothesDraw, sItem.ClothesTexture);
                            player.EmitLocked("Client:SpawnArea:setCharClothes", 10, sItem.ClothesDecals, sItem.ClothesDecalsTexture);
                            if (sItem.ClothesUndershirt != 0) { player.EmitLocked("Client:SpawnArea:setCharClothes", 8, sItem.ClothesUndershirt, sItem.ClothesUndershirtTexture); }
                            else
                            {
                                if (ClothesGender == false) { player.EmitLocked("Client:SpawnArea:setCharClothes", 8, 57, 0); }
                                else { player.EmitLocked("Client:SpawnArea:setCharClothes", 8, 2, 0); }
                            }
                        }
                    }
                    else if (Type == "Leg") {
                        clothesId = 4; 
                        player.EmitLocked("Client:SpawnArea:setCharClothes", clothesId, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesDraw, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesTexture);
                    }
                    else if (Type == "Feet") { clothesId = 6; player.EmitLocked("Client:SpawnArea:setCharClothes", clothesId, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesDraw, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesTexture); }
                    else if (Type == "Mask") { clothesId = 1; player.EmitLocked("Client:SpawnArea:setCharClothes", clothesId, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesDraw, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesTexture); }
                    else if (Type == "Necklace") { clothesId = 7; player.EmitLocked("Client:SpawnArea:setCharClothes", clothesId, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesDraw, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesTexture); }
                    else if (Type == "Undershirt") { clothesId = 8; player.EmitLocked("Client:SpawnArea:setCharClothes", clothesId, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesDraw, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesTexture); }
                    else if (Type == "Armor") { clothesId = 9; player.EmitLocked("Client:SpawnArea:setCharClothes", clothesId, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesDraw, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesTexture); }
                    else if (Type == "Hat") { clothesId = 0; player.EmitLocked("Client:SpawnArea:setCharAccessory", clothesId, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesDraw, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesTexture); }
                    else if (Type == "Glass") { clothesId = 1; player.EmitLocked("Client:SpawnArea:setCharAccessory", clothesId, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesDraw, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesTexture); }
                    else if (Type == "Earring") { clothesId = 2; player.EmitLocked("Client:SpawnArea:setCharAccessory", clothesId, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesDraw, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesTexture); }
                    else if (Type == "Watch") { clothesId = 6; player.EmitLocked("Client:SpawnArea:setCharAccessory", clothesId, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesDraw, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesTexture); }
                    else if (Type == "Bracelet") { clothesId = 7; player.EmitLocked("Client:SpawnArea:setCharAccessory", clothesId, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesDraw, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesTexture); }
                    if (Type == "Top") { SetCharacterCorrectTorso(player, ServerItems.ServerItems_.First(x => x.itemName == ClothesName).ClothesDraw); }
                    if (ClothesName.Contains("-M-")) { ClothesName = ClothesName.Replace("-M-", "♂"); }
                    else if (ClothesName.Contains("-W")) { ClothesName = ClothesName.Replace("-W-", "♀"); }
                    HUDHandler.SendNotification(player, 1, 5000, $"Du hast {ClothesName} angezogen!");
                }
                else { HUDHandler.SendNotification(player, 4, 5000, $"Dieses Kleidungsstück passt dir nicht ({ClothesName})."); }
            }
            else if(GetCharacterClothes(charid, Type) == ClothesName)
            {
                SetCharacterClothes(charid, Type, "None");
                if (Type == "Top")
                {
                    clothesId = 11;
                    player.EmitLocked("Client:SpawnArea:setCharClothes", clothesId, 15, 0);
                    SetCharacterCorrectTorso(player, 15);
                    if (ClothesGender == false) { player.EmitLocked("Client:SpawnArea:setCharClothes", 8, 57, 0); }
                    else { player.EmitLocked("Client:SpawnArea:setCharClothes", 8, 2, 0); }
                }
                else if (Type == "Leg")
                {
                    clothesId = 4;
                    if (ClothesGender == false) { player.EmitLocked("Client:SpawnArea:setCharClothes", clothesId, 21, 0); }
                    else { player.EmitLocked("Client:SpawnArea:setCharClothes", clothesId, 15, 0); }
                }
                else if (Type == "Feet")
                {
                    clothesId = 6;
                    if (ClothesGender == false) { player.EmitLocked("Client:SpawnArea:setCharClothes", clothesId, 34, 0); }
                    else { player.EmitLocked("Client:SpawnArea:setCharClothes", clothesId, 35, 0); }
                }
                else if (Type == "Mask")
                {
                    clothesId = 1;
                    player.EmitLocked("Client:SpawnArea:setCharClothes", clothesId, 0, 0);
                }
                else if(Type == "Necklace")
                {
                    clothesId = 7;
                    player.EmitLocked("Client:SpawnArea:setCharClothes", clothesId, 0, 0);
                }
                else if(Type == "Undershirt")
                {
                    clothesId = 8;
                    if(ClothesGender == false) { player.EmitLocked("Client:SpawnArea:setCharClothes", clothesId, 57, 0); }
                    else { player.EmitLocked("Client:SpawnArea:setCharClothes", clothesId, 34, 0); }
                }
                else if(Type == "Armor")
                {
                    clothesId = 9;
                    player.EmitLocked("Client:SpawnArea:setCharClothes", clothesId, 0, 0);
                }
                else if(Type == "Hat")
                {
                    clothesId = 0;
                    player.EmitLocked("Client:SpawnArea:clearCharAccessory", clothesId);
                }
                else if(Type == "Glass")
                {
                    clothesId = 1;
                    player.EmitLocked("Client:SpawnArea:clearCharAccessory", 1);
                }
                else if(Type == "Earring")
                {
                    clothesId = 2;
                    player.EmitLocked("Client:SpawnArea:clearCharAccessory", 2);
                }
                else if(Type == "Watch")
                {
                    clothesId = 6;
                    player.EmitLocked("Client:SpawnArea:clearCharAccessory", 6);
                }
                else if(Type == "Bracelet")
                {
                    clothesId = 7;
                    player.EmitLocked("Client:SpawnArea:clearCharAccessory", 7);
                }

                if(ClothesName.Contains("-M-")) { ClothesName = ClothesName.Replace("-M-", "♂"); }
                else if(ClothesName.Contains("-W-")) { ClothesName = ClothesName.Replace("-W-", "♀"); }
                HUDHandler.SendNotification(player, 1, 5000, $"Du hast {ClothesName} ausgezogen.");
            }
            else if(GetCharacterClothes(charid, Type) != ClothesName && GetCharacterClothes(charid, Type) != "None")
            {
                string clothesTypeStr = "";
                if(Type == "Top") { clothesTypeStr = "deinen Oberkörper"; }
                else if(Type == "Leg") { clothesTypeStr = "deine Beine"; }
                else if(Type == "Feet") { clothesTypeStr = "deine Füße"; }
                else if(Type == "Mask") { clothesTypeStr = "dein Gesicht"; }
                else if(Type == "Necklace") { clothesTypeStr = "deinen Hals"; }
                else if(Type == "Undershirt") { clothesTypeStr = "dein Unterhemd"; }
                else if(Type == "Hat") { clothesTypeStr = "dein Kopf"; }
                else if(Type == "Glass") { clothesTypeStr = "deine Augen"; }
                else if(Type == "Earring") { clothesTypeStr = "deine Ohren"; }
                else if(Type == "Watch") { clothesTypeStr = "deinen Unterarm"; }
                else if(Type == "Bracelet") { clothesTypeStr = "deinen Unterarm"; }
                HUDHandler.SendNotification(player, 3, 5000, $"Du musst vorher {clothesTypeStr} freimachen.");
            }
        }*/

        [AsyncClientEvent("Server:ClothesStorage:setCharacterClothes")]
        public static async Task SwitchCharacterClothes(ClassicPlayer player, int clothId, bool isProp)
        {
            try
            {
                if (player == null || !player.Exists || player.CharacterId <= 0) return;
                bool gender = GetCharacterGender(player.CharacterId);

                SetCharacterClothes(player, player.CharacterId, clothId, isProp);
                if (!isProp) player.EmitLocked("Client:SpawnArea:setCharClothes", ServerClothes.GetClothesComponent(clothId, Convert.ToInt32(gender)), ServerClothes.GetClothesDraw(clothId, Convert.ToInt32(gender)), ServerClothes.GetClothesTexture(clothId, Convert.ToInt32(gender)));
                else player.EmitLocked("Client:SpawnArea:setCharAccessory", ServerClothes.GetClothesComponent(clothId, Convert.ToInt32(gender)), ServerClothes.GetClothesDraw(clothId, Convert.ToInt32(gender)), ServerClothes.GetClothesTexture(clothId, Convert.ToInt32(gender)));
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void SetCharacterClothes(ClassicPlayer player, int charid, int clothId, bool isProp)
        {
            if (charid <= 0) return;
            var chars = CharactersSkin.FirstOrDefault(p => p.charId == charid);
            if (chars != null)
            {
                if (!isProp)
                {
                    switch (ServerClothes.GetClothesComponent(clothId, Convert.ToInt32(GetCharacterGender(player.CharacterId))))
                    {
                        case 11:
                            chars.clothesTop = clothId;
                            break;
                        case 3:
                            chars.clothesTorso = clothId;
                            break;
                        case 4:
                            chars.clothesLeg = clothId;
                            break;
                        case 6:
                            chars.clothesFeet = clothId;
                            break;
                        case 7:
                            chars.clothesNecklace = clothId;
                            break;
                        case 1:
                            chars.clothesMask = clothId;
                            break;
                        case 9:
                            chars.clothesArmor = clothId;
                            break;
                        case 8:
                            chars.clothesUndershirt = clothId;
                            break;
                        case 10:
                            chars.clothesDecal = clothId;
                            break;

                    }
                }
                else
                {
                    switch (ServerClothes.GetClothesComponent(clothId, Convert.ToInt32(GetCharacterGender(player.CharacterId))))
                    {
                        case 0:
                            chars.clothesHat = clothId;
                            break;
                        case 1:
                            chars.clothesGlass = clothId;
                            break;
                        case 7:
                            chars.clothesBracelet = clothId;
                            break;
                        case 6:
                            chars.clothesWatch = clothId;
                            break;
                        case 2:
                            chars.clothesEarring = clothId;
                            break;
                    }
                }


                try
                {
                    using (gtaContext db = new gtaContext())
                    {
                        db.Characters_Skin.Update(chars);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Alt.Log($"{e}");
                }
            }
        }

        public static bool IsCharacterUnconscious(int charId)
        {
            try
            {
                if (charId <= 0) return false;
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars != null) return chars.isUnconscious;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static void SetCharacterUnconscious(int charId, bool isUnconscious, int unconsciousTime)
        {
            try
            {
                if (charId <= 0) return;
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars != null)
                {
                    chars.isUnconscious = isUnconscious;
                    chars.unconsciousTime = unconsciousTime;
                    using (gtaContext db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static int GetCharacterUnconsciousTime(int charId)
        {
            try
            {
                if (charId <= 0) return 0;
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars != null) return chars.unconsciousTime;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static bool IsCharacterFastFarm(int charId)
        {
            try
            {
                if (charId <= 0) return false;
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars != null) return chars.isFastFarm;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        /// <summary>
        /// Setzt Character für eine bestimmbare Zeit auf doppelte Aufsammelmenge
        /// </summary>
        /// <param name="charId"></param>
        /// <param name="isFastFarm"></param>
        /// <param name="fastFarmTime"></param>
        public static void SetCharacterFastFarm(int charId, bool isFastFarm, int fastFarmTime)
        {
            try
            {
                if (charId <= 0) return;
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars != null)
                {
                    chars.isFastFarm = isFastFarm;
                    chars.fastFarmTime = fastFarmTime;
                    using (gtaContext db = new gtaContext())
                    {
                        db.AccountsCharacters.Update(chars);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static int GetCharacterFastFarmTime(int charId)
        {
            try
            {
                if (charId <= 0) return 0;
                var chars = PlayerCharacters.FirstOrDefault(x => x.charId == charId);
                if (chars != null) return chars.fastFarmTime;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }


    }
}
