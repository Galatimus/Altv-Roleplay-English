using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Altv_Roleplay.Factories;
using Altv_Roleplay.Model;
using Altv_Roleplay.Services;
using Altv_Roleplay.Utils;

namespace Altv_Roleplay.Handler
{
    class AnimationMenuHandler : IScript
    {
        [AsyncClientEvent("Server:AnimationMenu:GetAnimationItems")]
        public async Task GetAnimationItems(IPlayer player)
        {
            try
            {
                var interactHTML = ""; 
                interactHTML += "<li><p id='InteractionMenu-SelectedTitle'>Schließen</p></li><li class='interactitem' data-action='close' data-actionstring='Abbrechen'><img src='../utils/img/cancel.png'></li>";

                interactHTML += "<li class='interactitem' id='InteractionMenu-crossarms3' data-action='crossarms3' data-actionstring='Arme verschränken'><img src='../utils/img/crossarms.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-facepalm' data-action='facepalm' data-actionstring='Facepalm'><img src='../utils/img/facepalm.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-finger2' data-action='finger2' data-actionstring='Mittelfinger'><img src='../utils/img/finger.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-wait5' data-action='wait5' data-actionstring='Warten'><img src='../utils/img/wait.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-hug3' data-action='hug3' data-actionstring='Umarmen'><img src='../utils/img/hug.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-inspect' data-action='inspect' data-actionstring='Untersuchen'><img src='../utils/img/inspect.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-kneel2' data-action='kneel2' data-actionstring='Knien'><img src='../utils/img/kneel.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-lean4' data-action='lean4' data-actionstring='Anlehnen'><img src='../utils/img/lean.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-mechanic' data-action='mechanic' data-actionstring='Fahrzeug Reparieren'><img src='../utils/img/mechanic.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-pushup' data-action='pushup' data-actionstring='Liegestütze'><img src='../utils/img/pushup.png'></li>";

                player.EmitLocked("Client:AnimationMenu:SetMenuItems", interactHTML);
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:AnimationMenuPage2:GetAnimationItems")]
        public async Task GetAnimationItemsPage2(IPlayer player)
        {
            try
            {
                var interactHTML = "";
                interactHTML += "<li><p id='InteractionMenu-SelectedTitle'>Schließen</p></li><li class='interactitem' data-action='close' data-actionstring='Abbrechen'><img src='../utils/img/cancel.png'></li>";

                interactHTML += "<li class='interactitem' id='InteractionMenu-dancef6' data-action='dancef6' data-actionstring='Tanzen'><img src='../utils/img/dance.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-dance3' data-action='dance3' data-actionstring='Tanzen 2'><img src='../utils/img/dance.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-danceupper' data-action='danceupper' data-actionstring='Tanzen 3'><img src='../utils/img/dance.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-dance6' data-action='dance6' data-actionstring='Tanzen 4'><img src='../utils/img/dance.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-dance5' data-action='dance5' data-actionstring='Tanzen 5'><img src='../utils/img/dance.png'></li>";

                interactHTML += "<li class='interactitem' id='InteractionMenu-danceslow2' data-action='danceslow2' data-actionstring='Langsam Tanzen'><img src='../utils/img/dance.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-danceshy' data-action='danceshy' data-actionstring='Zurückhaltend Tanzen'><img src='../utils/img/dance.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-dancesilly' data-action='dancesilly' data-actionstring='Dumm Tanzen'><img src='../utils/img/dance.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-dancesilly4' data-action='dancesilly4' data-actionstring='Dumm Tanzen 2'><img src='../utils/img/dance.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-dancesilly5' data-action='dancesilly5' data-actionstring='Dumm Tanzen 3'><img src='../utils/img/dance.png'></li>";
                

                player.EmitLocked("Client:AnimationMenuPage2:SetMenuItems", interactHTML); 
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:AnimationMenuPage3:GetAnimationItems")]
        public async Task GetAnimationItemsPage3(IPlayer player)
        {
            try
            {
                var interactHTML = "";
                interactHTML += "<li><p id='InteractionMenu-SelectedTitle'>Schließen</p></li><li class='interactitem' data-action='close' data-actionstring='Normal'><img src='../utils/img/cancel.png'></li>";

                interactHTML += "<li class='interactitem' id='InteractionMenu-injured' data-action='injured' data-actionstring='Verletzt'><img src='../utils/img/injured.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-arrogant' data-action='arrogant' data-actionstring='Arrogant'><img src='../utils/img/arrogant.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-casual' data-action='casual' data-actionstring='Lässig'><img src='../utils/img/casual.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-casual4' data-action='casual4' data-actionstring='Sportlich'><img src='../utils/img/casual2.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-confident' data-action='confident' data-actionstring='Zuversichtlich'><img src='../utils/img/confident.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-drunk' data-action='drunk' data-actionstring='Betrunken'><img src='../utils/img/drunk.png'></li>";

                interactHTML += "<li class='interactitem' id='InteractionMenu-gangster' data-action='gangster' data-actionstring='Gangster'><img src='../utils/img/gangster.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-gangster2' data-action='gangster2' data-actionstring='Gangster 2'><img src='../utils/img/gangster.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-cop' data-action='cop' data-actionstring='Polizist'><img src='../utils/img/cop.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-cop2' data-action='cop2' data-actionstring='Polizist 2'><img src='../utils/img/cop.png'></li>";
                

                player.EmitLocked("Client:AnimationMenuPage3:SetMenuItems", interactHTML); 
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
    }
}
