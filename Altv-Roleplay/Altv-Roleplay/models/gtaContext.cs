using AltV.Net.Data;
using AltV.Net;
using Altv_Roleplay.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Numerics;

namespace Altv_Roleplay.models
{
    public partial class gtaContext : DbContext
    {
        public gtaContext() { }
        public gtaContext(DbContextOptions<gtaContext> options) : base(options) { }
        public virtual DbSet<Accounts> Accounts { get; set; }
        public virtual DbSet<AccountsCharacters> AccountsCharacters { get; set; }
        public virtual DbSet<Characters_Bank> Characters_Bank { get; set; }
        public virtual DbSet<Characters_Skin> Characters_Skin { get; set; }
        public virtual DbSet<Characters_LastPos> Characters_LastPos { get; set; }
        public virtual DbSet<Characters_Licenses> Characters_Licenses { get; set; }
        public virtual DbSet<Characters_Minijobs> Characters_Minijobs { get; set; }
        public virtual DbSet<Server_Faction_Labor_Items> Server_Faction_Labor_Items { get; set; }
        public virtual DbSet<Characters_Permissions> Characters_Permissions { get; set; }
        public virtual DbSet<Server_Storages> Server_Storages { get; set; }
        public virtual DbSet<Characters_Inventory> Characters_Inventory { get; set; }
        public virtual DbSet<Characters_Wanteds> Characters_Wanteds { get; set; }
        public virtual DbSet<Characters_Tablet_Apps> Characters_Tablet_Apps { get; set; }
        public virtual DbSet<Characters_Tablet_Tutorial> Characters_Tablet_Tutorials { get; set; }
        public virtual DbSet<CharactersPhoneChats> CharactersPhoneChats { get; set; }
        public virtual DbSet<CharactersPhoneVerlauf> CharactersPhoneVerlauf { get; set; }
        public virtual DbSet<CharactersPhoneChatMessages> CharactersPhoneChatMessages { get; set; }
        public virtual DbSet<CharactersPhoneContacts> CharactersPhoneContacts { get; set; }
        public virtual DbSet<CharactersOwnedClothes> CharactersOwnedClothes { get; set; }
        public virtual DbSet<Characters_Tattoos> Characters_Tattoos { get; set; }
        public virtual DbSet<Server_All_Vehicles> Server_All_Vehicles { get; set; }
        public virtual DbSet<Server_Animations> Server_Animations { get; set; }
        public virtual DbSet<Server_ATM> Server_ATM { get; set; }
        public virtual DbSet<Server_Banks> Server_Banks { get; set; }
        public virtual DbSet<Server_Bank_Paper> Server_Bank_Paper { get; set; }
        public virtual DbSet<Server_Blips> Server_Blips { get; set; }
        public virtual DbSet<Server_Barbers> Server_Barbers { get; set; }
        public virtual DbSet<Server_Companys> Server_Companys { get; set; }
        public virtual DbSet<Server_Company_Members> Server_Company_Members { get; set; }
        public virtual DbSet<Server_Clothes_Shops> Server_Clothes_Shops { get; set; }
        public virtual DbSet<Server_Clothes_Shops_Items> Server_Clothes_Shops_Items { get; set; }
        public virtual DbSet<Server_Doors> Server_Doors { get; set; }
        public virtual DbSet<Server_Factions> Server_Factions { get; set; }
        public virtual DbSet<Server_Faction_Clothes> Server_Faction_Clothes { get; set; }
        public virtual DbSet<Server_Faction_Members> Server_Faction_Members { get; set; }
        public virtual DbSet<Server_Faction_Ranks> Server_Faction_Ranks { get; set; }
        public virtual DbSet<Server_Faction_Storage_Items> Server_Faction_Storage_Items { get; set; }
        public virtual DbSet<Server_Faction_Positions> Server_Faction_Positions { get; set; }
        public virtual DbSet<Server_Farming_Producer> Server_Farming_Producer { get; set; }
        public virtual DbSet<Server_Farming_Spots> Server_Farming_Spots { get; set; }
        public virtual DbSet<Server_Fuel_Stations> Server_Fuel_Stations { get; set; }
        public virtual DbSet<Server_Fuel_Spots> Server_Fuel_Spots { get; set; }
        public virtual DbSet<Server_Garages> Server_Garages { get; set; }
        public virtual DbSet<Server_Garage_Slots> Server_Garage_Slots { get; set; }
        public virtual DbSet<Server_Hotels> Server_Hotels { get; set; }
        public virtual DbSet<Server_Hotels_Apartments> Server_Hotels_Apartments { get; set; }
        public virtual DbSet<Server_Hotels_Storage> Server_Hotels_Storages { get; set; }
        public virtual DbSet<Server_Houses_Interiors> Server_Houses_Interiors { get; set; }
        public virtual DbSet<Server_Houses_Storage> Server_Houses_Storages { get; set; }
        public virtual DbSet<Server_Houses_Renter> Server_Houses_Renters { get; set; }
        public virtual DbSet<Server_Houses> Server_Houses { get; set; }
        public virtual DbSet<Server_Items> Server_Items { get; set; }
        public virtual DbSet<Server_Jobs> Server_Jobs { get; set; }
        public virtual DbSet<Server_Licenses> Server_Licenses { get; set; }
        public virtual DbSet<Server_Markers> Server_Markers { get; set; }
        public virtual DbSet<Server_Minijob_Busdriver_Routes> Server_Minijob_Busdriver_Routes { get; set; }
        public virtual DbSet<Server_Minijob_Busdriver_Spots> Server_Minijob_Busdriver_Spots { get; set; }
        public virtual DbSet<Server_Minijob_Garbage_Spots> Server_Minijob_Garbage_Spots { get; set; }
        public virtual DbSet<Server_Peds> Server_Peds { get; set; }
        public virtual DbSet<Server_Shops> Server_Shops { get; set; }
        public virtual DbSet<Server_Shops_Items> Server_Shops_Items { get; set; }
        public virtual DbSet<Server_Tablet_Apps> Server_Tablet_Apps { get; set; }
        public virtual DbSet<Server_Tablet_Events> Server_Tablet_Events { get; set; }
        public virtual DbSet<Server_Tablet_Notes> Server_Tablet_Notes { get; set; }
        public virtual DbSet<Server_Teleports> Server_Teleports { get; set; }
        public virtual DbSet<Server_Vehicles> Server_Vehicles { get; set; }
        public virtual DbSet<Server_Vehicles_Mod> Server_Vehicles_Mods { get; set; }
        public virtual DbSet<Server_Vehicle_Items> Server_Vehicle_Items { get; set; }
        public virtual DbSet<Server_Vehicle_Shops> Server_Vehicle_Shops { get; set; }
        public virtual DbSet<Server_Vehicle_Shops_Items> Server_Vehicle_Shops_Items { get; set; }
        public virtual DbSet<Server_Wanteds> Server_Wanteds { get; set; }
        public virtual DbSet<LogsLogin> LogsLogin { get; set; }
        public virtual DbSet<Logs_Company> LogsCompany { get; set; }
        public virtual DbSet<Logs_Faction> LogsFaction { get; set; }
        public virtual DbSet<Server_Tattoos> Server_Tattoos { get; set; }
        public virtual DbSet<Server_Tattoo_Shops> Server_Tattoo_Shops { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //Lokal
                string connectionStr = $"server={Constants.DatabaseConfig.Host};port={Constants.DatabaseConfig.Port};user={Constants.DatabaseConfig.User};password={Constants.DatabaseConfig.Password};database={Constants.DatabaseConfig.Database}";
                optionsBuilder.UseMySql(connectionStr, ServerVersion.AutoDetect(connectionStr));
                optionsBuilder.EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Accounts>(entity =>
            {
                entity.HasKey(e => e.playerid);
                entity.ToTable("accounts", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.playerid).HasName("id");
                entity.Property(e => e.playerid).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.playerName).IsRequired().HasColumnName("username").HasMaxLength(64).IsUnicode(false).HasDefaultValueSql("N/A");
                entity.Property(e => e.Email).IsRequired().HasColumnName("email").HasMaxLength(64);
                entity.Property(e => e.socialClub).IsRequired().HasColumnName("socialid").HasMaxLength(64).IsUnicode(false);
                entity.Property(e => e.hardwareId).IsRequired().HasColumnName("hwid").HasMaxLength(255).IsUnicode(false);
                entity.Property(e => e.password).IsRequired().HasColumnName("password").HasMaxLength(256);
                entity.Property(e => e.Online).HasColumnName("online").HasColumnType("int(1)").HasDefaultValueSql("0");
                entity.Property(e => e.whitelisted).HasColumnName("whitelisted");
                entity.Property(e => e.ban).HasColumnName("ban");
                entity.Property(e => e.banReason).HasColumnName("banReason").HasMaxLength(128);
                entity.Property(e => e.adminLevel).HasColumnName("adminlevel").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Characters_Tattoos>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("characters_tattoos", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charId").HasColumnType("int(11)");
                entity.Property(e => e.tattooId).HasColumnName("tattooId").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Server_Tattoos>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_tattoos", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.collection).HasColumnName("collection").HasMaxLength(64);
                entity.Property(e => e.nameHash).HasColumnName("nameHash").HasMaxLength(64);
                entity.Property(e => e.name).HasColumnName("name").HasMaxLength(64);
                entity.Property(e => e.part).HasColumnName("part").HasMaxLength(64);
                entity.Property(e => e.price).HasColumnName("price").HasColumnType("int(11)");
                entity.Property(e => e.gender).HasColumnName("gender").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Server_Tattoo_Shops>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_tattooshops", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.name).HasColumnName("name");
                entity.Property(e => e.owner).HasColumnName("owner").HasColumnType("int(11)");
                entity.Property(e => e.bank).HasColumnName("bank").HasColumnType("int(11)");
                entity.Property(e => e.price).HasColumnName("price").HasColumnType("int(11)");
                entity.Property(e => e.pedX).HasColumnName("pedX");
                entity.Property(e => e.pedY).HasColumnName("pedY");
                entity.Property(e => e.pedZ).HasColumnName("pedZ");
                entity.Property(e => e.pedModel).HasColumnName("pedModel");
                entity.Property(e => e.pedRot).HasColumnName("pedRot");
            });

            modelBuilder.Entity<AccountsCharacters>(entity =>
            {
                entity.HasKey(e => e.charId);
                entity.ToTable("accounts_characters", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.charId).HasName("id");
                entity.Property(e => e.charId).HasColumnName("charId").HasColumnType("int(11)");
                entity.Property(e => e.accountId).IsRequired().HasColumnName("accountId").HasColumnType("int(11)");
                entity.Property(e => e.charname).IsRequired().HasColumnName("charname").HasMaxLength(35);
                entity.Property(e => e.death).IsRequired().HasColumnName("death");
                entity.Property(e => e.accState).HasColumnName("accState");
                entity.Property(e => e.firstJoin).IsRequired().HasColumnName("firstjoin");
                entity.Property(e => e.firstSpawnPlace).IsRequired().HasColumnName("firstspawnplace").HasMaxLength(32);
                entity.Property(e => e.firstJoinTimestamp).HasColumnName("firstJoinTimestamp");
                entity.Property(e => e.gender).IsRequired().HasColumnName("gender");
                entity.Property(e => e.birthdate).IsRequired().HasColumnName("birthdate");
                entity.Property(e => e.birthplace).HasColumnName("birthplace");
                entity.Property(e => e.health).HasColumnName("health");
                entity.Property(e => e.armor).HasColumnName("armor");
                entity.Property(e => e.hunger).HasColumnName("hunger");
                entity.Property(e => e.thirst).HasColumnName("thirst");
                entity.Property(e => e.address).HasColumnName("address");
                entity.Property(e => e.phonenumber).HasColumnName("phonenumber");
                entity.Property(e => e.isCrime).HasColumnName("isCrime");
                entity.Property(e => e.paydayTime).HasColumnName("paydayTime");
                entity.Property(e => e.job).HasColumnName("job").HasMaxLength(64);
                entity.Property(e => e.jobHourCounter).HasColumnName("jobHourCounter").HasColumnType("int(11)");
                entity.Property(e => e.lastJobPaycheck).HasColumnName("lastJobPaycheck");
                entity.Property(e => e.weapon_Primary).HasColumnName("weapon_primary");
                entity.Property(e => e.weapon_Primary_Ammo).HasColumnName("weapon_primary_ammo");
                entity.Property(e => e.weapon_Secondary).HasColumnName("weapon_secondary");
                entity.Property(e => e.weapon_Secondary_Ammo).HasColumnName("weapon_secondary_ammo");
                entity.Property(e => e.weapon_Secondary2).HasColumnName("weapon_secondary2");
                entity.Property(e => e.weapon_Secondary2_Ammo).HasColumnName("weapon_secondary2_ammo");
                entity.Property(e => e.weapon_Fist).HasColumnName("weapon_fist");
                entity.Property(e => e.weapon_Fist_Ammo).HasColumnName("weapon_fist_ammo");
                entity.Property(e => e.isUnconscious).HasColumnName("isUnconscious");
                entity.Property(e => e.unconsciousTime).HasColumnName("unconsciousTime");
                entity.Property(e => e.isFastFarm).HasColumnName("isFastFarm");
                entity.Property(e => e.fastFarmTime).HasColumnName("fastFarmTime");
                entity.Property(e => e.lastLogin).HasColumnName("lastLogin");
                entity.Property(e => e.isPhoneEquipped).HasColumnName("isPhoneEquipped");
                entity.Property(e => e.playtimeHours).HasColumnName("playtimeHours");
                entity.Property(e => e.isInJail).HasColumnName("isInJail");
                entity.Property(e => e.jailTime).HasColumnName("jailTime");
                entity.Property(e => e.pedName).HasColumnName("pedName").HasMaxLength(64);
                entity.Property(e => e.isAnimalPed).HasColumnName("isAnimalPed").HasColumnType("int(11)");
                entity.Property(e => e.isLaptopEquipped).HasColumnName("isLaptopEquipped");
            });

            modelBuilder.Entity<Characters_Bank>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("characters_bank", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charid").HasColumnType("int(11)");
                entity.Property(e => e.accountNumber).HasColumnName("accountnumber").HasColumnType("int(11)");
                entity.Property(e => e.money).HasColumnName("money");
                entity.Property(e => e.pin).HasColumnName("pin");
                entity.Property(e => e.mainAccount).HasColumnName("mainaccount");
                entity.Property(e => e.closed).HasColumnName("closed");
                entity.Property(e => e.pinTrys).HasColumnName("pinTrys");
                entity.Property(e => e.createZone).HasColumnName("createZone");
            });

            modelBuilder.Entity<Characters_LastPos>(entity =>
            {
                entity.HasKey(e => e.charId);
                entity.ToTable("characters_lastpos", Constants.DatabaseConfig.Database);
                entity.Property(e => e.charId).HasColumnName("charid").HasColumnType("int(11)");
                entity.Property(e => e.lastPosX).HasColumnName("lastPosX");
                entity.Property(e => e.lastPosY).HasColumnName("lastPosY");
                entity.Property(e => e.lastPosZ).HasColumnName("lastPosZ");
                entity.Property(e => e.lastDimension).HasColumnName("lastDimension");
            });

            modelBuilder.Entity<Characters_Licenses>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("characters_licenses", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charId").HasColumnType("int(11)");
                entity.Property(e => e.PKW).HasColumnName("pkw");
                entity.Property(e => e.LKW).HasColumnName("lkw");
                entity.Property(e => e.Bike).HasColumnName("bike");
                entity.Property(e => e.Boat).HasColumnName("boat");
                entity.Property(e => e.Fly).HasColumnName("fly");
                entity.Property(e => e.Helicopter).HasColumnName("helicopter");
                entity.Property(e => e.PassengerTransport).HasColumnName("passengertransport");
                entity.Property(e => e.weaponlicense).HasColumnName("weaponlicense");
            });

            modelBuilder.Entity<Characters_Minijobs>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("characters_minijobs", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charId").HasColumnType("int(11)");
                entity.Property(e => e.jobName).HasColumnName("jobName").HasMaxLength(64);
                entity.Property(e => e.exp).HasColumnName("exp").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Characters_Permissions>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("characters_permissions", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charId").HasColumnType("int(11)");
                entity.Property(e => e.permissionName).HasColumnName("permissionName").HasMaxLength(64);
            });

            modelBuilder.Entity<Characters_Skin>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("characters_skin", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charId").HasColumnType("int(11)");
                entity.Property(e => e.facefeatures).HasColumnName("facefeatures");
                entity.Property(e => e.headblendsdata).HasColumnName("headblendsdata");
                entity.Property(e => e.headoverlays).HasColumnName("headoverlays");
                entity.Property(e => e.clothesTop).HasColumnName("clothesTop").HasColumnType("int(11)");
                entity.Property(e => e.clothesTorso).HasColumnName("clothesTorso").HasColumnType("int(11)");
                entity.Property(e => e.clothesLeg).HasColumnName("clothesLeg").HasColumnType("int(11)");
                entity.Property(e => e.clothesFeet).HasColumnName("clothesFeet").HasColumnType("int(11)");
                entity.Property(e => e.clothesHat).HasColumnName("clothesHat").HasColumnType("int(11)");
                entity.Property(e => e.clothesGlass).HasColumnName("clothesGlass").HasColumnType("int(11)");
                entity.Property(e => e.clothesEarring).HasColumnName("clothesEarring").HasColumnType("int(11)");
                entity.Property(e => e.clothesNecklace).HasColumnName("clothesNecklace").HasColumnType("int(11)");
                entity.Property(e => e.clothesMask).HasColumnName("clothesMask").HasColumnType("int(11)");
                entity.Property(e => e.clothesArmor).HasColumnName("clothesArmor").HasColumnType("int(11)");
                entity.Property(e => e.clothesUndershirt).HasColumnName("clothesUndershirt").HasColumnType("int(11)");
                entity.Property(e => e.clothesBracelet).HasColumnName("clothesBracelet").HasColumnType("int(11)");
                entity.Property(e => e.clothesWatch).HasColumnName("clothesWatch").HasColumnType("int(11)");
                entity.Property(e => e.clothesBag).HasColumnName("clothesBag").HasColumnType("int(11)");
                entity.Property(e => e.clothesDecal).HasColumnName("clothesDecal").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Characters_Inventory>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("characters_inventory", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charid").HasColumnType("int(11)");
                entity.Property(e => e.itemName).HasColumnName("itemName").HasMaxLength(128);
                entity.Property(e => e.itemAmount).HasColumnName("itemAmount").HasColumnType("int(11)");
                entity.Property(e => e.itemLocation).HasColumnName("itemLocation").HasMaxLength(32);
            });

            modelBuilder.Entity<Characters_Wanteds>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("characters_wanteds", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charId").HasColumnType("int(11)");
                entity.Property(e => e.wantedId).HasColumnName("wantedId").HasColumnType("int(11)");
                entity.Property(e => e.givenString).HasColumnName("givenString").HasMaxLength(64);
            });

            modelBuilder.Entity<Characters_Tablet_Apps>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("characters_tablet_apps", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charId").HasColumnType("int(11)");
                entity.Property(e => e.weather).HasColumnName("weather");
                entity.Property(e => e.news).HasColumnName("news");
                entity.Property(e => e.banking).HasColumnName("banking");
                entity.Property(e => e.lifeinvader).HasColumnName("lifeinvader");
                entity.Property(e => e.vehicles).HasColumnName("vehicles");
                entity.Property(e => e.events).HasColumnName("events");
                entity.Property(e => e.company).HasColumnName("company");
                entity.Property(e => e.notices).HasColumnName("notices");
            });

            modelBuilder.Entity<Characters_Tablet_Tutorial>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("characters_tablet_tutorial", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charId").HasColumnType("int(11)");
                entity.Property(e => e.openTablet).HasColumnName("openTablet");
                entity.Property(e => e.openInventory).HasColumnName("openInventory");
                entity.Property(e => e.createBankAccount).HasColumnName("createBankAccount");
                entity.Property(e => e.buyVehicle).HasColumnName("buyVehicle");
                entity.Property(e => e.useGarage).HasColumnName("useGarage");
                entity.Property(e => e.acceptJob).HasColumnName("acceptJob");
            });

            modelBuilder.Entity<CharactersPhoneChats>(entity =>
            {
                entity.HasKey(e => e.chatId);
                entity.ToTable("characters_phone_chats", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.chatId).HasName("chatId");
                entity.Property(e => e.chatId).HasColumnName("chatId").HasColumnType("int(11)");
                entity.Property(e => e.phoneNumber).HasColumnName("phoneNumber").HasColumnType("int(11)");
                entity.Property(e => e.anotherNumber).HasColumnName("anotherNumber").HasColumnType("int(11)");
            });

            modelBuilder.Entity<CharactersPhoneVerlauf>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("characters_phone_verlauf", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charId").HasColumnType("int(11)");
                entity.Property(e => e.phoneNumber).HasColumnName("phoneNumber").HasColumnType("int(11)");
                entity.Property(e => e.anotherNumber).HasColumnName("anotherNumber").HasColumnType("int(11)");
                entity.Property(e => e.date).HasColumnName("date");
            });

            modelBuilder.Entity<CharactersPhoneChatMessages>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("characters_phone_chatmessages", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.chatId).HasColumnName("chatId").HasColumnType("int(11)");
                entity.Property(e => e.fromNumber).HasColumnName("fromNumber").HasColumnType("int(11)");
                entity.Property(e => e.toNumber).HasColumnName("toNumber").HasColumnType("int(11)");
                entity.Property(e => e.unix).HasColumnName("unix").HasColumnType("int(11)");
                entity.Property(e => e.message).HasColumnName("message");
            });

            modelBuilder.Entity<CharactersPhoneContacts>(entity =>
            {
                entity.HasKey(e => e.contactId);
                entity.ToTable("characters_phone_contacts", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.contactId).HasName("contactId");
                entity.Property(e => e.contactId).HasColumnName("contactId").HasColumnType("int(11)");
                entity.Property(e => e.phoneNumber).HasColumnName("phoneNumber").HasColumnType("int(11)");
                entity.Property(e => e.contactName).HasColumnName("contactName");
                entity.Property(e => e.contactNumber).HasColumnName("contactNumber");
            });

            modelBuilder.Entity<CharactersOwnedClothes>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("characters_ownedclothes", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charId").HasColumnType("int(11)");
                entity.Property(e => e.clothId).HasColumnName("clothId").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Server_All_Vehicles>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_all_vehicles", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.category).HasColumnName("category").HasMaxLength(64);
                entity.Property(e => e.manufactor).HasColumnName("manufactor").HasMaxLength(64);
                entity.Property(e => e.name).HasColumnName("name").HasMaxLength(64);
                entity.Property(e => e.hash).HasColumnName("hash");
                entity.Property(e => e.price).HasColumnName("price").HasColumnType("int(11)");
                entity.Property(e => e.trunkCapacity).HasColumnName("trunkcapacity").HasColumnType("int(11)");
                entity.Property(e => e.maxFuel).HasColumnName("maxfuel").HasColumnType("int(11)");
                entity.Property(e => e.fuelType).HasColumnName("fueltype").HasMaxLength(64);
                entity.Property(e => e.seats).HasColumnName("seats");
                entity.Property(e => e.tax).HasColumnName("tax").HasColumnType("int(11)");
                entity.Property(e => e.vehClass).HasColumnName("vehClass").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Server_Animations>(entity =>
            {
                entity.HasKey(e => e.animId);
                entity.ToTable("server_animations", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.animId).HasName("animId");
                entity.Property(e => e.animId).HasColumnName("animId");
                entity.Property(e => e.displayName).HasColumnName("displayName").HasMaxLength(64);
                entity.Property(e => e.animDict).HasColumnName("animDict").HasMaxLength(64);
                entity.Property(e => e.animName).HasColumnName("animName").HasMaxLength(64);
                entity.Property(e => e.animFlag).HasColumnName("animFlag").HasColumnType("int(11)");
                entity.Property(e => e.duration).HasColumnName("duration").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Server_ATM>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_atm", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
                entity.Property(e => e.zoneName).HasColumnName("zoneName").HasMaxLength(64);
            });

            modelBuilder.Entity<Server_Banks>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_banks", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
                entity.Property(e => e.zoneName).HasColumnName("zoneName").HasMaxLength(64);
            });

            modelBuilder.Entity<Server_Bank_Paper>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_bank_paper", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.accountNumber).HasColumnName("accountNumber");
                entity.Property(e => e.Date).HasColumnName("Date");
                entity.Property(e => e.Time).HasColumnName("Time");
                entity.Property(e => e.Type).HasColumnName("Type");
                entity.Property(e => e.ToOrFrom).HasColumnName("ToOrFrom");
                entity.Property(e => e.TransactionMessage).HasColumnName("TransactionMessage");
                entity.Property(e => e.moneyAmount).HasColumnName("moneyAmount");
                entity.Property(e => e.zoneName).HasColumnName("zoneName");
            });

            modelBuilder.Entity<Server_Blips>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_blips", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.name).HasColumnName("name").HasMaxLength(64);
                entity.Property(e => e.color).HasColumnName("color");
                entity.Property(e => e.scale).HasColumnName("scale");
                entity.Property(e => e.shortRange).HasColumnName("shortRange");
                entity.Property(e => e.sprite).HasColumnName("sprite");
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
            });

            modelBuilder.Entity<Server_Barbers>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_barbers", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
                entity.Property(e => e.pedModel).HasColumnName("pedModel").HasMaxLength(64);
                entity.Property(e => e.pedX).HasColumnName("pedX");
                entity.Property(e => e.pedY).HasColumnName("pedY");
                entity.Property(e => e.pedZ).HasColumnName("pedZ");
                entity.Property(e => e.pedRot).HasColumnName("pedRot");
            });

            modelBuilder.Entity<Server_Companys>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_companys", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.companyName).HasColumnName("companyName").HasMaxLength(64);
                entity.Property(e => e.companyOwnerId).HasColumnName("companyOwnerId").HasColumnType("int(11)");
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
                entity.Property(e => e.givenLicense).HasColumnName("givenLicense").HasMaxLength(64);
                entity.Property(e => e.createdTimestamp).HasColumnName("createdTimestamp");
                entity.Property(e => e.companyMoney).HasColumnName("companyMoney").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Server_Company_Members>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_company_members", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.companyId).HasColumnName("companyId").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charId").HasColumnType("int(11)");
                entity.Property(e => e.rank).HasColumnName("rank").HasColumnType("int(11)");
                entity.Property(e => e.invitedTimestamp).HasColumnName("invitedTimestamp");
            });

            modelBuilder.Entity<Server_Clothes_Shops>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_clothesshops", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.name).HasColumnName("name");
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
                entity.Property(e => e.pedX).HasColumnName("pedX");
                entity.Property(e => e.pedY).HasColumnName("pedY");
                entity.Property(e => e.pedZ).HasColumnName("pedZ");
                entity.Property(e => e.pedRot).HasColumnName("pedRot");
                entity.Property(e => e.pedModel).HasColumnName("pedModel");
            });

            modelBuilder.Entity<Server_Clothes_Shops_Items>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_clothesshops_items", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.componentId).HasColumnName("componentId").HasColumnType("int(11)");
                entity.Property(e => e.drawableId).HasColumnName("drawableId").HasColumnType("int(11)");
                entity.Property(e => e.textureId).HasColumnName("textureId").HasColumnType("int(11)");
                entity.Property(e => e.gender).HasColumnName("gender").HasColumnType("int(11)");
                entity.Property(e => e.isProp).HasColumnName("isProp").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Server_Doors>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_doors", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.name).HasColumnName("name").HasMaxLength(64);
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
                entity.Property(e => e.hash).HasColumnName("hash").HasMaxLength(64);
                entity.Property(e => e.state).HasColumnName("state");
                entity.Property(e => e.doorKey).HasColumnName("doorKey").HasMaxLength(64);
                entity.Property(e => e.doorKey2).HasColumnName("doorKey2").HasMaxLength(64);
                entity.Property(e => e.type).HasColumnName("type").HasMaxLength(64);
                entity.Property(e => e.lockPosX).HasColumnName("lockPosX");
                entity.Property(e => e.lockPosY).HasColumnName("lockPosY");
                entity.Property(e => e.lockPosZ).HasColumnName("lockPosZ");
            });

            modelBuilder.Entity<Server_Factions>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_factions", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.factionName).HasColumnName("factionName").HasMaxLength(128);
                entity.Property(e => e.factionShort).HasColumnName("factionShort").HasMaxLength(64);
                entity.Property(e => e.factionMoney).HasColumnName("factionMoney"); 
                entity.Property(e => e.laborPos).HasColumnName("laborPos").HasConversion(
                     v => JsonConvert.SerializeObject(v),
                     v => JsonConvert.DeserializeObject<Position>(v));
            });

            modelBuilder.Entity<Server_Faction_Labor_Items>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_faction_labor_items", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.factionId).HasColumnName("factionId").HasColumnType("int(11)");
                entity.Property(e => e.accountId).HasColumnName("accountId").HasColumnType("int(11)");
                entity.Property(e => e.itemName).HasColumnName("itemName").HasMaxLength(64);
                entity.Property(e => e.itemAmount).HasColumnName("itemAmount").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Server_Faction_Clothes>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_faction_clothes", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.faction).HasColumnName("faction").HasColumnType("int(11)");
                entity.Property(e => e.clothesName).HasColumnName("clothesName").HasMaxLength(128);
            });

            modelBuilder.Entity<Server_Faction_Members>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_faction_members", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charId").HasColumnType("int(11)");
                entity.Property(e => e.factionId).HasColumnName("factionId").HasColumnType("int(11)");
                entity.Property(e => e.rank).HasColumnName("rank").HasColumnType("int(11)");
                entity.Property(e => e.serviceNumber).HasColumnName("servicenumber").HasColumnType("int(11)");
                entity.Property(e => e.isDuty).HasColumnName("isDuty");
                entity.Property(e => e.lastChange).HasColumnName("lastChange");
            });

            modelBuilder.Entity<Server_Faction_Ranks>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_faction_ranks", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.factionId).HasColumnName("factionId").HasColumnType("int(11)");
                entity.Property(e => e.rankId).HasColumnName("rankId").HasColumnType("int(11)");
                entity.Property(e => e.name).HasColumnName("name").HasMaxLength(64);
                entity.Property(e => e.paycheck).HasColumnName("paycheck").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Server_Faction_Storage_Items>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_faction_storage_items", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charId").HasColumnType("int(11)");
                entity.Property(e => e.factionId).HasColumnName("factionId").HasColumnType("int(11)");
                entity.Property(e => e.itemName).HasColumnName("itemName").HasMaxLength(128);
                entity.Property(e => e.amount).HasColumnName("amount").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Server_Faction_Positions>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_faction_positions", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.factionId).HasColumnName("factionId").HasColumnType("int(11)");
                entity.Property(e => e.posType).HasColumnName("posType").HasMaxLength(64);
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
                entity.Property(e => e.rotation).HasColumnName("rotation");
            });

            modelBuilder.Entity<Server_Farming_Producer>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_farming_producer", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
                entity.Property(e => e.pedRotation).HasColumnName("pedRotation");
                entity.Property(e => e.pedModel).HasColumnName("pedModel").HasMaxLength(64);
                entity.Property(e => e.neededItem).HasColumnName("neededItem").HasMaxLength(64);
                entity.Property(e => e.neededItemTWO).HasColumnName("neededItemTWO").HasMaxLength(64);
                entity.Property(e => e.neededItemTHREE).HasColumnName("neededItemTHREE").HasMaxLength(64);
                entity.Property(e => e.producedItem).HasColumnName("producedItem").HasMaxLength(64);
                entity.Property(e => e.range).HasColumnName("range");
                entity.Property(e => e.duration).HasColumnName("duration");
                entity.Property(e => e.neededItemAmount).HasColumnName("neededItemAmount").HasColumnType("int(11)");
                entity.Property(e => e.producedItemAmount).HasColumnName("producedItemAmount").HasColumnType("int(11)");
                entity.Property(e => e.neededItemTWOAmount).HasColumnName("neededItemTWOAmount").HasColumnType("int(11)");
                entity.Property(e => e.neededItemTHREEAmount).HasColumnName("neededItemTHREEAmount").HasColumnType("int(11)");
                entity.Property(e => e.blipName).HasColumnName("blipName").HasMaxLength(64);
                entity.Property(e => e.isBlipVisible).HasColumnName("isBlipVisible");
            });


            modelBuilder.Entity<Server_Storages>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_storages", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.owner).HasColumnName("owner").HasColumnType("int(11)");
                entity.Property(e => e.secondOwner).HasColumnName("secondOwner").HasColumnType("int(11)");
                entity.Property(e => e.entryPos).HasColumnName("entryPos").HasConversion(
                     v => JsonConvert.SerializeObject(v),
                     v => JsonConvert.DeserializeObject<Position>(v));
                entity.Property(e => e.items).HasColumnName("items").HasConversion(
                     v => JsonConvert.SerializeObject(v),
                     v => JsonConvert.DeserializeObject<List<Server_Storage_Item>>(v));
                entity.Property(e => e.maxSize).HasColumnName("maxSize");
                entity.Property(e => e.price).HasColumnName("price").HasColumnType("int(11)");
                entity.Property(e => e.isfaction).HasColumnName("isfaction").HasColumnType("int(11)");
                entity.Property(e => e.factionid).HasColumnName("factionid").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Server_Farming_Spots>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_farming_spots", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
                entity.Property(e => e.itemName).HasColumnName("itemName").HasMaxLength(64);
                entity.Property(e => e.animation).HasColumnName("animation").HasMaxLength(64);
                entity.Property(e => e.neededItemToFarm).HasColumnName("neededItemToFarm").HasMaxLength(64);
                entity.Property(e => e.itemMinAmount).HasColumnName("itemMinAmount");
                entity.Property(e => e.itemMaxAmount).HasColumnName("itemMaxAmount");
                entity.Property(e => e.markerColorR).HasColumnName("markerColorR");
                entity.Property(e => e.markerColorG).HasColumnName("markerColorG");
                entity.Property(e => e.markerColorB).HasColumnName("markerColorB");
                entity.Property(e => e.blipColor).HasColumnName("blipColor").HasColumnType("int(11)");
                entity.Property(e => e.range).HasColumnName("range");
                entity.Property(e => e.duration).HasColumnName("duration");
                entity.Property(e => e.isBlipVisible).HasColumnName("isBlipVisible");
            });

            modelBuilder.Entity<Server_Fuel_Stations>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_fuel_stations", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.name).HasColumnName("name").HasMaxLength(64);
                entity.Property(e => e.owner).HasColumnName("owner").HasColumnType("int(11)");
                entity.Property(e => e.availableFuel).HasColumnName("availableFuel").HasMaxLength(128);
                entity.Property(e => e.availableLiters).HasColumnName("availableLiters").HasColumnType("int(11)");
                entity.Property(e => e.bank).HasColumnName("bank").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Server_Fuel_Spots>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_fuel_spots", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.fuelStationId).HasColumnName("fuelStationId").HasColumnType("int(11)");
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
            });

            modelBuilder.Entity<Server_Garages>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_garages", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.name).HasColumnName("name").HasMaxLength(64);
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
                entity.Property(e => e.rotation).HasColumnName("rotation");
                entity.Property(e => e.type).HasColumnName("type").HasColumnType("int(11)");
                entity.Property(e => e.isBlipVisible).HasColumnName("isBlipVisible");
            });

            modelBuilder.Entity<Server_Garage_Slots>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_garage_slots", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.garageId).HasColumnName("garageId").HasColumnType("int(11)");
                entity.Property(e => e.parkId).HasColumnName("parkid").HasColumnType("int(11)");
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
                entity.Property(e => e.rotX).HasColumnName("rotX");
                entity.Property(e => e.rotY).HasColumnName("rotY");
                entity.Property(e => e.rotZ).HasColumnName("rotZ");
            });

            modelBuilder.Entity<Server_Hotels>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_hotels", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.name).HasColumnName("name").HasMaxLength(64);
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
            });

            modelBuilder.Entity<Server_Hotels_Apartments>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_hotels_apartments", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.hotelId).HasColumnName("hotelId").HasColumnType("int(11)");
                entity.Property(e => e.interiorId).HasColumnName("interiorId").HasColumnType("int(11)");
                entity.Property(e => e.ownerId).HasColumnName("ownerId").HasColumnType("int(11)");
                entity.Property(e => e.rentPrice).HasColumnName("rentPrice").HasColumnType("int(11)");
                entity.Property(e => e.maxRentHours).HasColumnName("maxRentHours").HasColumnType("int(11)");
                entity.Property(e => e.lastRent).HasColumnName("lastRent");
            });

            modelBuilder.Entity<Server_Hotels_Storage>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_hotels_storage", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.apartmentId).HasColumnName("apartmentId").HasColumnType("int(11)");
                entity.Property(e => e.itemName).HasColumnName("itemName").HasMaxLength(64);
                entity.Property(e => e.amount).HasColumnName("amount").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Server_Houses_Interiors>(entity =>
            {
                entity.HasKey(e => e.interiorId);
                entity.ToTable("server_houses_interior", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.interiorId).HasName("interiorId");
                entity.Property(e => e.interiorId).HasColumnName("interiorId");
                entity.Property(e => e.exitX).HasColumnName("exitX");
                entity.Property(e => e.exitY).HasColumnName("exitY");
                entity.Property(e => e.exitZ).HasColumnName("exitZ");
                entity.Property(e => e.storageX).HasColumnName("storageX");
                entity.Property(e => e.storageY).HasColumnName("storageY");
                entity.Property(e => e.storageZ).HasColumnName("storageZ");
                entity.Property(e => e.storageLimit).HasColumnName("storageLimit");
                entity.Property(e => e.manageX).HasColumnName("manageX");
                entity.Property(e => e.manageY).HasColumnName("manageY");
                entity.Property(e => e.manageZ).HasColumnName("manageZ");
            });

            modelBuilder.Entity<Server_Houses_Storage>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_houses_storage", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.houseId).HasColumnName("houseId").HasColumnType("int(11)");
                entity.Property(e => e.itemName).HasColumnName("itemName").HasMaxLength(128);
                entity.Property(e => e.itemAmount).HasColumnName("amount").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Server_Houses_Renter>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_houses_renter", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charId").HasColumnType("int(11)");
                entity.Property(e => e.houseId).HasColumnName("houseId").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Server_Houses>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_houses", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.interiorId).HasColumnName("interiorId").HasColumnType("int(11)");
                entity.Property(e => e.ownerId).HasColumnName("ownerId").HasColumnType("int(11)");
                entity.Property(e => e.street).HasColumnName("street").HasMaxLength(64);
                entity.Property(e => e.maxRenters).HasColumnName("maxRenters").HasColumnType("int(11)");
                entity.Property(e => e.rentPrice).HasColumnName("rentPrice").HasColumnType("int(11)");
                entity.Property(e => e.isRentable).HasColumnName("isRentable");
                entity.Property(e => e.hasStorage).HasColumnName("hasStorage");
                entity.Property(e => e.hasAlarm).HasColumnName("hasAlarm");
                entity.Property(e => e.hasBank).HasColumnName("hasBank");
                entity.Property(e => e.entranceX).HasColumnName("entranceX");
                entity.Property(e => e.entranceY).HasColumnName("entranceY");
                entity.Property(e => e.entranceZ).HasColumnName("entranceZ");
                entity.Property(e => e.money).HasColumnName("money");
            });

            modelBuilder.Entity<Server_Items>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_items", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.itemName).HasColumnName("itemName").HasMaxLength(64);
                entity.Property(e => e.itemType).HasColumnName("itemType").HasMaxLength(64);
                entity.Property(e => e.itemDescription).HasColumnName("itemDescription").HasMaxLength(256);
                entity.Property(e => e.itemWeight).HasColumnName("itemWeight");
                entity.Property(e => e.isItemDesire).HasColumnName("isItemDesire");
                entity.Property(e => e.itemDesireFood).HasColumnName("itemDesireFood").HasColumnType("int(11)");
                entity.Property(e => e.itemDesireDrink).HasColumnName("itemDesireDrink").HasColumnType("int(11)");
                entity.Property(e => e.hasItemAnimation).HasColumnName("hasItemAnimation");
                entity.Property(e => e.itemAnimationName).HasColumnName("itemAnimationName").HasMaxLength(64);
                entity.Property(e => e.isItemDroppable).HasColumnName("isItemDroppable");
                entity.Property(e => e.isItemUseable).HasColumnName("isItemUseable");
                entity.Property(e => e.isItemGiveable).HasColumnName("isItemGiveable");
                entity.Property(e => e.isItemClothes).HasColumnName("isItemClothes");
                entity.Property(e => e.ClothesType).HasColumnName("ClothesType");
                entity.Property(e => e.ClothesDraw).HasColumnName("ClothesDraw");
                entity.Property(e => e.ClothesTexture).HasColumnName("ClothesTexture");
                entity.Property(e => e.ClothesUndershirt).HasColumnName("ClothesUndershirt");
                entity.Property(e => e.ClothesUndershirtTexture).HasColumnName("ClothesUndershirtTexture");
                entity.Property(e => e.ClothesDecals).HasColumnName("ClothesDecals");
                entity.Property(e => e.ClothesDecalsTexture).HasColumnName("ClothesDecalsTexture");
                entity.Property(e => e.itemPicSRC).HasColumnName("itemPicSRC");
            });

            modelBuilder.Entity<Server_Jobs>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_jobs", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.jobName).HasColumnName("jobName").HasMaxLength(64);
                entity.Property(e => e.jobPaycheck).HasColumnName("jobPaycheck").HasColumnType("int(11)");
                entity.Property(e => e.jobNeededHours).HasColumnName("jobNeededHours").HasColumnType("int(11)");
                entity.Property(e => e.jobPic).HasColumnName("jobPic").HasMaxLength(64);
            });

            modelBuilder.Entity<Server_Licenses>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_licenses", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.licCut).HasColumnName("licCut").HasMaxLength(64);
                entity.Property(e => e.licName).HasColumnName("licName").HasMaxLength(64);
                entity.Property(e => e.licPrice).HasColumnName("licPrice").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Server_Markers>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_markers", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.type).HasColumnName("type").HasColumnType("int(11)");
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
                entity.Property(e => e.scaleX).HasColumnName("scaleX");
                entity.Property(e => e.scaleY).HasColumnName("scaleY");
                entity.Property(e => e.scaleZ).HasColumnName("scaleZ");
                entity.Property(e => e.red).HasColumnName("red").HasColumnType("int(11)");
                entity.Property(e => e.green).HasColumnName("green").HasColumnType("int(11)");
                entity.Property(e => e.blue).HasColumnName("blue").HasColumnType("int(11)");
                entity.Property(e => e.alpha).HasColumnName("alpha").HasColumnType("int(11)");
                entity.Property(e => e.bobUpAndDown).HasColumnName("bobUpAndDown");
            });

            modelBuilder.Entity<Server_Minijob_Busdriver_Routes>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_minijob_busdriver_routes", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.routeId).HasColumnName("routeId").HasColumnType("int(11)");
                entity.Property(e => e.routeName).HasColumnName("routeName").HasMaxLength(64);
                entity.Property(e => e.hash).HasColumnName("hash");
                entity.Property(e => e.neededExp).HasColumnName("neededExp");
                entity.Property(e => e.givenExp).HasColumnName("givenExp");
                entity.Property(e => e.paycheck).HasColumnName("paycheck");
                entity.Property(e => e.neededTime).HasColumnName("neededTime").HasMaxLength(64);
            });

            modelBuilder.Entity<Server_Minijob_Busdriver_Spots>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_minijob_busdriver_spots", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.routeId).HasColumnName("routeId").HasColumnType("int(11)");
                entity.Property(e => e.spotId).HasColumnName("spotId").HasColumnType("int(11)");
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
            });

            modelBuilder.Entity<Server_Minijob_Garbage_Spots>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_minijob_garbage_spots", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.routeId).HasColumnName("routeId").HasColumnType("int(11)");
                entity.Property(e => e.spotId).HasColumnName("spotId").HasColumnType("int(11)");
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
            });

            modelBuilder.Entity<Server_Peds>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_peds", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.model).HasColumnName("model").HasMaxLength(64);
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
                entity.Property(e => e.rotation).HasColumnName("rotation");
            });

            modelBuilder.Entity<Server_Shops>(entity =>
            {
                entity.HasKey(e => e.shopId);
                entity.ToTable("server_shops", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.shopId).HasName("shopid");
                entity.Property(e => e.shopId).HasColumnName("shopid").HasColumnType("int(11)");
                entity.Property(e => e.name).HasColumnName("name").HasMaxLength(64);
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
                entity.Property(e => e.pedX).HasColumnName("pedX");
                entity.Property(e => e.pedY).HasColumnName("pedY");
                entity.Property(e => e.pedZ).HasColumnName("pedZ");
                entity.Property(e => e.pedRot).HasColumnName("pedRot");
                entity.Property(e => e.pedModel).HasColumnName("pedModel").HasMaxLength(64);
                entity.Property(e => e.neededLicense).HasColumnName("neededLicense").HasMaxLength(64);
                entity.Property(e => e.isOnlySelling).HasColumnName("isOnlySelling");
                entity.Property(e => e.isBlipVisible).HasColumnName("isBlipVisible");
                entity.Property(e => e.faction).HasColumnName("faction");
            });

            modelBuilder.Entity<Server_Shops_Items>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_shop_items", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.shopId).HasColumnName("shopid").HasColumnType("int(11)");
                entity.Property(e => e.itemName).HasColumnName("itemName").HasMaxLength(128);
                entity.Property(e => e.itemAmount).HasColumnName("itemAmount").HasColumnType("int(11)");
                entity.Property(e => e.itemPrice).HasColumnName("itemPrice").HasColumnType("int(11)");
                entity.Property(e => e.itemGender).HasColumnName("itemGender").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Server_Tablet_Apps>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_tablet_apps", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.appName).HasColumnName("appName").HasMaxLength(64);
                entity.Property(e => e.appPrice).HasColumnName("appPrice").HasColumnType("int(11)");
            });

            modelBuilder.Entity<Server_Tablet_Events>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_tablet_events", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charId").HasColumnType("int(11)");
                entity.Property(e => e.title).HasColumnName("title").HasMaxLength(64);
                entity.Property(e => e.ownerName).HasColumnName("owner").HasMaxLength(64);
                entity.Property(e => e.callnumber).HasColumnName("callnumber").HasMaxLength(64);
                entity.Property(e => e.location).HasColumnName("location").HasMaxLength(64);
                entity.Property(e => e.eventtype).HasColumnName("eventtype").HasMaxLength(64);
                entity.Property(e => e.date).HasColumnName("date").HasMaxLength(64);
                entity.Property(e => e.time).HasColumnName("time").HasMaxLength(64);
                entity.Property(e => e.info).HasColumnName("info").HasMaxLength(128);
                entity.Property(e => e.created).HasColumnName("created");
            });

            modelBuilder.Entity<Server_Tablet_Notes>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_tablet_notes", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charId").HasColumnType("int(11)");
                entity.Property(e => e.color).HasColumnName("color").HasMaxLength(64);
                entity.Property(e => e.title).HasColumnName("title").HasMaxLength(64);
                entity.Property(e => e.text).HasColumnName("text").HasMaxLength(128);
                entity.Property(e => e.created).HasColumnName("created");
            });

            modelBuilder.Entity<Server_Teleports>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_teleports", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.name).HasColumnName("name").HasMaxLength(64);
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
                entity.Property(e => e.targetX).HasColumnName("targetX");
                entity.Property(e => e.targetY).HasColumnName("targetY");
                entity.Property(e => e.targetZ).HasColumnName("targetZ");
                entity.Property(e => e.dimension).HasColumnName("dimension");
            });

            modelBuilder.Entity<LogsLogin>(entity =>
            {
                entity.ToTable("logs_login", Constants.DatabaseConfig.Database);
                entity.HasKey(e => e.id);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.username).IsRequired().HasColumnName("username").HasMaxLength(64);
                entity.Property(e => e.socialclub).IsRequired().HasColumnName("socialclub").HasMaxLength(64);
                entity.Property(e => e.text).IsRequired().HasColumnName("text").HasMaxLength(256);
                entity.Property(e => e.address).IsRequired().HasColumnName("ipadress").HasMaxLength(64);
                entity.Property(e => e.hwid).IsRequired().HasColumnName("hardwareid").HasMaxLength(64);
                entity.Property(e => e.success).IsRequired().HasColumnName("success");
                entity.Property(e => e.timestamp).IsRequired().HasColumnName("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Logs_Company>(entity =>
            {
                entity.ToTable("logs_company", Constants.DatabaseConfig.Database);
                entity.HasKey(e => e.id);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.companyId).HasColumnName("companyId").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charId").HasColumnType("int(11)");
                entity.Property(e => e.targetCharId).HasColumnName("targetCharId").HasColumnType("int(11)");
                entity.Property(e => e.type).HasColumnName("type").HasMaxLength(64);
                entity.Property(e => e.text).IsRequired().HasColumnName("text").HasMaxLength(256);
                entity.Property(e => e.timestamp).IsRequired().HasColumnName("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Logs_Faction>(entity =>
            {
                entity.ToTable("logs_faction", Constants.DatabaseConfig.Database);
                entity.HasKey(e => e.id);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.factionId).HasColumnName("factionId").HasColumnType("int(11)");
                entity.Property(e => e.charId).HasColumnName("charId").HasColumnType("int(11)");
                entity.Property(e => e.targetCharId).HasColumnName("targetCharId").HasColumnType("int(11)");
                entity.Property(e => e.type).HasColumnName("type").HasMaxLength(64);
                entity.Property(e => e.text).IsRequired().HasColumnName("text").HasMaxLength(256);
                entity.Property(e => e.timestamp).IsRequired().HasColumnName("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Server_Vehicles>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_vehicles", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.charid).HasColumnName("charid").HasColumnType("int(11)");
                entity.Property(e => e.hash).HasColumnName("hash");
                entity.Property(e => e.vehType).HasColumnName("vehType").HasColumnType("int(11)");
                entity.Property(e => e.faction).HasColumnName("faction");
                entity.Property(e => e.fuel).HasColumnName("fuel");
                entity.Property(e => e.KM).HasColumnName("km");
                entity.Property(e => e.engineState).HasColumnName("enginestate");
                entity.Property(e => e.isEngineHealthy).HasColumnName("isEngineHealthy");
                entity.Property(e => e.lockState).HasColumnName("lockstate");
                entity.Property(e => e.isInGarage).HasColumnName("isingarage");
                entity.Property(e => e.garageId).HasColumnName("garageid");
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
                entity.Property(e => e.rotX).HasColumnName("rotX");
                entity.Property(e => e.rotY).HasColumnName("rotY");
                entity.Property(e => e.rotZ).HasColumnName("rotZ");
                entity.Property(e => e.plate).HasColumnName("plate").HasMaxLength(8);
                entity.Property(e => e.lastUsage).HasColumnName("lastUsage");
                entity.Property(e => e.buyDate).HasColumnName("buyDate");
            });

            modelBuilder.Entity<Server_Vehicles_Mod>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_vehicles_mod", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.vehId).HasColumnName("vehId").HasColumnType("int(11)");
                entity.Property(e => e.colorPrimaryType).HasColumnName("colorPrimaryType");
                entity.Property(e => e.colorPrimary_r).HasColumnName("colorPrimary_r");
                entity.Property(e => e.colorPrimary_g).HasColumnName("colorPrimary_g");
                entity.Property(e => e.colorPrimary_b).HasColumnName("colorPrimary_b");
                entity.Property(e => e.colorSecondaryType).HasColumnName("colorSecondaryType");
                entity.Property(e => e.colorSecondary_r).HasColumnName("colorSecondary_r");
                entity.Property(e => e.colorSecondary_g).HasColumnName("colorSecondary_g");
                entity.Property(e => e.colorSecondary_b).HasColumnName("colorSecondary_b");
                entity.Property(e => e.colorPearl).HasColumnName("colorPearl").HasColumnType("int(11)");
                entity.Property(e => e.headlightColor).HasColumnName("headlightColor").HasColumnType("int(11)");
                entity.Property(e => e.spoiler).HasColumnName("spoiler").HasColumnType("int(11)");
                entity.Property(e => e.front_bumper).HasColumnName("front_bumper").HasColumnType("int(11)");
                entity.Property(e => e.rear_bumper).HasColumnName("rear_bumper").HasColumnType("int(11)");
                entity.Property(e => e.side_skirt).HasColumnName("side_skirt").HasColumnType("int(11)");
                entity.Property(e => e.exhaust).HasColumnName("exhaust").HasColumnType("int(11)");
                entity.Property(e => e.frame).HasColumnName("frame").HasColumnType("int(11)");
                entity.Property(e => e.grille).HasColumnName("grille").HasColumnType("int(11)");
                entity.Property(e => e.hood).HasColumnName("hood").HasColumnType("int(11)");
                entity.Property(e => e.fender).HasColumnName("fender").HasColumnType("int(11)");
                entity.Property(e => e.right_fender).HasColumnName("right_fender").HasColumnType("int(11)");
                entity.Property(e => e.roof).HasColumnName("roof").HasColumnType("int(11)");
                entity.Property(e => e.engine).HasColumnName("engine").HasColumnType("int(11)");
                entity.Property(e => e.brakes).HasColumnName("brakes").HasColumnType("int(11)");
                entity.Property(e => e.transmission).HasColumnName("transmission").HasColumnType("int(11)");
                entity.Property(e => e.horns).HasColumnName("horns").HasColumnType("int(11)");
                entity.Property(e => e.suspension).HasColumnName("suspension").HasColumnType("int(11)");
                entity.Property(e => e.armor).HasColumnName("armor").HasColumnType("int(11)");
                entity.Property(e => e.turbo).HasColumnName("turbo").HasColumnType("int(11)");
                entity.Property(e => e.xenon).HasColumnName("xenon").HasColumnType("int(11)");
                entity.Property(e => e.wheel_type).HasColumnName("wheel_type").HasColumnType("int(11)");
                entity.Property(e => e.wheels).HasColumnName("wheels").HasColumnType("int(11)");
                entity.Property(e => e.back_wheels).HasColumnName("back_wheels").HasColumnType("int(11)");
                entity.Property(e => e.wheelcolor).HasColumnName("wheelcolor").HasColumnType("int(11)");
                entity.Property(e => e.plate_holder).HasColumnName("plate_holder").HasColumnType("int(11)");
                entity.Property(e => e.plate_vanity).HasColumnName("plate_vanity").HasColumnType("int(11)");
                entity.Property(e => e.trim_design).HasColumnName("trim_design").HasColumnType("int(11)");
                entity.Property(e => e.ornaments).HasColumnName("ornaments").HasColumnType("int(11)");
                entity.Property(e => e.dial_design).HasColumnName("dial_design").HasColumnType("int(11)");
                entity.Property(e => e.door_interior).HasColumnName("door_interior").HasColumnType("int(11)");
                entity.Property(e => e.seats).HasColumnName("seats").HasColumnType("int(11)");
                entity.Property(e => e.steering_wheel).HasColumnName("steering_wheel").HasColumnType("int(11)");
                entity.Property(e => e.shift_lever).HasColumnName("shift_lever").HasColumnType("int(11)");
                entity.Property(e => e.plaques).HasColumnName("plaques").HasColumnType("int(11)");
                entity.Property(e => e.hydraulics).HasColumnName("hydraulics").HasColumnType("int(11)");
                entity.Property(e => e.rear_shelf).HasColumnName("rear_shelf").HasColumnType("int(11)");
                entity.Property(e => e.engine_block).HasColumnName("engine_block").HasColumnType("int(11)");
                entity.Property(e => e.trunk).HasColumnName("trunk").HasColumnType("int(11)");
                entity.Property(e => e.airfilter).HasColumnName("airfilter").HasColumnType("int(11)");
                entity.Property(e => e.strut_bar).HasColumnName("strut_bar").HasColumnType("int(11)");
                entity.Property(e => e.arch_cover).HasColumnName("arch_cover").HasColumnType("int(11)");
                entity.Property(e => e.antenna).HasColumnName("antenna").HasColumnType("int(11)");
                entity.Property(e => e.exterior_parts).HasColumnName("exterior_parts").HasColumnType("int(11)");
                entity.Property(e => e.tank).HasColumnName("tank").HasColumnType("int(11)");
                entity.Property(e => e.door).HasColumnName("door").HasColumnType("int(11)");
                entity.Property(e => e.window_tint).HasColumnName("window_tint").HasColumnType("int(11)");
                entity.Property(e => e.rear_hydraulics).HasColumnName("rear_hydraulics").HasColumnType("int(11)");
                entity.Property(e => e.livery).HasColumnName("livery").HasColumnType("int(11)");
                entity.Property(e => e.plate).HasColumnName("plate").HasColumnType("int(11)");
                entity.Property(e => e.plate_color).HasColumnName("plate_color").HasColumnType("int(11)");
                entity.Property(e => e.interior_color).HasColumnName("interior_color");
                entity.Property(e => e.neon).HasColumnName("neon");
                entity.Property(e => e.neon_r).HasColumnName("neon_r");
                entity.Property(e => e.neon_g).HasColumnName("neon_g");
                entity.Property(e => e.neon_b).HasColumnName("neon_b");
                entity.Property(e => e.smoke_r).HasColumnName("smoke_r");
                entity.Property(e => e.smoke_g).HasColumnName("smoke_g");
                entity.Property(e => e.smoke_b).HasColumnName("smoke_b");
                entity.Property(e => e.smoke).HasColumnName("smoke");
            });


            modelBuilder.Entity<Server_Vehicle_Items>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_vehicle_items", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.vehId).HasColumnName("vehId").HasColumnType("int(11)");
                entity.Property(e => e.itemName).HasColumnName("itemName").HasMaxLength(128);
                entity.Property(e => e.itemAmount).HasColumnName("itemAmount").HasColumnType("int(11)");
                entity.Property(e => e.isInGlovebox).HasColumnName("isInGlovebox");
            });

            modelBuilder.Entity<Server_Vehicle_Shops>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_vehicle_shops", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.name).HasColumnName("name").HasMaxLength(64);
                entity.Property(e => e.pedX).HasColumnName("pedX");
                entity.Property(e => e.pedY).HasColumnName("pedY");
                entity.Property(e => e.pedZ).HasColumnName("pedZ");
                entity.Property(e => e.pedRot).HasColumnName("pedRot");
                entity.Property(e => e.parkOutX).HasColumnName("parkOutX");
                entity.Property(e => e.parkOutY).HasColumnName("parkOutY");
                entity.Property(e => e.parkOutZ).HasColumnName("parkOutZ");
                entity.Property(e => e.parkOutRotX).HasColumnName("parkOutRotX");
                entity.Property(e => e.parkOutRotY).HasColumnName("parkOutRotY");
                entity.Property(e => e.parkOutRotZ).HasColumnName("parkOutRotZ");
                entity.Property(e => e.neededLicense).HasColumnName("neededLicense").HasMaxLength(64);
            });

            modelBuilder.Entity<Server_Vehicle_Shops_Items>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("server_vehicle_shops_items", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.id).HasName("id");
                entity.Property(e => e.id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.shopId).HasColumnName("shopid").HasColumnType("int(11)");
                entity.Property(e => e.hash).HasColumnName("hash");
                entity.Property(e => e.price).HasColumnName("price").HasColumnType("int(11)");
                entity.Property(e => e.posX).HasColumnName("posX");
                entity.Property(e => e.posY).HasColumnName("posY");
                entity.Property(e => e.posZ).HasColumnName("posZ");
                entity.Property(e => e.rotX).HasColumnName("rotX");
                entity.Property(e => e.rotY).HasColumnName("rotY");
                entity.Property(e => e.rotZ).HasColumnName("rotZ");
                entity.Property(e => e.isOnlyOnlineAvailable).HasColumnName("isOnlyOnlineAvailable");
            });

            modelBuilder.Entity<Server_Wanteds>(entity =>
            {
                entity.HasKey(e => e.wantedId);
                entity.ToTable("server_wanteds", Constants.DatabaseConfig.Database);
                entity.HasIndex(e => e.wantedId).HasName("wantedId");
                entity.Property(e => e.wantedId).HasColumnName("wantedId").HasColumnType("int(11)");
                entity.Property(e => e.category).HasColumnName("category").HasColumnType("int(11)");
                entity.Property(e => e.wantedName).HasColumnName("wantedName").HasMaxLength(128);
                entity.Property(e => e.paragraph).HasColumnName("paragraph").HasColumnType("int(11)");
                entity.Property(e => e.jailtime).HasColumnName("jailtime").HasColumnType("int(11)");
                entity.Property(e => e.ticketfine).HasColumnName("ticketfine").HasColumnType("int(11)");
            });
        }
    }
}
