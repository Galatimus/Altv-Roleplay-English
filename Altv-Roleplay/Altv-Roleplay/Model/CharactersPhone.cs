using AltV.Net;
using Altv_Roleplay.Factories;
using Altv_Roleplay.models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AltV.Net.Async;
using Newtonsoft.Json;

namespace Altv_Roleplay.Model
{
    class CharactersPhone : IScript
    {
        public static List<CharactersPhoneChats> CharactersPhoneChats_ = new List<CharactersPhoneChats>();
        public static List<CharactersPhoneVerlauf> CharactersPhoneVerlauf_ = new List<CharactersPhoneVerlauf>();
        public static List<CharactersPhoneChatMessages> CharactersPhoneChatMessages_ = new List<CharactersPhoneChatMessages>();
        public static List<CharactersPhoneContacts> CharactersPhoneContacts_ = new List<CharactersPhoneContacts>();

        public static void CreatePhoneChat(int phoneNumber, int targetPhoneNumber)
        {
            try
            {
                var chatData = new CharactersPhoneChats()
                {
                    phoneNumber = phoneNumber,
                    anotherNumber = targetPhoneNumber
                };

                CharactersPhoneChats_.Add(chatData);
                using (gtaContext db = new gtaContext())
                {
                    db.CharactersPhoneChats.Add(chatData);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
        public static void CreatePhoneVerlauf(ClassicPlayer player, int phoneNumber, int targetPhoneNumber)
        {
            try
            {
                var chatData = new CharactersPhoneVerlauf()
                {
                    charId = player.CharacterId,
                    phoneNumber = phoneNumber,
                    anotherNumber = targetPhoneNumber,
                    date = DateTime.Now
                };

                CharactersPhoneVerlauf_.Add(chatData);
                using (gtaContext db = new gtaContext())
                {
                    db.CharactersPhoneVerlauf.Add(chatData);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void CreatePhoneContact(int phoneNumber, string contactName, int contactNumber)
        {
            try
            {
                var contactData = new CharactersPhoneContacts()
                {
                    phoneNumber = phoneNumber,
                    contactName = contactName,
                    contactNumber = contactNumber
                };
                CharactersPhoneContacts_.Add(contactData);
                using (gtaContext db = new gtaContext())
                {
                    db.CharactersPhoneContacts.Add(contactData);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void CreatePhoneChatMessage(int chatId, int fromNumber, int toNumber, int unix, string message)
        {
            try
            {
                var messageData = new CharactersPhoneChatMessages()
                {
                    chatId = chatId,
                    fromNumber = fromNumber,
                    toNumber = toNumber,
                    unix = unix,
                    message = message
                };

                CharactersPhoneChatMessages_.Add(messageData);
                using (gtaContext db = new gtaContext())
                {
                    db.CharactersPhoneChatMessages.Add(messageData);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void DeletePhoneChat(int chatId)
        {
            try
            {
                using (var db = new gtaContext())
                {
                    var chatData = CharactersPhoneChats_.ToList().FirstOrDefault(x => x.chatId == chatId);
                    if(chatData != null)
                    {                        
                        db.CharactersPhoneChats.Remove(chatData);
                        CharactersPhoneChats_.Remove(chatData);
                    }

                    foreach (var message in CharactersPhoneChatMessages_.ToList().Where(x => x.chatId == chatId))
                    {                        
                        db.CharactersPhoneChatMessages.Remove(message);
                        CharactersPhoneChatMessages_.Remove(message);
                    }
                    db.SaveChanges();
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void DeletePhoneContact(int contactId, int phoneNumber)
        {
            try
            {
                var contactData = CharactersPhoneContacts_.ToList().FirstOrDefault(x => x.contactId == contactId && x.phoneNumber == phoneNumber);
                if (contactData == null) return;
                using (var db = new gtaContext())
                {
                    db.CharactersPhoneContacts.Remove(contactData);
                    CharactersPhoneContacts_.Remove(contactData);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void EditContact(int contactId, int newPhoneNumber, string newName)
        {
            try
            {
                var contactData = CharactersPhoneContacts_.FirstOrDefault(x => x.contactId == contactId);
                if (contactData == null) return;
                contactData.contactNumber = newPhoneNumber;
                contactData.contactName = newName;
                using (var db = new gtaContext())
                {
                    db.CharactersPhoneContacts.Update(contactData);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static bool ExistContactById(int contactId, int phoneNumber)
        {
            try
            {
                return CharactersPhoneContacts_.ToList().Exists(x => x.contactId == contactId && x.phoneNumber == phoneNumber);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static bool ExistContactByName(int phoneNumber, string contactName)
        {
            try
            {
                return CharactersPhoneContacts_.ToList().Exists(x => x.phoneNumber == phoneNumber && x.contactName == contactName);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static bool ExistContactByNumber(int phoneNumber, int contactNumber)
        {
            try
            {
                return CharactersPhoneContacts_.ToList().Exists(x => x.phoneNumber == phoneNumber && x.contactNumber == contactNumber);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static bool ExistChatByNumbers(int phoneNumber, int anotherNumber)
        {
            try
            {
                return CharactersPhoneChats_.ToList().Exists(x => (x.phoneNumber == phoneNumber && x.anotherNumber == anotherNumber) || (x.phoneNumber == anotherNumber && x.anotherNumber == phoneNumber));
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static bool ExistChatById(int chatId)
        {
            try
            {
                return CharactersPhoneChats_.ToList().Exists(x => x.chatId == chatId);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static void RequestChatJSON(ClassicPlayer player, int playerNumber)
        {
            try
            {
                if (player == null || !player.Exists || player.CharacterId <= 0 || playerNumber <= 0) return;
                var chats = CharactersPhoneChats_.ToList().Where(x => x.phoneNumber == playerNumber || x.anotherNumber == playerNumber).Select(x => new
                {
                    x.chatId,
                    from = x.phoneNumber,
                    to = x.anotherNumber,
                    unix = GetLastMessageUnix(x.chatId),
                    text = GetLastMessageText(x.chatId),
                }).OrderByDescending(x => x.unix).ToList();

                var itemCount = (int)chats.Count;
                var iterations = Math.Floor((decimal)(itemCount / 5));
                var rest = itemCount % 5;

                for(var i = 0; i < iterations; i++)
                {
                    var skip = i * 5;
                    player.EmitLocked("Client:Smartphone:addChatJSON", JsonConvert.SerializeObject(chats.Skip(skip).Take(5).ToList()));
                }
                if (rest != 0) player.EmitLocked("Client:Smartphone:addChatJSON", JsonConvert.SerializeObject(chats.Skip((int)iterations * 5).ToList()));
                player.EmitLocked("Client:Smartphone:setAllChats");
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static int GetLastMessageUnix(int chatId)
        {
            try
            {
                var lastMessage = CharactersPhoneChatMessages_.OrderBy(x => x.unix).ToList().LastOrDefault(x => x.chatId == chatId);
                if (lastMessage != null) return lastMessage.unix;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static string GetLastMessageText(int chatId)
        {
            try
            {
                var lastMessage = CharactersPhoneChatMessages_.OrderBy(x => x.unix).ToList().LastOrDefault(x => x.chatId == chatId);
                if (lastMessage != null) return lastMessage.message;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return "";
        }
    }
}
 