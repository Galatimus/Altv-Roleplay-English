using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Data;
using Altv_Roleplay.Model;
using Altv_Roleplay.Handler;
using System;
using System.Timers;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Net;
using System.IO;
using Altv_Roleplay.Utils;
using System.Linq;
using System.Numerics;
using AltV.Net.Async;
using System.Threading.Tasks;
using Altv_Roleplay.Factories;

namespace Altv_Roleplay
{
    public class Main : AsyncResource
    {
        public override IEntityFactory<IPlayer> GetPlayerFactory()
        {
            return new AccountsFactory();
        }

        public override IBaseObjectFactory<IColShape> GetColShapeFactory()
        {
            return new ColshapeFactory();
        }

        public override IEntityFactory<IVehicle> GetVehicleFactory()
        {
            return new VehicleFactory();
        }

        public override void OnStart()
        {
            AltV.Net.EntitySync.AltEntitySync.Init(7, (threadId) => 200, (threadId) => false,
            (threadCount, repository) => new AltV.Net.EntitySync.ServerEvent.ServerEventNetworkLayer(threadCount, repository),
            (entity, threadCount) => entity.Type,
            (entityId, entityType, threadCount) => entityType,
            (threadId) =>
            {
                return threadId switch
                {
                                // Marker
                                0 => new AltV.Net.EntitySync.SpatialPartitions.LimitedGrid3(50_000, 50_000, 75, 10_000, 10_000, 64),
                                // Text
                                1 => new AltV.Net.EntitySync.SpatialPartitions.LimitedGrid3(50_000, 50_000, 75, 10_000, 10_000, 32),
                                // Props
                                2 => new AltV.Net.EntitySync.SpatialPartitions.LimitedGrid3(50_000, 50_000, 100, 10_000, 10_000, 1500),
                                // Help Text
                                3 => new AltV.Net.EntitySync.SpatialPartitions.LimitedGrid3(50_000, 50_000, 100, 10_000, 10_000, 1),
                                // Blips
                                4 => new EntityStreamer.GlobalEntity(),
                                // Dynamic Blip
                                5 => new AltV.Net.EntitySync.SpatialPartitions.LimitedGrid3(50_000, 50_000, 175, 10_000, 10_000, 200),
                                // Ped
                                6 => new AltV.Net.EntitySync.SpatialPartitions.LimitedGrid3(50_000, 50_000, 175, 10_000, 10_000, 64),
                    _ => new AltV.Net.EntitySync.SpatialPartitions.LimitedGrid3(50_000, 50_000, 175, 10_000, 10_000, 115),
                };
            },
            new AltV.Net.EntitySync.IdProvider());

            Environment.SetEnvironmentVariable("COMPlus_legacyCorruptedState­­ExceptionsPolicy", "1");

            //Datenbank laden
            Database.DatabaseHandler.ResetDatabaseOnlineState();
            Database.DatabaseHandler.LoadAllPlayers();
            Database.DatabaseHandler.LoadAllPlayerCharacters();
            Database.DatabaseHandler.LoadAllCharacterSkins();
            Database.DatabaseHandler.LoadAllCharacterBankAccounts();
            Database.DatabaseHandler.LoadAllCharacterLastPositions();
            Database.DatabaseHandler.LoadAllCharacterInventorys();
            Database.DatabaseHandler.LoadAllCharacterLicenses();
            Database.DatabaseHandler.LoadAllCharacterPermissions();
            Database.DatabaseHandler.LoadAllCharacterMinijobData();
            Database.DatabaseHandler.LoadAllCharacterPhoneChats();
            Database.DatabaseHandler.LoadAllServerStorages();
            Database.DatabaseHandler.LoadAllCharacterWanteds();
            Database.DatabaseHandler.LoadAllServerBlips();
            Database.DatabaseHandler.LoadAllServerMarkers();
            Database.DatabaseHandler.LoadAllServerVehiclesGlobal();
            Database.DatabaseHandler.LoadAllServerAnimations();
            Database.DatabaseHandler.LoadAllServerATMs();
            Database.DatabaseHandler.LoadAllServerBanks();
            Database.DatabaseHandler.LoadAllServerBankPapers();
            Database.DatabaseHandler.LoadAllServerItems();
            Database.DatabaseHandler.LoadAllServerPeds();
            Database.DatabaseHandler.LoadAllClothesShops();
            Database.DatabaseHandler.LoadAllServerShops();
            Database.DatabaseHandler.LoadAllServerShopItems();
            Database.DatabaseHandler.LoadAllServerBarbers();
            Database.DatabaseHandler.LoadAllServerTeleports();
            Database.DatabaseHandler.LoadAllGarages();
            Database.DatabaseHandler.LoadAllGarageSlots();
            Database.DatabaseHandler.LoadAllVehicleMods();
            Database.DatabaseHandler.LoadAllVehicles();
            Database.DatabaseHandler.LoadAllVehicleTrunkItems();
            Database.DatabaseHandler.LoadAllVehicleShops();
            Database.DatabaseHandler.LoadAllVehicleShopItems();
            Database.DatabaseHandler.LoadAllServerFarmingSpots();
            Database.DatabaseHandler.LoadAllServerFarmingProducers();
            Database.DatabaseHandler.LoadAllServerJobs();
            Database.DatabaseHandler.LoadAllServerLicenses();
            Database.DatabaseHandler.LoadAllServerFuelStations();
            Database.DatabaseHandler.LoadALlServerFuelStationSpots();
            Database.DatabaseHandler.LoadAllServerTabletAppData();
            Database.DatabaseHandler.LoadAllCharactersTabletApps();
            Database.DatabaseHandler.LoadAllCharactersTabletTutorialEntrys();
            Database.DatabaseHandler.LoadAllServerTabletEvents();
            Database.DatabaseHandler.LoadAllServerTabletNotes();
            Database.DatabaseHandler.LoadAllServerCompanys();
            Database.DatabaseHandler.LoadAllServerCompanyMember();
            Database.DatabaseHandler.LoadAllServerFactions();
            Database.DatabaseHandler.LoadAllServerFactionRanks();
            Database.DatabaseHandler.LoadAllServerFactionMembers();
            Database.DatabaseHandler.LoadAllServerFactionStorageItems();
            Database.DatabaseHandler.LoadAllServerDoors();
            Database.DatabaseHandler.LoadAllServerHotels();
            Database.DatabaseHandler.LoadAllServerHouses();
            Database.DatabaseHandler.LoadAllServerMinijobBusdriverRoutes();
            Database.DatabaseHandler.LoadAllServerMinijobBusdriverRouteSpots();
            Database.DatabaseHandler.LoadAllServerMinijobGarbageSpots();
            Database.DatabaseHandler.LoadAllServerLogsFaction();
            Database.DatabaseHandler.LoadAllServerLogsCompany();
            Database.DatabaseHandler.LoadAllTattooStuff();

            // Utils
            EntityStreamer.MarkerStreamer.Create(EntityStreamer.MarkerTypes.MarkerTypeVerticalCylinder, new Vector3(Constants.Positions.weedLabor_InvPosition.X, Constants.Positions.weedLabor_InvPosition.Y, Constants.Positions.weedLabor_InvPosition.Z - 1), new Vector3(1), color: new Rgba(150, 0, 0, 100), streamRange: 15, dimension: -2147483648);
            EntityStreamer.MarkerStreamer.Create(EntityStreamer.MarkerTypes.MarkerTypeVerticalCylinder, new Vector3(Constants.Positions.storage_ExitPosition.X, Constants.Positions.storage_ExitPosition.Y, Constants.Positions.storage_ExitPosition.Z - 1), new Vector3(1), color: new Rgba(150, 0, 0, 100), streamRange: 15, dimension: -2147483648);
            EntityStreamer.MarkerStreamer.Create(EntityStreamer.MarkerTypes.MarkerTypeVerticalCylinder, new Vector3(Constants.Positions.storage_InvPosition.X, Constants.Positions.storage_InvPosition.Y, Constants.Positions.storage_InvPosition.Z - 1), new Vector3(1), color: new Rgba(150, 0, 0, 100), streamRange: 15, dimension: -2147483648);
            EntityStreamer.MarkerStreamer.Create(EntityStreamer.MarkerTypes.MarkerTypeVerticalCylinder, new Vector3(Constants.Positions.storage_LSPDInvPosition.X, Constants.Positions.storage_LSPDInvPosition.Y, Constants.Positions.storage_LSPDInvPosition.Z - 1), new Vector3(1), color: new Rgba(150, 0, 0, 100), streamRange: 15, dimension: 0);
            EntityStreamer.BlipStreamer.CreateStaticBlip("Dynasty8", 2, 0.5f, true, 374, Constants.Positions.dynasty8_pedPositionStorage, 0);
            EntityStreamer.BlipStreamer.CreateStaticBlip("Schlüsseldienst", 39, 0.5f, true, 255, Constants.Positions.Vehicleschluesseldienst_Position, 0);
            EntityStreamer.BlipStreamer.CreateStaticBlip("Waschstraße", 85, 0.5f, true, 100, Constants.Positions.Waschstrasse, 0);
            EntityStreamer.BlipStreamer.CreateStaticBlip("Waschstraße", 85, 0.5f, true, 100, Constants.Positions.Waschstrasse2, 0);
            EntityStreamer.BlipStreamer.CreateStaticBlip("Waschstraße", 85, 0.5f, true, 100, Constants.Positions.Waschstrasse3, 0);
            EntityStreamer.BlipStreamer.CreateStaticBlip("Waschstraße", 85, 0.5f, true, 100, Constants.Positions.Waschstrasse4, 0);
            EntityStreamer.BlipStreamer.CreateStaticBlip("Waschstraße", 85, 0.5f, true, 100, Constants.Positions.Waschstrasse5, 0);
            EntityStreamer.BlipStreamer.CreateStaticBlip("Waschstraße", 85, 0.5f, true, 100, Constants.Positions.Waschstrasse6, 0);
            EntityStreamer.BlipStreamer.CreateStaticBlip("Waschstraße", 85, 0.5f, true, 100, Constants.Positions.Waschstrasse7, 0);

            //Database.DatabaseHandler.RenewAll();

            Minijobs.Elektrolieferant.Main.Initialize();
            Minijobs.Pilot.Main.Initialize();
            Minijobs.Müllmann.Main.Initialize();
            Minijobs.Busfahrer.Main.Initialize();

            //Events registrieren
            Alt.OnColShape += ColAction;
            //Alt.OnClient<IPlayer, string, string>("Server:Login:ValidateLoginCredentials", LoginHandler.ValidateLoginCredentials);
            //Alt.OnClient<IPlayer, int>("Server:Charselector:PreviewCharacter", LoginHandler.PreviewCharacter);
            //Alt.OnClient<IPlayer, string, string>("Server:Charselector:spawnChar", Handler.LoginHandler.CharacterSelectedSpawnPlace);
            //Alt.OnClient<IPlayer, int>("Server:Charselector:KillCharacter", Characters.KillCharacter);
            //Alt.OnClient<IPlayer, string, string, string, string>("Server:Register:RegisterNewPlayer", Handler.RegisterHandler.RegisterNewPlayer);
            //Alt.OnClient<IPlayer>("Server:Charcreator:CreateCEF", Handler.CharCreatorHandler.CreateCefBrowser);
            //Alt.OnClient<IPlayer, string, string, bool, string, string, string>("Server:Charcreator:CreateCharacter", Handler.CharCreatorHandler.CreateCharacter);
            /*Alt.OnClient<IPlayer, float, float, float>("ServerBlip:TpWayPoint", tptoWaypoint); //ToDo: entfernen*/
            //Alt.OnClient<IPlayer>("Server:Inventory:RequestInventoryItems", Handler.InventoryHandler.RequestInventoryItems);
            //Alt.OnClient<IPlayer, string, int, string, string>("Server:Inventory:switchItemToDifferentInv", Handler.InventoryHandler.switchItemToDifferentInv);
            //Alt.OnClient<IPlayer, string, int, string>("Server:Inventory:UseItem", Handler.InventoryHandler.UseItem);
            //Alt.OnClient<IPlayer, string, int, string>("Server:Inventory:DropItem", Handler.InventoryHandler.DropItem);
            //Alt.OnClient<IPlayer, string, int, string, int>("Server:Inventory:GiveItem", InventoryHandler.GiveItem);
            //Alt.OnClient<IPlayer>("Server:KeyHandler:PressE", Handler.KeyHandler.PressE);
            //Alt.OnClient<IPlayer>("Server:KeyHandler:PressU", KeyHandler.PressU);
            //Alt.OnClient<IPlayer, string>("Server:HUD:sendIdentityCardApplyForm", Handler.TownhallHandler.sendIdentityCardApplyForm);
            //Alt.OnClient<IPlayer, string>("Server:Bank:CreateNewBankAccount", Handler.BankHandler.CreateNewBankAccount);
            //Alt.OnClient<IPlayer, string, string>("Server:Bank:BankAccountAction", Handler.BankHandler.BankAccountAction);
            //Alt.OnClient<IPlayer>("Server:Inventory:closeCEF", Handler.InventoryHandler.CloseInventoryCEF);
            //Alt.OnClient<IPlayer, bool>("Server:CEF:setCefStatus", setCefStatus);
            //Alt.OnClient<IPlayer, int>("Server:ATM:requestBankData", Handler.BankHandler.requestATMBankData);
            //Alt.OnClient<IPlayer, int, int, string>("Server:ATM:WithdrawMoney", Handler.BankHandler.WithdrawATMMoney);
            //Alt.OnClient<IPlayer, int, int, string>("Server:ATM:DepositMoney", Handler.BankHandler.DepositATMMoney);
            //Alt.OnClient<IPlayer, int, int, int, string>("Server:ATM:TransferMoney", Handler.BankHandler.TransferATMMoney);
            //Alt.OnClient<IPlayer, string, int>("Server:ATM:TryPin", Handler.BankHandler.TryATMPin);
            //Alt.OnClient<IPlayer, int, int, string>("Server:Shop:buyItem", Handler.ShopHandler.buyShopItem);
            //Alt.OnClient<IPlayer, int, int, string>("Server:Shop:sellItem", ShopHandler.sellShopItem);
            //Alt.OnClient<IPlayer, string>("Server:Barber:finishBarber", Handler.CharCreatorHandler.finishBarber);
            //Alt.OnClient<IPlayer>("Server:Barber:RequestCurrentSkin", Handler.CharCreatorHandler.SetCorrectCharacterSkin);
            //Alt.OnClient<IPlayer, IVehicle>("Server:Raycast:LockVehicle", RaycastHandler.LockVehicle);
            //Alt.OnClient<IPlayer, IVehicle>("Server:Raycast:ToggleVehicleEngine", RaycastHandler.ToggleVehicleEngine);
            //Alt.OnClient<IPlayer, IVehicle>("Server:Raycast:OpenVehicleFuelMenu", RaycastHandler.OpenVehicleFuelMenu);
            //Alt.OnClient<IPlayer, IVehicle>("Server:Raycast:OpenCloseVehicleTrunk", RaycastHandler.OpenCloseVehicleTrunk);
            //Alt.OnClient<IPlayer, IVehicle>("Server:Raycast:ViewVehicleTrunk", RaycastHandler.ViewVehicleTrunk);
            //Alt.OnClient<IPlayer, IVehicle>("Server:Raycast:ViewVehicleGlovebox", RaycastHandler.ViewVehicleGlovebox);
            //Alt.OnClient<IPlayer, int, int, string, int, string, string>("Server:VehicleTrunk:StorageItem", VehicleHandler.VehicleTrunkStorageItem);
            //Alt.OnClient<IPlayer, int, int, string, int, string>("Server:VehicleTrunk:TakeItem", VehicleHandler.VehicleTrunkTakeItem);
            //Alt.OnClient<IPlayer, IPlayer>("Server:Raycast:showPlayerSupportId", RaycastHandler.showPlayerSupportId);
            //Alt.OnClient<IPlayer, IPlayer, string>("Server:Raycast:OpenGivePlayerBillCEF", RaycastHandler.OpenGivePlayerBillCEF);
            //Alt.OnClient<IPlayer, IPlayer>("Server:Raycast:RevivePlayer", Factions.LSFD.Functions.RevivePlayer);
            //Alt.OnClient<IPlayer, string, string, int, int>("Server:PlayerBill:giveBill", RaycastHandler.PlayerBillGiveBill);
            //Alt.OnClient<IPlayer, string, string, int, int, string, int>("Server:PlayerBill:BillAction", RaycastHandler.PlayerBillAction);
            //Alt.OnClient<IPlayer, IPlayer>("Server:Raycast:givePlayerItemRequest", RaycastHandler.givePlayerItemRequest);
            //Alt.OnClient<IPlayer, IPlayer>("Server:Raycast:GiveTakeHandcuffs", RaycastHandler.GiveTakeHandcuffs);
            //Alt.OnClient<IPlayer, IPlayer>("Server:Raycast:GiveTakeRopeCuffs", RaycastHandler.GiveTakeRopeCuffs);
            //Alt.OnClient<IPlayer, IPlayer>("Server:Raycast:SearchPlayerInventory", RaycastHandler.SearchPlayerInventory);
            //Alt.OnClient<IPlayer, int, string, string, int>("Server:PlayerSearch:TakeItem", InventoryHandler.PlayerSearchTakeItem);
            //Alt.OnClient<IPlayer, IPlayer>("Server:Raycast:openGivePlayerLicenseCEF", RaycastHandler.openGivePlayerLicenseCEF);
            //Alt.OnClient<IPlayer, int, string, int>("Server:Garage:DoAction", GarageHandler.DoGarageAction);
            //Alt.OnClient<IPlayer, int, string>("Server:VehicleShop:BuyVehicle", ShopHandler.BuyVehicle);
            //Alt.OnClient<IPlayer, float>("Server:Vehicle:UpdateVehicleKM", HUDHandler.UpdateVehicleKM);
            //Alt.OnClient<IPlayer, string>("Server:Jobcenter:SelectJob", TownhallHandler.SelectJobcenterJob);
            //Alt.OnClient<IPlayer, int, int, string, int, int>("Server:FuelStation:FuelVehicleAction", FuelStationHandler.FuelVehicle);
            //Alt.OnClient<IPlayer>("Server:ClothesShop:RequestCurrentSkin", Characters.SetCharacterCorrectClothes);
            //Alt.OnClient<IPlayer, int, int, string>("Server:ClothesShop:buyItem", ShopHandler.buyShopItem);
            //Alt.OnClient<IPlayer>("Server:Tablet:openCEF", TabletHandler.openCEF);
            //Alt.OnClient<IPlayer>("Server:Tablet:RequestTabletData", TabletHandler.RequestTabletData);
            //Alt.OnClient<IPlayer, string, bool>("Server:Tablet:AppStoreInstallUninstallApp", TabletHandler.AppStoreInstallUninstallApp);
            //Alt.OnClient<IPlayer, int, string, int>("Server:Tablet:BankingAppNewTransaction", TabletHandler.BankingAppNewTransaction);
            //Alt.OnClient<IPlayer, string, string, string, string, string, string, string>("Server:Tablet:EventsAppNewEntry", TabletHandler.EventsAppNewEntry);
            //Alt.OnClient<IPlayer, string, string, string>("Server:Tablet:NotesAppNewNote", TabletHandler.NotesAppNewNote);
            //Alt.OnClient<IPlayer, int>("Server:Tablet:NotesAppDeleteNote", TabletHandler.NotesAppDeleteNote);
            //Alt.OnClient<IPlayer, string, int, string>("Server:Tablet:VehicleStoreBuyVehicle", TabletHandler.VehicleStoreBuyVehicle);
            //Alt.OnClient<IPlayer, string, int>("Server:Tablet:CompanyAppInviteNewMember", TabletHandler.CompanyAppInviteNewMember);
            //Alt.OnClient<IPlayer>("Server:Tablet:CompanyAppLeaveCompany", TabletHandler.CompanyAppLeaveCompany);
            //Alt.OnClient<IPlayer, int, int>("Server:Tablet:CompanyAppRankAction", TabletHandler.CompanyAppRankAction);
            //Alt.OnClient<IPlayer, string, int, int>("Server:FactionBank:DepositMoney", BankHandler.DepositFactionMoney);
            //Alt.OnClient<IPlayer, string, int, int>("Server:FactionBank:WithdrawMoney", BankHandler.WithdrawFactionMoney);
            //Alt.OnClient<IPlayer, string, int, int>("Server:Tablet:FactionManagerAppInviteNewMember", TabletHandler.FactionManagerAppInviteNewMember);
            //Alt.OnClient<IPlayer, string, int>("Server:Tablet:FactionManagerRankAction", TabletHandler.FactionManagerRankAction);
            //Alt.OnClient<IPlayer, int, int>("Server:Tablet:FactionManagerSetRankPaycheck", TabletHandler.FactionManagerSetRankPaycheck);
            //Alt.OnClient<IPlayer, int, int, string, int, string>("Server:FactionStorage:StorageItem", FactionHandler.FactionStorageStorageItem);
            //Alt.OnClient<IPlayer, int, int, string, int>("Server:FactionStorage:TakeItem", FactionHandler.FactionStorageTakeItem);
            //Alt.OnClient<IPlayer, string, IVehicle>("Server:InteractionMenu:GetMenuVehicleItems", RaycastHandler.GetMenuVehicleItems);
            //Alt.OnClient<IPlayer, string, IPlayer>("Server:InteractionMenu:GetMenuPlayerItems", RaycastHandler.GetMenuPlayerItems);
            //Alt.OnClient<IPlayer, string, int, string, string>("Server:VehicleLicensing:LicensingAction", VehicleHandler.LicensingAction);
            //Alt.OnClient<IPlayer, string>("Server:Tablet:LSPDAppSearchPerson", Factions.LSPD.Functions.LSPDAppSearchPerson);
            //Alt.OnClient<IPlayer, string>("Server:Tablet:LSPDAppSearchVehiclePlate", Factions.LSPD.Functions.LSPDAppSearchVehiclePlate);
            //Alt.OnClient<IPlayer, string>("Server:Tablet:LSPDAppSearchLicense", Factions.LSPD.Functions.LSPDAppSearchLicense);
            //Alt.OnClient<IPlayer, string, string>("Server:Tablet:LSPDAppTakeLicense", Factions.LSPD.Functions.LSPDAppTakeLicense);
            //Alt.OnClient<IPlayer, int, string>("Server:GivePlayerLicense:GiveLicense", Factions.LSFS.Functions.GiveLicense);
            //Alt.OnClient<IPlayer, string>("Server:Tablet:JusticeAppGiveWeaponLicense", Factions.Justice.Functions.GiveWeaponLicense);
            //Alt.OnClient<IPlayer, string>("Server:Tablet:JusticeAppSearchBankAccounts", Factions.Justice.Functions.SearchBankAccounts);
            //Alt.OnClient<IPlayer, int>("Server:Tablet:JusticeAppViewBankTransactions", Factions.Justice.Functions.ViewBankTransactions);
            //Alt.OnClient<IPlayer, int>("Server:MinijobPilot:StartJob", Minijobs.Pilot.Main.StartMiniJob);
            //Alt.OnClient<IPlayer, int>("Server:MinijobBusdriver:StartJob", Minijobs.Busfahrer.Main.StartMiniJob);
            //Alt.OnClient<IPlayer, int, int>("Server:Hotel:RentHotel", HotelHandler.RentHotel);
            //Alt.OnClient<IPlayer, int, int>("Server:Hotel:LockHotel", HotelHandler.LockHotel);
            //Alt.OnClient<IPlayer, int, int>("Server:Hotel:EnterHotel", HotelHandler.EnterHotel);
            //Alt.OnClient<IPlayer, int, string, int, string>("Server:HotelStorage:StorageItem", HotelHandler.StorageHotelItem);
            //Alt.OnClient<IPlayer, int, string, int>("Server:HotelStorage:TakeItem", HotelHandler.TakeHotelItem);
            //Alt.OnClient<IPlayer, int>("Server:House:BuyHouse", HouseHandler.BuyHouse);
            //Alt.OnClient<IPlayer, int>("Server:House:EnterHouse", HouseHandler.EnterHouse);
            //Alt.OnClient<IPlayer, int, string, int, string>("Server:HouseStorage:StorageItem", HouseHandler.StorageItem);
            //Alt.OnClient<IPlayer, int, string, int>("Server:HouseStorage:TakeItem", HouseHandler.TakeItem);
            //Alt.OnClient<IPlayer, int>("Server:House:RentHouse", HouseHandler.RentHouse);
            //Alt.OnClient<IPlayer, int>("Server:House:UnrentHouse", HouseHandler.UnrentHouse);
            //Alt.OnClient<IPlayer, int, int>("Server:HouseManage:setRentPrice", HouseHandler.setRentPrice);
            //Alt.OnClient<IPlayer, int, string>("Server:HouseManage:setRentState", HouseHandler.setRentState);
            //Alt.OnClient<IPlayer, int, int>("Server:HouseManage:RemoveRenter", HouseHandler.RemoveRenter);
            //Alt.OnClient<IPlayer, int, string>("Server:HouseManage:BuyUpgrade", HouseHandler.BuyUpgrade);
            //Alt.OnClient<IPlayer, int, int>("Server:HouseManage:WithdrawMoney", HouseHandler.WithdrawMoney);
            //Alt.OnClient<IPlayer, int, int>("Server:HouseManage:DepositMoney", HouseHandler.DepositMoney);
            //Alt.OnClient<IPlayer, int, string>("Server:Tablet:sendDispatchToFaction", TabletHandler.sendDispatchToFaction);
            //Alt.OnClient<IPlayer, int, int>("Server:Tablet:DeleteFactionDispatch", TabletHandler.DeleteFactionDispatch);
            //Alt.OnClient<IPlayer, IPlayer>("Server:Raycast:showIdcard", RaycastHandler.showIdCard);
            //Alt.OnClient<IPlayer, int>("Server:House:SellHouse", HouseHandler.SellHouse);
            //Alt.OnClient<IPlayer, int>("Server:House:setMainHouse", HouseHandler.setMainHouse);
            //Alt.OnClient<IPlayer, IPlayer>("Server:Raycast:healPlayer", Factions.LSFD.Functions.HealPlayer);
            //Alt.OnClient<IPlayer, IVehicle>("Server:Raycast:RepairVehicle", Factions.ACLS.Functions.RepairVehicle);
            //Alt.OnClient<IPlayer, IVehicle>("Server:Raycast:towVehicle", Factions.ACLS.Functions.TowVehicle);
            //Alt.OnClient<IPlayer, IVehicle>("Server:Raycast:tuneVehicle", Factions.ACLS.Functions.openTuningMenu);
            //Alt.OnClient<IPlayer, IVehicle, string, string, int, int, int>("Server:Tuning:switchTuningColor", Factions.ACLS.Functions.switchTuningColor);
            //Alt.OnClient<IPlayer, IVehicle>("Server:Tuning:resetToNormal", Factions.ACLS.Functions.resetTuningToNormal);
            //Alt.OnClient<IPlayer, IVehicle, string, int, string>("Server:Tuning:switchTuning", Factions.ACLS.Functions.switchTuning);
            Alt.OnClient<IPlayer, string>("Server:Utilities:BanMe", banme);

            //Timer initialisieren
            System.Timers.Timer entityTimer = new System.Timers.Timer();
            System.Timers.Timer kilometerTimer = new System.Timers.Timer();
            System.Timers.Timer hudTimer = new System.Timers.Timer();
            System.Timers.Timer LaborTimer = new System.Timers.Timer();
            System.Timers.Timer fuelTimer = new System.Timers.Timer();
            System.Timers.Timer desireTimer = new System.Timers.Timer();
            System.Timers.Timer VehicleAutomaticParkFetchTimer = new System.Timers.Timer();
            //Elapsed?
            entityTimer.Elapsed += new ElapsedEventHandler(TimerHandler.OnEntityTimer);
            kilometerTimer.Elapsed += new ElapsedEventHandler(TimerHandler.OnKilometerTimer);
            hudTimer.Elapsed += new ElapsedEventHandler(TimerHandler.OnHUDTimer);
            LaborTimer.Elapsed += new ElapsedEventHandler(TimerHandler.LaborTimer);
            fuelTimer.Elapsed += new ElapsedEventHandler(TimerHandler.OnFuelTimer);
            desireTimer.Elapsed += new ElapsedEventHandler(TimerHandler.OnDesireTimer);
            VehicleAutomaticParkFetchTimer.Elapsed += new ElapsedEventHandler(TimerHandler.VehicleAutomaticParkFetch);
            //Interval
            entityTimer.Interval += 60000;
            kilometerTimer.Interval += 30000;
            hudTimer.Interval += 2500;
            fuelTimer.Interval += 30000;
            LaborTimer.Interval += 60000;
            desireTimer.Interval += 200000;
            VehicleAutomaticParkFetchTimer.Interval += 60000 * 5;
            //enabled?
            entityTimer.Enabled = true;
            kilometerTimer.Enabled = false;
            hudTimer.Enabled = true;
            fuelTimer.Enabled = true;
            desireTimer.Enabled = true;
            VehicleAutomaticParkFetchTimer.Enabled = true;
            LaborTimer.Enabled = true;

            // Dynasty8
            EntityStreamer.PedStreamer.Create("a_m_y_business_02", Constants.Positions.dynasty8_pedPositionStorage, new System.Numerics.Vector3(0, 0, Constants.Positions.dynasty8_pedRotationStorage), 0);
            EntityStreamer.MarkerStreamer.Create(EntityStreamer.MarkerTypes.MarkerTypeVerticalCylinder, Constants.Positions.dynasty8_positionStorage, new System.Numerics.Vector3(1), color: new Rgba(255, 51, 51, 100), dimension: 0, streamRange: 20);
            EntityStreamer.HelpTextStreamer.Create("Drücke E um eine Lagerhalle zu erwerben oder deine zu verkaufen.", Constants.Positions.dynasty8_positionStorage, streamRange: 1);



            Console.WriteLine($"Main-Thread = {Thread.CurrentThread.ManagedThreadId}");
        }

        private void banme(IPlayer player, string msg)
        {
            try
            {
                if (player == null || !player.Exists || player.AdminLevel() != 0) return;
                Alt.Log($"Ban Me: {player.Name} - {DateTime.Now.ToString()}");
                int charId = User.GetPlayerOnline(player);
                player.Kick("");
                if (charId <= 0) return;
                User.SetPlayerBanned(Characters.GetCharacterAccountId(charId), true, $"Grund: {msg}");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        private void ColAction(IColShape colShape, IEntity targetEntity, bool state)
        {
            if (colShape == null) return;
            if (!colShape.Exists) return;
            IPlayer client = targetEntity as IPlayer;
            if (client == null || !client.Exists) return;
            string colshapeName = colShape.GetColShapeName();
            ulong colshapeId = colShape.GetColShapeId();

            if(colshapeName == "Cardealer" && state == true)
            {
                ulong vehprice = colShape.GetColshapeCarDealerVehPrice();
                string vehname = colShape.GetColshapeCarDealerVehName();
                HUDHandler.SendNotification(client, 1, 2500, $"Name: {vehname}<br>Preis: {vehprice}$");
                return;
            }
            else if(colshapeName == "DoorShape" && state)
            {
                var doorData = ServerDoors.ServerDoors_.FirstOrDefault(x => x.id == (int)colshapeId);
                if (doorData == null) return;
                client.EmitLocked("Client:DoorManager:ManageDoor", doorData.hash, new Position(doorData.posX, doorData.posY, doorData.posZ), (bool)doorData.state);
            }
        }

        private void tptoWaypoint(IPlayer player, float x, float y, float z) //ToDo: entfernen
        {
            if (player == null) return;
            player.Position = new Position(x, y, z);
        }     

        public override void OnStop()
        {
            foreach (var player in Alt.GetAllPlayers().Where(p => p != null && p.Exists)) player.Kick("Server wird heruntergefahren...");
            Alt.Log("Server ist gestoppt.");
        }        
    }
}
