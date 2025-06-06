﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Interfaces;
using System.Globalization;
using System.Net.Sockets;


namespace YavaPrimum.Core.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly YavaPrimumDBContext _dbContext;
        private readonly IConverterService _converterService;
        private readonly IUserService _userService;

        public NotificationsService(YavaPrimumDBContext dbContext, IConverterService converterService, IUserService userService)
        {
            _dbContext = dbContext;
            _converterService = converterService;
            _userService = userService;
        }

        public async Task<Notifications> GetById(Guid id)
        {

            Notifications n = await _dbContext.Notifications
                 //.Include(t => t)
                 .Include(t => t.Task.Status)
                .Include(c => c.Task.Candidate)
                .Include(c => c.Task.Candidate.Country)
                .Include(c => c.Task.Candidate.Post)
                .Include(c => c.Task.Candidate)
                .Include(u => u.Task.User)
                .Include(u => u.Task.User.Company)
                .Include(u => u.Task.User.Company.Country)
                .Include(u => u.Task.User.Post)
                .Include(u => u.Task.Status)
                .Include(u => u.Task)
                .Include(r => r.Recipient)
                .Include(r => r.Recipient.Post)
                .Include(r => r.Recipient.Company)
                .Include(r => r.Recipient.Company.Country)
                .Where(r => r.NotificationsId == id)
                .FirstOrDefaultAsync();

            return n;
        }

        public async Task<List<NotificationsResponse>> GetAllByUserId(Guid id)
        {
            List<Notifications> notifications = await _dbContext.Notifications
                //.Include(t => t)
                .Include(t => t.Task.Status)
                .Include(c => c.Task.Candidate)
                .Include(c => c.Task.Candidate.Country)
                .Include(c => c.Task.Candidate.Post)
                .Include(c => c.Task.Candidate)
                .Include(u => u.Task.User)
                .Include(u => u.Task.User.Company)
                .Include(u => u.Task.User.Company.Country)
                .Include(u => u.Task.User.Post)
                .Include(u => u.Task.Status)
                .Include(u => u.Task)
                .Include(r => r.Recipient)
                .Include(r => r.Recipient.Post)
                .Include(r => r.Recipient.Company)
                .Include(r => r.Recipient.Company.Country)
                .Where(r => r.Recipient.UserId == id)
                .OrderByDescending(n => n.DateTimeOfNotify) // Сортировка по дате создания (от новой к старой)
                .ToListAsync();

            List<NotificationsResponse> notificationsResponse = new List<NotificationsResponse>();

            foreach (var notification in notifications)
            {
                notificationsResponse.Add(new NotificationsResponse
                {
                    NotificationsId = notification.NotificationsId,
                    Task = await _converterService.ConvertToFront(task: notification.Task),
                    DateTime = notification.DateTimeOfNotify,
                    IsReaded = notification.IsReaded,
                    TextMessage = notification.TextMessage,
                    Recipient = await _converterService.ConvertToFront(notification.Recipient),
                    Status = notification.Task.Status.Name,
                });
            }

            /*if (notificationsResponse.Count > 0)
            {
                Console.WriteLine("Уведомлений у " + notificationsResponse[0].Recipient.Surname + " : " + notificationsResponse.Count);
            }
            else
            {
                Console.WriteLine("Уведомлений нет.");
            }*/

            return notificationsResponse;
        }


        public async Task ReadNotification(Guid id)
        {
            Notifications notification = await GetById(id);
            Console.WriteLine(notification.ToString);
            notification.IsReaded = true;

            _dbContext.Update(notification);
            _dbContext.SaveChanges();

        }

        public async Task ReadNotificationOfCandidate(Guid candidateId)
        {
            var notifications = await _dbContext.Notifications
                .Where(x => x.Task.Candidate.CandidateId == candidateId &&
                            x.Task.Status.Name == "Собеседование пройдено" || x.Task.Status.Name == "Выполнено тестовое задание")
                .ToListAsync(); // Directly retrieve the notification objects

            foreach (var notification in notifications)
            {
                notification.IsReaded = true;
            }

            _dbContext.Notifications.UpdateRange(notifications);
            await _dbContext.SaveChangesAsync();

        }

        public async Task ReadAllNotificationOfCandidate(Guid candidateId)
        {
            var notifications = await _dbContext.Notifications
                .Where(x => x.Task.Candidate.CandidateId == candidateId)
                .ToListAsync(); // Directly retrieve the notification objects

            foreach (var notification in notifications)
            {
                notification.IsReaded = true;
            }

            _dbContext.Notifications.UpdateRange(notifications);
            await _dbContext.SaveChangesAsync();

        }

        public async Task ReadNotificationOfTask(Guid taskId)
        {
            var notifications = await _dbContext.Notifications
                .Where(x => x.Task.TasksId == taskId)
                .ToListAsync(); // Directly retrieve the notification objects

            foreach (var notification in notifications)
            {
                notification.IsReaded = true;
            }

            _dbContext.Notifications.UpdateRange(notifications);
            await _dbContext.SaveChangesAsync();

        }

        public async Task SendCountryRecruiterNotifications(Tasks task)
        {
            List<Notifications> notifications = new List<Notifications>();

            var recruters = _dbContext.User.Where(u =>
                                u.Company.Country == task.Candidate.Country &&
                                u.Post.Name == "Кадровик");

            foreach (var recruter in recruters)
            {
                notifications.Add(new Notifications()
                {
                    NotificationsId = new Guid(),
                    Task = task,
                    DateTimeOfNotify = DateTime.Now,
                    Recipient = recruter,
                    IsReaded = false,
                    TextMessage = GetMessageText(task)
                });
            }
            _dbContext.Notifications.AddRange(notifications);
            await _dbContext.SaveChangesAsync();

        }

        public async Task SendMessage(Tasks task)
        {

            User? recipient = _dbContext.Tasks
                .Include(u => u.User)
                .Include(u => u.User.Post)
                .Where(u => u.Candidate == task.Candidate && u.User.Post != task.User.Post)
                .FirstOrDefault()?.User;
            if (recipient == null) Console.WriteLine("Получатель не найден");

            
            Notifications notification = new Notifications()
            {
                NotificationsId = new Guid(),
                Task = task,
                DateTimeOfNotify = DateTime.Now,
                Recipient = recipient,
                IsReaded = false,
                TextMessage = GetMessageText(task)
            };

            await _dbContext.Notifications.AddAsync(notification);
            await _dbContext.SaveChangesAsync();
        }


        public async Task SendMessageToChangeDataOrTime(Tasks task, bool isDate, bool isHr = true)
        {

            User? recipient = _dbContext.Tasks
                .Include(u => u.User)
                .Include(u => u.User.Post)
                .Where(u => u.Candidate == task.Candidate && u.User.Post != task.User.Post)
                .FirstOrDefault()?.User;
            if (recipient == null) Console.WriteLine("Получатель не найден");


            Notifications notification = new Notifications()
            {
                NotificationsId = new Guid(),
                Task = task,
                DateTimeOfNotify = DateTime.Now,
                Recipient = recipient,
                IsReaded = false,
                TextMessage = isHr? isDate ? GetTextMessageForDateChange(task) : GetTextMessageForTimeChange(task) 
                                    : isDate ? GetTextMessageForDateChangeKadr(task) : GetTextMessageForTimeChangeKadr(task)
            };

            await _dbContext.Notifications.AddAsync(notification);
            await _dbContext.SaveChangesAsync();
        }

        public string GetMessageText(Tasks task)
        {
            var candidate = task.Candidate;
            var user = task.User;

            if (candidate == null || user == null || task.Status.MessageTemplate == null)
                throw new InvalidOperationException("У задачи отсутствует информация о кандидате или пользователе.");


            // Замена плейсхолдеров на реальные значения
            var textMessage = task.Status.MessageTemplate
                .Replace("[Candidate.Surname]", candidate.Surname)
                .Replace("[Candidate.FirstName]", candidate.FirstName)
                .Replace("[Candidate.Post]", task.Candidate.Post.Name)
                .Replace("[User.Surname]", user.Surname)
                .Replace("[User.FirstName]", user.FirstName)
                .Replace("[Date]", task.DateTime.ToShortDateString())
                .Replace("[Time]", task.DateTime.ToString("HH:mm"));

            return textMessage;
        }



        public async Task<string> SendMessageToEmail(string email, string myMessage, string subject)
        {

            try
            {
                MailAddress mailFrom = new MailAddress("yavaprimum@mail.ru", "YavaPrimum");
                MailAddress mailTo = new MailAddress(email);
                MailMessage message = new MailMessage(mailFrom, mailTo);
                message.Body = myMessage;
                message.Subject = subject;

                SmtpClient smtpClient = new SmtpClient()
                {
                    Host = "smtp.mail.ru",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(mailFrom.Address, "vuy7pBNKJC1LEESaksGN")
                };

                await smtpClient.SendMailAsync(message);
                Console.WriteLine($"Сообщение отправлено на {email}");
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Ошибка формата email: " + ex.Message);
                return "105";
            }
            catch (Exception e)
            {
                var error = new ArgumentException("Указанный email адрес не существует или имеет неверный формат");
                Console.WriteLine(error.Message);
                throw error;
            }

            return email;
        }


    public string GetTextMessageForTestTask(Tasks task)
        {
            return $@"
                Уважаемый(ая) {task.Candidate.FirstName ?? "Кандидат"}!

                Благодарим вас за участие в собеседовании на позицию {task.Candidate.Post.Name ?? "позиция не указана"}.
                Мы были впечатлены вашими навыками и опытом и теперь хотели бы предложить вам выполнить тестовое задание.

                ### Детали задания:
                Срок выполнения: до {task.DateTime.ToString("dd MMMM yyyy", CultureInfo.GetCultureInfo("ru-RU"))}
                Задача: {task.AdditionalData ?? "Описание отсутствует"}

                Вы можете задать любые уточняющие вопросы — мы с радостью на них ответим.


                Желаем удачи! Мы с нетерпением ждем вашей работы.

                С уважением,  
                {task.User.FirstName ?? "HR-специалист"}  
                HR PRIMUM 
                {task.User.Email ?? "Email отсутствует"}  
                {"https://primum1.com/"}
";

        }

        private string GetTextMessageForDateChange(Tasks task)
        {
            return $"Кандидат {task.Candidate.Surname} {task.Candidate.FirstName} на должности {task.Candidate.Post.Name} нуждается в смене даты. Дата {task.DateTime.ToString("dd.MM.yyyy")} ему не подходит";
        }

        private string GetTextMessageForTimeChange(Tasks task)
        {
            return $"Кандидат {task.Candidate.Surname} {task.Candidate.FirstName} на должности {task.Candidate.Post.Name} нуждается в смене времени. Время  {task.DateTime.ToString("dd.MM.yyyy")} {task.DateTime.ToString("HH:mm")} ему не подходит";
        }

        private string GetTextMessageForTimeChangeKadr(Tasks task)
        {
            return $"Кандидат {task.Candidate.Surname} {task.Candidate.FirstName} {task.Candidate.Post.Name} должен изменить время. Сотруднику подходит время {task.DateTime.ToString("dd.MM.yyyy")} {task.DateTime.ToString("HH:mm")} {task.DateTime.ToString("HH:mm")}.";
        }

        private string GetTextMessageForDateChangeKadr(Tasks task)
        {
            return $"Кандидат {task.Candidate.Surname} {task.Candidate.FirstName} {task.Candidate.Post.Name} должен изменить дату. Сотруднику подходит {task.DateTime.ToString("dd.MM.yyyy")}.";
        }


    }
}
