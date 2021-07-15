using Newtonsoft.Json.Linq;
using System;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace anonUz
{
    class Program
    {
        static TelegramBotClient Bot = new TelegramBotClient(api);

        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            
            Bot.StartReceiving();

            Bot.OnUpdate += Bot_OnUpdateAsync;
            Console.Title = Bot.GetMeAsync().Result.FirstName;
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static async void Bot_OnUpdateAsync(object sender, Telegram.Bot.Args.UpdateEventArgs e)
        {
            
            
            if (e.Update.Type == UpdateType.Message)
            {
                var from = e.Update.Message.From;
                
                Console.WriteLine($"{from.Id:###############}|{from.FirstName}") ;
                if (!Directory.Exists($"./history/{from.Id}"))
                {
                    Directory.CreateDirectory($"./history/{from.Id}");
                }
                if (e.Update.Message.Text == "/start")
                {
                    await Bot.SendTextMessageAsync(from.Id, Configurate(hello, from));
                }
                else if (e.Update.Message.Text == next)
                {
                    await Bot.SendTextMessageAsync(from.Id, Configurate(searching,from));
                    Find(from.Id);
                }
                else if (e.Update.Message.Text == search)
                {
                    await Bot.SendTextMessageAsync(from.Id, Configurate(searching,from));
                    Find(from.Id);
                }
                else if (e.Update.Message.Text == cancel)
                {
                    var next1 = long.Parse(File.ReadAllText("next"));
                    if (next1 == from.Id)
                    {
                        File.WriteAllText("next", "");
                    }
                    await Bot.SendTextMessageAsync(from.Id, Configurate(canceled, from));
                }
                else if (e.Update.Message.Text == stop)
                {
                    if (File.Exists($"./Chats/{from.Id}"))
                    {
                        await Bot.SendTextMessageAsync(from.Id, Configurate(chatEnd, from));
                        var next = int.Parse(File.ReadAllText($"./Chats/{from.Id}"));
                        await Bot.SendTextMessageAsync(next, Configurate(chatEnded, from));

                        File.AppendAllText($"./history/{from.Id}/{next}", $"-----------END({next})-----------\n");
                        File.AppendAllText($"./history/{next}/{from.Id}", $"-----------END({from.Id})-----------\n");

                        File.Delete($"./Chats/{from.Id}");
                        File.Delete($"./Chats/{next}");
                    }
                    else
                    {
                        await Bot.SendTextMessageAsync(from.Id, Configurate(noInterlocutor, from));
                    }
                }
                else
                {
                    if (File.Exists($"./Chats/{from.Id}"))
                    {
                        var next = int.Parse(File.ReadAllText($"./Chats/{from.Id}"));
                        InputOnlineFile iof;
                        string id, cap;
                        switch (e.Update.Message.Type)
                        {
                            case MessageType.Text:
                                await Bot.SendTextMessageAsync(next, e.Update.Message.Text);
                                File.AppendAllText($"./history/{from.Id}/{next}", $"[*]{e.Update.Message.Text}\n");
                                File.AppendAllText($"./history/{next}/{from.Id}", $"[/]{e.Update.Message.Text}\n");
                                break;
                            case MessageType.Photo:
                                var photo = e.Update.Message.Photo;
                                cap = e.Update.Message.Caption;
                                var p = photo[photo.Length - 1].FileId;
                                iof = new InputOnlineFile(p);

                                await Bot.SendPhotoAsync(next, iof, caption: cap);
                                File.AppendAllText($"./history/{from.Id}/{next}", $"[*]Photo({e.Update.Message.Caption}|{p})\n");
                                File.AppendAllText($"./history/{next}/{from.Id}", $"[/]Photo({e.Update.Message.Caption}|{p})\n");
                                break;
                            case MessageType.Audio:
                                var audio = e.Update.Message.Audio.FileId;
                                cap = e.Update.Message.Caption;
                                iof = new InputOnlineFile(audio);

                                await Bot.SendAudioAsync(next, iof, caption: cap);
                                File.AppendAllText($"./history/{from.Id}/{next}", $"[*]Audio({e.Update.Message.Caption}|{audio})\n");
                                File.AppendAllText($"./history/{next}/{from.Id}", $"[/]Audio({e.Update.Message.Caption}|{audio})\n");
                                break;
                            case MessageType.Video:
                                var video = e.Update.Message.Video.FileId;
                                cap = e.Update.Message.Caption;
                                iof = new InputOnlineFile(video);

                                await Bot.SendVideoAsync(next, iof, caption: cap);
                                File.AppendAllText($"./history/{from.Id}/{next}", $"[*]Video({e.Update.Message.Caption}|{video})\n");
                                File.AppendAllText($"./history/{next}/{from.Id}", $"[/]Video({e.Update.Message.Caption}|{video})\n");
                                break;
                            case MessageType.Voice:
                                id = e.Update.Message.Voice.FileId;
                                cap = e.Update.Message.Caption;
                                iof = new InputOnlineFile(id);

                                await Bot.SendVoiceAsync(next, iof, caption: cap);
                                File.AppendAllText($"./history/{from.Id}/{next}", $"[*]Voice({e.Update.Message.Caption}|{id})\n");
                                File.AppendAllText($"./history/{next}/{from.Id}", $"[/]Voice({e.Update.Message.Caption}|{id})\n");
                                break;
                            case MessageType.Document:
                                id = e.Update.Message.Document.FileId;
                                cap = e.Update.Message.Caption;
                                iof = new InputOnlineFile(id);

                                await Bot.SendDocumentAsync(next, iof, caption: cap);
                                File.AppendAllText($"./history/{from.Id}/{next}", $"[*]Document({e.Update.Message.Caption}|{id})\n");
                                File.AppendAllText($"./history/{next}/{from.Id}", $"[/]Document({e.Update.Message.Caption}|{id})\n");
                                break;
                            case MessageType.Sticker:
                                id = e.Update.Message.Sticker.FileId;
                                iof = new InputOnlineFile(id);

                                await Bot.SendStickerAsync(next, iof);
                                File.AppendAllText($"./history/{from.Id}/{next}", $"[*]Sticker({e.Update.Message.Caption}|{id})\n");
                                File.AppendAllText($"./history/{next}/{from.Id}", $"[/]Sticker({e.Update.Message.Caption}|{id})\n");
                                break;
                            case MessageType.Location:
                                var loc = e.Update.Message.Location;

                                await Bot.SendLocationAsync(next, loc.Latitude, loc.Longitude);
                                File.AppendAllText($"./history/{from.Id}/{next}", $"[*]Location({loc.Latitude}|{loc.Longitude})\n");
                                File.AppendAllText($"./history/{next}/{from.Id}", $"[/]Location({loc.Latitude}|{loc.Longitude})\n");
                                break;
                            case MessageType.Contact:
                                var con = e.Update.Message.Contact;

                                await Bot.SendContactAsync(next, con.PhoneNumber, con.FirstName);
                                File.AppendAllText($"./history/{from.Id}/{next}", $"[*]Location({con.PhoneNumber}|{con.FirstName})\n");
                                File.AppendAllText($"./history/{next}/{from.Id}", $"[/]Location({con.PhoneNumber}|{con.FirstName})\n");
                                break;
                            case MessageType.Venue:
                                var ven = e.Update.Message.Venue;

                                await Bot.SendVenueAsync(next, ven.Location.Latitude,
                                    ven.Location.Longitude, ven.Title,
                                    ven.Address);
                                break;
                            case MessageType.VideoNote:
                                id = e.Update.Message.VideoNote.FileId;
                                cap = e.Update.Message.Caption;
                                iof = new InputOnlineFile(id);

                                await Bot.SendVideoNoteAsync(next, iof);
                                File.AppendAllText($"./history/{from.Id}/{next}", $"[*]Location({e.Update.Message.Caption}|{id})\n");
                                File.AppendAllText($"./history/{next}/{from.Id}", $"[/]Location({e.Update.Message.Caption}|{id})\n");
                                break;
                            default:
                                await Bot.SendTextMessageAsync(from.Id, Configurate(iCantSendThis));
                                break;
                        }

                    }
                    else
                    {
                        await Bot.SendTextMessageAsync(from.Id, Configurate(noInterlocutor));
                    }

                }
                
            }

        }

        static void Find(int id)
        {
            var next = File.ReadAllText("next");
            if (next == "" || next == null || next == id.ToString())
            {
                File.WriteAllText("next", id.ToString());
            }
            else
            {
                File.WriteAllText("next", string.Empty);

                Bot.SendTextMessageAsync(id, Configurate(interlocutorFound));
                Bot.SendTextMessageAsync(int.Parse(next), Configurate(interlocutorFound));

                File.WriteAllText($"./Chats/{id}", next);
                File.WriteAllText($"./Chats/{next}", id.ToString());
                File.AppendAllText($"./history/{id}/{next}", $"-----------BEGIN({next})-----------\n");
                File.AppendAllText($"./history/{next}/{id}", $"-----------BEGIN({id})-----------\n");
            }

        }
        /*
          %FirstName% - User FirstName
          %FirstNameWL% - User FirstName with link
          %LastName% - User LastName
          %LastNameWL% - User LastName with link
          %FullNameWL% - User FullName with link
          %ID% - User ID
          %IDWL% - User ID with link
          %InterlocutorFN% - Interlocutor FirstName
          %InterlocutorFNWL% - Interlocutor FirstName with link
          %InterlocutorLN% - Interlocutor LastName
          %InterlocutorLNWL% - Interlocutor LastName with link
          %InterlocutorFullNameWL% - Interlocutor FullName with link
          %InterlocutorID% - Interlocutor ID
          %InterlocutorIDWL% - Interlocutor ID with link
          ------------------------------------------------
          next - Next interlocutor or search new
          search - search = start
          stop - Stop chat
          cancel - Cancel serching interlocutor
        */
        static string cfg = File.ReadAllText("config.cfg");
        static JObject json = JObject.Parse(cfg.Substring(cfg.IndexOf('{')));
        static JObject phrases = (JObject)json["phrases"];
        static JObject commands = (JObject)json["commands"];
        static JArray lang = (JArray)json["lang"];

        static string
            api = json["bot"].ToString(),
            lang1 = lang[0].ToString(),
            next = commands["next"].ToString(),
            search = commands["search"].ToString(),
            stop = commands["stop"].ToString(),
            cancel = commands["cancel"].ToString(),
            hello = phrases["hello"].ToString(),
            searching = phrases["searching"].ToString(),
            chatEnd = phrases["chatEnd"].ToString(),
            chatEnded = phrases["chatEnded"].ToString(),
            interlocutorFound = phrases["interlocutorFound"].ToString(),
            noInterlocutor = phrases["noInterlocutor"].ToString(),
            iCantSendThis = phrases["iCantSendThis"].ToString(),
            canceled = phrases["canceled"].ToString(),
            help = phrases["help"].ToString(),
            rules = phrases["rules"].ToString();
        static string Configurate(string cfg, User user=null, User interlocutor=null)
        {
            string retVal = "";
            if (!cfg.Contains('%'))
            {
                return cfg;
            }
            else
            {
                if (interlocutor == null)
                {
                    retVal = cfg
                        .Replace("%FirstName%", user.FirstName)
                        .Replace("%LastName%", user.LastName)
                        .Replace("%ID%", user.Id.ToString())
                        .Replace("%FirstNameWL%", $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a>")
                        .Replace("%LastNameWL%", $"<a href=\"tg://user?id={user.Id}\">{user.LastName}</a>")
                        .Replace("%FullNameWL%", $"<a href=\"tg://user?id={user.Id}\">{user.FirstName + " " + user.LastName}</a>")
                        .Replace("%IDWL%", $"<a href=\"tg://user?id={user.Id}\">{user.Id}</a>");
                }
                else
                {
                    retVal = cfg
                        .Replace("%FirstName%", user.FirstName)
                        .Replace("%InterlocutorFN%", interlocutor.FirstName)
                        .Replace("%LastName%", user.LastName)
                        .Replace("%InterlocutorLN%", interlocutor.LastName)
                        .Replace("%ID%", user.Id.ToString())
                        .Replace("%InterlocutorID%", interlocutor.Id.ToString())
                        .Replace("%FirstNameWL%", $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a>")
                        .Replace("%InterlocutorFNWL%", $"<a href=\"tg://user?id={interlocutor.Id}\">{interlocutor.FirstName}</a>")
                        .Replace("%LastNameWL%", $"<a href=\"tg://user?id={user.Id}\">{user.LastName}</a>")
                        .Replace("%InterlocutorLNWL%", $"<a href=\"tg://user?id={interlocutor.Id}\">{interlocutor.LastName}</a>")
                        .Replace("%FullNameWL%", $"<a href=\"tg://user?id={user.Id}\">{user.FirstName + " " + user.LastName}</a>")
                        .Replace("%InterlocutorFullNameWL%", $"<a href=\"tg://user?id={interlocutor.Id}\">{interlocutor.FirstName + " " + interlocutor.LastName}</a>")
                        .Replace("%IDWL%", $"<a href=\"tg://user?id={user.Id}\">{user.Id}</a>")
                        .Replace("%InterlocutorIDWL%", $"<a href=\"tg://user?id={interlocutor.Id}\">{interlocutor.Id}</a>");
                }
            }
            return retVal;
        }
    }
}
