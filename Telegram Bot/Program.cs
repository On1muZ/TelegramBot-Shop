using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using static System.Net.Mime.MediaTypeNames;
using Telegram.Bot.Types.Enums;

using System.Configuration;
using System.Collections.Specialized;



namespace Telegram_Bot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            string TOKEN = Bot.TOKEN;
            

            long admin_id = Bot.ADMIN_ID;

            var client = new TelegramBotClient(TOKEN);

            TimeSpan time = new TimeSpan(24, 59, 59);

            client.Timeout = time;

            await Console.Out.WriteLineAsync("Запуск бота . . .");
            client.StartReceiving(Update, Error);

            UserDataBase.CreateTable();
            MailingStateDataBase.CreateTable();
            OrderStateDataBase.CreateTable();
            OrderDataBase.CreateTable();

            await Console.Out.WriteLineAsync("Бот запущен!\nНажмите любую клавишу для завершения работы бота . . .");
            await client.SendTextMessageAsync(chatId: admin_id, text: "Бот запущен");
            Console.ReadKey();
        }

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }

        private static async Task Update(ITelegramBotClient botClient, Update update, CancellationToken TOKEN)
        {
            var message = update.Message;
            var callbackData = update.CallbackQuery;

            try
            {
                if (callbackData != null)
                {
                    if (callbackData.Data == "cancel_state")
                    {

                        if (UserDataBase.GetState(callbackData.Message.Chat.Id) == "Mailing_get_text" || UserDataBase.GetState(callbackData.Message.Chat.Id) == "Mailing_get_photo")
                        {
                            MailingStateDataBase.DeleteState(callbackData.Message.Chat.Id);
                        }
                        else if (UserDataBase.GetState(callbackData.Message.Chat.Id) == "order_get_text" || UserDataBase.GetState(callbackData.Message.Chat.Id) == "order_get_photo")
                        {
                            OrderStateDataBase.DeleteState(callbackData.Message.Chat.Id);
                        }
                        UserDataBase.SetState(callbackData.Message.Chat.Id, "no");
                        await botClient.SendTextMessageAsync(chatId: callbackData.Message.Chat.Id, text: "Отменено");
                    }
                    else if (callbackData.Data == "NextOrder")
                    {
                        try
                        {


                            List<string[]> orders = OrderDataBase.GetOrders();
                            long order_position = UserDataBase.GetOrderPosition(callbackData.Message.Chat.Id) + 1;

                            InlineKeyboardButton[] row1 = new InlineKeyboardButton[]
                        {
                        InlineKeyboardButton.WithCallbackData(text: $"{order_position + 1}", callbackData: "Null"),
                        InlineKeyboardButton.WithCallbackData(text: "/", callbackData: "Null"),
                        InlineKeyboardButton.WithCallbackData(text: $"{orders.Count}", callbackData: "Null"),
                        };

                            InlineKeyboardButton[] row2 = new InlineKeyboardButton[]
                            {
                        InlineKeyboardButton.WithCallbackData(text: "<", callbackData: "PastOrder"),
                        InlineKeyboardButton.WithCallbackData(text: ">", callbackData: "NextOrder"),
                            };

                            InlineKeyboardButton[][] buttons = new InlineKeyboardButton[3][];

                            if (UserDataBase.IsAdmin(callbackData.Message.Chat.Id))
                            {
                                InlineKeyboardButton[] row3 = new InlineKeyboardButton[]
                                {
                            InlineKeyboardButton.WithCallbackData(text: "Удалить", callbackData: "delete_order")
                                };
                                buttons = new InlineKeyboardButton[][] { row1, row2, row3 };
                            }
                            else
                            {
                                buttons = new InlineKeyboardButton[][] { row1, row2 };
                            }
                            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(buttons);



                            string text = orders[Convert.ToInt32(order_position)][0];
                            string photo = orders[Convert.ToInt32(order_position)][1];
                            long messageId = UserDataBase.GetMainMessage(callbackData.Message.Chat.Id);
                            await botClient.DeleteMessageAsync(chatId: callbackData.Message.Chat.Id, messageId: Convert.ToInt32(messageId));

                            Message botMessage = await botClient.SendPhotoAsync(chatId: callbackData.Message.Chat.Id, caption: text, photo: photo, parseMode: ParseMode.Html, replyMarkup: keyboard);
                            UserDataBase.SetOrderPosition(callbackData.Message.Chat.Id, order_position);
                            UserDataBase.SetMainMessage(callbackData.Message.Chat.Id, message_id: botMessage.MessageId);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            Console.Write("");
                        }
                    }
                    else if (callbackData.Data == "PastOrder")
                    {
                        try
                        {
                            List<string[]> orders = OrderDataBase.GetOrders();
                            long order_position = UserDataBase.GetOrderPosition(callbackData.Message.Chat.Id) - 1;

                            InlineKeyboardButton[] row1 = new InlineKeyboardButton[]
                        {
                        InlineKeyboardButton.WithCallbackData(text: $"{order_position + 1}", callbackData: "Null"),
                        InlineKeyboardButton.WithCallbackData(text: "/", callbackData: "Null"),
                        InlineKeyboardButton.WithCallbackData(text: $"{orders.Count}", callbackData: "Null"),
                        };

                            InlineKeyboardButton[] row2 = new InlineKeyboardButton[]
                            {
                        InlineKeyboardButton.WithCallbackData(text: "<", callbackData: "PastOrder"),
                        InlineKeyboardButton.WithCallbackData(text: ">", callbackData: "NextOrder"),
                            };

                            InlineKeyboardButton[][] buttons = new InlineKeyboardButton[3][];

                            if (UserDataBase.IsAdmin(callbackData.Message.Chat.Id))
                            {
                                InlineKeyboardButton[] row3 = new InlineKeyboardButton[]
                                {
                            InlineKeyboardButton.WithCallbackData(text: "Удалить", callbackData: "delete_order")
                                };
                                buttons = new InlineKeyboardButton[][] { row1, row2, row3 };
                            }
                            else
                            {
                                buttons = new InlineKeyboardButton[][] { row1, row2 };
                            }
                            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(buttons);



                            string text = orders[Convert.ToInt32(order_position)][0];
                            string photo = orders[Convert.ToInt32(order_position)][1];
                            long messageId = UserDataBase.GetMainMessage(callbackData.Message.Chat.Id);
                            await botClient.DeleteMessageAsync(chatId: callbackData.Message.Chat.Id, messageId: Convert.ToInt32(messageId));

                            Message botMessage = await botClient.SendPhotoAsync(chatId: callbackData.Message.Chat.Id, caption: text, photo: photo, parseMode: ParseMode.Html, replyMarkup: keyboard);
                            UserDataBase.SetOrderPosition(callbackData.Message.Chat.Id, order_position);
                            UserDataBase.SetMainMessage(callbackData.Message.Chat.Id, message_id: botMessage.MessageId);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            Console.Write("");
                        }
                    }
                    else if (callbackData.Data == "delete_order")
                    {
                        if (UserDataBase.IsAdmin(callbackData.Message.Chat.Id))
                        {
                            List<string[]> orders = OrderDataBase.GetOrders();
                            long order_id = UserDataBase.GetOrderPosition(callbackData.Message.Chat.Id);
                            long message_id = UserDataBase.GetMainMessage(chat_id: callbackData.Message.Chat.Id);

                            OrderDataBase.DeleteOrder(orders[Convert.ToInt32(order_id)][0]);
                            await botClient.DeleteMessageAsync(chatId: callbackData.Message.Chat.Id, messageId: Convert.ToInt32(message_id));
                            await botClient.SendTextMessageAsync(chatId: callbackData.Message.Chat.Id, text: "Удалено успешно");

                            UserDataBase.SetOrderPosition(callbackData.Message.Chat.Id, 0);
                        }
                    }
                }

                if (message != null) { 

                if (callbackData is null && UserDataBase.GetState(message.Chat.Id) != "no")// Стейты ...
                {
                    if (UserDataBase.GetState(message.Chat.Id) == "Mailing_get_text")
                    {
                        MailingStateDataBase.StateText(message.Chat.Id, message.Text);
                        UserDataBase.SetState(message.Chat.Id, "Mailing_get_photo");
                        InlineKeyboardMarkup InlineKeyboard = new InlineKeyboardMarkup(new[]
                        {
                    InlineKeyboardButton.WithCallbackData(text: "Отмена", callbackData: "cancel_state")
                });
                        await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Пришлите фото рассылки", replyMarkup: InlineKeyboard);

                    }
                    else if (UserDataBase.GetState(message.Chat.Id) == "Mailing_get_photo")
                    {
                        try
                        {
                            string photo = message.Photo.Last().FileId;
                            MailingStateDataBase.StatePhoto(message.Chat.Id, photo);
                            string[] state_data = MailingStateDataBase.GetStateData(message.Chat.Id);

                            List<long> users = UserDataBase.GetUsers();

                            foreach (long chat_id in users)
                            {
                                await botClient.SendPhotoAsync(chatId: chat_id, photo: state_data[1], caption: state_data[0], parseMode: ParseMode.Html);
                            }
                            await Console.Out.WriteLineAsync($"Произошла рассылка от {message.From.FirstName}");

                        }
                        catch (Exception)
                        {
                            await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Ошибка с данными . . .");
                        }
                        finally
                        {
                            UserDataBase.SetState(message.Chat.Id, "no");
                            MailingStateDataBase.DeleteState(message.Chat.Id);
                        }
                    }

                    else if (UserDataBase.GetState(message.Chat.Id) == "order_get_text")
                    {
                        InlineKeyboardMarkup InlineKeyboard = new InlineKeyboardMarkup(new[]
                        {
                        InlineKeyboardButton.WithCallbackData(text: "Отмена", callbackData: "cancel_state")
                    });
                        OrderStateDataBase.StateText(message.Chat.Id, message.Text);
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Пришлите фото товара", replyMarkup: InlineKeyboard);
                        UserDataBase.SetState(message.Chat.Id, "order_get_photo");
                    }
                    else if (UserDataBase.GetState(message.Chat.Id) == "order_get_photo")
                    {
                        try
                        {
                            string photo = message.Photo.Last().FileId;
                            OrderStateDataBase.StatePhoto(message.Chat.Id, photo);
                            string[] state_data = OrderStateDataBase.GetStateData(message.Chat.Id);


                            List<long> users = UserDataBase.GetUsers();

                            foreach (long chat_id in users)
                            {
                                string caption = $"В магазине появился новый товар:\n{state_data[0]}";
                                await botClient.SendPhotoAsync(chatId: chat_id, photo: state_data[1], caption: caption, parseMode: ParseMode.Html);
                            }
                            OrderDataBase.AddOrder(state_data[0], state_data[1]);
                            await Console.Out.WriteLineAsync($"Появился новый товар в магазине от {message.From.FirstName}");
                        }
                        catch (Exception ex)
                        {
                            await Console.Out.WriteLineAsync(ex.ToString());
                            await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Ошибка с данными . . . " + ex);
                        }
                        finally
                        {
                            UserDataBase.SetState(message.Chat.Id, "no");
                            OrderStateDataBase.DeleteState(message.Chat.Id);
                        }
                    }
                }

                    if (callbackData is null && UserDataBase.GetState(message.Chat.Id) == "no")
                    {
                        await Console.Out.WriteLineAsync($"Сообщение: {message.Text}, от {message.From.FirstName} {message.Chat.Id}");
                        if (message.Text == "/start" || message.Text == "/help")
                        {

                            var replyKeyboard = new ReplyKeyboardMarkup(new[]
                                {
                        new KeyboardButton[]{"Магазин"},
                        new KeyboardButton[]{"Настройки"}
                    })
                            {
                                ResizeKeyboard = true
                            };

                            if (UserDataBase.IsAdmin(message.Chat.Id))
                            {
                                replyKeyboard = new ReplyKeyboardMarkup(new[]
                            {
                        new KeyboardButton[]{"Магазин"},
                        new KeyboardButton[]{"Настройки"},
                        new KeyboardButton[]{ "Панель Админа" }
                    })
                                {
                                    ResizeKeyboard = true
                                };
                            }



                            if (UserDataBase.IsUser(message.Chat.Id))
                            {
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Здравствуйте {message.From.FirstName}! Наш бот-магазин приветствует Вас! В нашем магазине вы можете купить различные вещи.", replyMarkup: replyKeyboard);
                            }
                            else
                            {
                                UserDataBase.AddUser(message.From.FirstName, message.Chat.Id, message.MessageId);
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Здравствуйте {message.From.FirstName}, наш бот-магазин приветствует Вас! В нашем магазине вы можете купить различные вещи.", replyMarkup: replyKeyboard);
                            }

                        }
                        else if (message.Text == "Панель Админа")
                        {
                            if (UserDataBase.IsAdmin(message.Chat.Id))
                            {
                                var replyKeyboard = new ReplyKeyboardMarkup(new[]
                                {
                        new KeyboardButton[]{"Создать рассылку"},
                        new KeyboardButton[]{"Добавить товар в магаизн"},
                        new KeyboardButton[]{ "Форматирование текста" },
                        new KeyboardButton[]{ "Панель пользователя" }
                    })
                                {
                                    ResizeKeyboard = true
                                };
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Режим админа активирован", replyMarkup: replyKeyboard);
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Вы не админ");
                            }
                        }
                        else if (message.Text == "Панель пользователя")
                        {
                            if (UserDataBase.IsAdmin(message.Chat.Id))
                            {
                                var replyKeyboard = new ReplyKeyboardMarkup(new[]
                            {
                        new KeyboardButton[]{"Магазин"},
                        new KeyboardButton[]{"Настройки"},
                        new KeyboardButton[]{ "Панель Админа" }
                    })
                                {
                                    ResizeKeyboard = true
                                };
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Режим пользователя активирован", replyMarkup: replyKeyboard);
                            }
                        }
                        else if (message.Text == "Создать рассылку")
                        {
                            if (UserDataBase.IsAdmin(message.Chat.Id))
                            {
                                InlineKeyboardMarkup InlineKeyboard = new InlineKeyboardMarkup(new[]
                                    {
                                InlineKeyboardButton.WithCallbackData(text: "Отмена", callbackData: "cancel_state")
                            });
                                UserDataBase.SetState(message.Chat.Id, "Mailing_get_text");
                                MailingStateDataBase.NewStateChatID(message.Chat.Id);
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Введите текст рассылки", replyMarkup: InlineKeyboard);
                            }
                        }
                        else if (message.Text == "Добавить товар в магаизн")
                        {
                            if (UserDataBase.IsAdmin(message.Chat.Id))
                            {
                                InlineKeyboardMarkup InlineKeyboard = new InlineKeyboardMarkup(new[]
                            {
                            InlineKeyboardButton.WithCallbackData(text: "Отмена", callbackData: "cancel_state")
                        });
                                UserDataBase.SetState(message.Chat.Id, "order_get_text");
                                OrderStateDataBase.NewStateChatID(message.Chat.Id);
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Введите текстовое описание товара вкючая цену на него, ссылку на человека, которому нужно писать и т. д.", replyMarkup: InlineKeyboard);

                            }
                        }
                        else if (message.Text == "Магазин")
                        {
                            try
                            {
                                List<string[]> orders = OrderDataBase.GetOrders();
                                long order_position = 0;

                                InlineKeyboardButton[] row1 = new InlineKeyboardButton[]
                                {
                        InlineKeyboardButton.WithCallbackData(text: $"{order_position + 1}", callbackData: "Null"),
                        InlineKeyboardButton.WithCallbackData(text: "/", callbackData: "Null"),
                        InlineKeyboardButton.WithCallbackData(text: $"{orders.Count}", callbackData: "Null"),
                                };

                                InlineKeyboardButton[] row2 = new InlineKeyboardButton[]
                                {
                        InlineKeyboardButton.WithCallbackData(text: "<", callbackData: "PastOrder"),
                        InlineKeyboardButton.WithCallbackData(text: ">", callbackData: "NextOrder"),
                                };

                                InlineKeyboardButton[][] buttons = new InlineKeyboardButton[3][];

                                if (UserDataBase.IsAdmin(message.Chat.Id))
                                {
                                    InlineKeyboardButton[] row3 = new InlineKeyboardButton[]
                                    {
                            InlineKeyboardButton.WithCallbackData(text: "Удалить", callbackData: "delete_order")
                                    };
                                    buttons = new InlineKeyboardButton[][] { row1, row2, row3 };
                                }
                                else
                                {
                                    buttons = new InlineKeyboardButton[][] { row1, row2 };
                                }
                                InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(buttons);

                                string text = orders[Convert.ToInt32(order_position)][0];
                                string photo = orders[Convert.ToInt32(order_position)][1];
                                var botMessage = await botClient.SendPhotoAsync(chatId: message.Chat.Id, caption: text, photo: photo, parseMode: ParseMode.Html, replyMarkup: keyboard);
                                UserDataBase.SetOrderPosition(message.Chat.Id, order_position);
                                UserDataBase.SetMainMessage(message.Chat.Id, botMessage.MessageId);
                            }
                            catch (Exception)
                            {
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Магазин пуст");
                            }
                        }
                        else if (message.Text == "Форматирование текста")
                        {
                            if (UserDataBase.IsAdmin(message.Chat.Id))
                            {
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "При форматировании текста вы можете использовать HTML теги: http://htmlbook.ru/content/formatirovanie-teksta.\n");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex}");
            }
        }

    }
}
