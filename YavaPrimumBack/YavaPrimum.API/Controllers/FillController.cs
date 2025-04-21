using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.Interfaces;
using YavaPrimum.Core.Services;

namespace YavaPrimum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FillController : ControllerBase
    {
        private readonly YavaPrimumDBContext _dBContext;
        private INotificationsService _notificationsService;
        private IConverterService _converterService;

        public FillController(
            YavaPrimumDBContext dBContext,
            INotificationsService notificationsService,
            IConverterService converterService)
        {

            _notificationsService = notificationsService;
            _converterService = converterService; 
            _dBContext = dBContext;
        }
  

        [HttpPost("/crtBase")]
        public async Task<ActionResult> crtBase()
        {
            var belarus = new Country
            {
                CountryId = Guid.NewGuid(),
                Name = "Беларусь",
                PhoneMask = "+375 (XX) XXX-XX-XX"
            };

            var kazakhstan = new Country
            {
                CountryId = Guid.NewGuid(),
                Name = "Казахстан",
                PhoneMask = "+7 (XXX) XXX-XX-XX"
            };

            var russia = new Country
            {
                CountryId = Guid.NewGuid(),
                Name = "Россия",
                PhoneMask = "+7 (XXX) XXX-XX-XX"
            };

            var uae = new Country
            {
                CountryId = Guid.NewGuid(),
                Name = "ОАЭ",
                PhoneMask = "+971 (X) XXX-XXXX"
            };

            var china = new Country
            {
                CountryId = Guid.NewGuid(),
                Name = "Китай",
                PhoneMask = "+86 (XXX) XXXX-XXXX"
            };

            // 2. Создаем компании
            var companies = new List<Company>
    {
        // Беларусь
        new Company { CompanyId = Guid.NewGuid(), Name = "Примвей", Country = belarus },
        new Company { CompanyId = Guid.NewGuid(), Name = "БелАгроПро", Country = belarus },
        
        // Казахстан
        new Company { CompanyId = Guid.NewGuid(), Name = "КазМинТех", Country = kazakhstan },
        new Company { CompanyId = Guid.NewGuid(), Name = "КазХайТек", Country = kazakhstan },
        
        // Россия
        new Company { CompanyId = Guid.NewGuid(), Name = "АвтоТранс", Country = russia },
        new Company { CompanyId = Guid.NewGuid(), Name = "Первый элемент", Country = russia },
        
        // ОАЭ
        new Company { CompanyId = Guid.NewGuid(), Name = "EmirTech", Country = uae },
        new Company { CompanyId = Guid.NewGuid(), Name = "Desert Solutions", Country = uae },
        
        // Китай
        new Company { CompanyId = Guid.NewGuid(), Name = "China Innovations", Country = china },
        new Company { CompanyId = Guid.NewGuid(), Name = "Beijing TechWorks", Country = china }
    };

            // 3. Создаем должности
            var posts = new List<Post>
    {
        new Post { PostId = Guid.NewGuid(), Name = "Водитель" },
        new Post { PostId = Guid.NewGuid(), Name = "Программист 1С" },
        new Post { PostId = Guid.NewGuid(), Name = "!Админ" },
        new Post { PostId = Guid.NewGuid(), Name = "HR" },
        new Post { PostId = Guid.NewGuid(), Name = "Кадровик" },
        new Post { PostId = Guid.NewGuid(), Name = "Логист" }
    };

            // 4. Создаем статусы задач
            var tasksStatuses = new List<TasksStatus> { new TasksStatus { TasksStatusId = Guid.NewGuid(), Name = "Назначен приём", TypeStatus = 0, MessageTemplate = null }, new TasksStatus { TasksStatusId = Guid.NewGuid(), Name = "Не пришел", TypeStatus = 2, MessageTemplate = "Кандидат [Candidate.Surname] [Candidate.FirstName] на должности [Candidate.Post] не пришел к кадровику" }, new TasksStatus { TasksStatusId = Guid.NewGuid(), Name = "Время подтверждено", TypeStatus = 3, MessageTemplate = "Дата и время [Date] [Time] для кандидата [Candidate.Surname] [Candidate.FirstName] были подтвеждена" }, new TasksStatus { TasksStatusId = Guid.NewGuid(), Name = "Пришел", TypeStatus = 2, MessageTemplate = "Кандидат [Candidate.Surname] [Candidate.FirstName] на должности [Candidate.Post] успешно принят на работу" }, new TasksStatus { TasksStatusId = Guid.NewGuid(), Name = "Дата подтверждена", TypeStatus = 4, MessageTemplate = "Дата [Date] для кандидата [Candidate.Surname] [Candidate.FirstName] была подтвеждена" }, new TasksStatus { TasksStatusId = Guid.NewGuid(), Name = "Назначено собеседование", TypeStatus = 0, MessageTemplate = null }, new TasksStatus { TasksStatusId = Guid.NewGuid(), Name = "Время отказано", TypeStatus = 3, MessageTemplate = "Дата и время [Date] [Time] для кандидата [Candidate.Surname] [Candidate.FirstName] не были подтвеждена. Пожалуйста выберите новое время" }, new TasksStatus { TasksStatusId = Guid.NewGuid(), Name = "Взят кандидат", TypeStatus = 3, MessageTemplate = "Кандидат [Candidate.Surname] [Candidate.FirstName] на должности [Candidate.Post] был взят в обработку кадровиком [User.Surname] [User.FirstName] и может быть принят(а) [Date]" }, new TasksStatus { TasksStatusId = Guid.NewGuid(), Name = "Подтверждение даты", TypeStatus = 3, MessageTemplate = null }, new TasksStatus { TasksStatusId = Guid.NewGuid(), Name = "Собеседование пройдено", TypeStatus = 2, MessageTemplate = "Кандидат [Candidate.Surname] [Candidate.FirstName] прошел(шла) собеседование на должность [Candidate.Post] и ожидает выбора кадровика" }, new TasksStatus { TasksStatusId = Guid.NewGuid(), Name = "Дата отказана", TypeStatus = 4, MessageTemplate = "Дата [Date] для кандидата [Candidate.Surname] [Candidate.FirstName] не была подтвеждена. Пожалуйста выберите новую дату" }, new TasksStatus { TasksStatusId = Guid.NewGuid(), Name = "Подтверждение времени", TypeStatus = 3, MessageTemplate = null }, new TasksStatus { TasksStatusId = Guid.NewGuid(), Name = "Собеседование не пройдено", TypeStatus = 2, MessageTemplate = null } };

            // 5. Создаем пользователей
            var users = new List<User>();
            var adminPost = posts.First(p => p.Name == "!Админ");
            var hrPost = posts.First(p => p.Name == "HR");
            var кадровикPost = posts.First(p => p.Name == "Кадровик");

            // Данные для генерации случайных пользователей
            var surnames = new[] { "Иванов", "Петров", "Сидоров", "Кузнецов", "Новиков", "Смирнов" };
            var firstNames = new[] { "Алексей", "Мария", "Дмитрий", "Екатерина", "Игорь", "Ольга" };
            var patronymics = new[] { "Алексеевич", "Ивановна", "Сергеевич", "Петровна", "Владимирович", "Николаевна" };
            var imagePaths = new[] { "profile_photo/ava1.png", "profile_photo/ava2.png", "profile_photo/ava3.png",
                            "profile_photo/ava4.png", "profile_photo/ava5.png", "profile_photo/ava6.png", "default.jpg" };
            var random = new Random();
            const string defaultPasswordHash = "$2a$11$rxIpj2vtErDmhuUc8isouODt3yOtce5bKfKQ.3la.JCMPdCL/j8zG";

            // Администратор
            users.Add(new User
            {
                UserId = Guid.NewGuid(),
                Surname = "Кривой",
                FirstName = "Сергей",
                Patronymic = "Алексеевич",
                ImgUrl = "profile_photo/default.jpg",
                Phone = "+375291234567",
                Email = "krivoi@primum.ru",
                PasswordHash = defaultPasswordHash,
                Company = companies.First(c => c.Country.Name == "Беларусь"),
                Post = adminPost
            });

            // HR и кадровики для каждой компании
            foreach (var company in companies)
            {
                // HR
                users.Add(new User
                {
                    UserId = Guid.NewGuid(),
                    Surname = surnames[random.Next(surnames.Length)],
                    FirstName = firstNames[random.Next(firstNames.Length)],
                    Patronymic = patronymics[random.Next(patronymics.Length)],
                    ImgUrl = imagePaths[random.Next(imagePaths.Length)],
                    Phone = $"+37544{random.Next(1000000, 9999999)}",
                    Email = $"{company.Name.ToLower()}-hr-{random.Next(1000)}@example.com",
                    PasswordHash = defaultPasswordHash,
                    Company = company,
                    Post = hrPost
                });

                // Кадровик
                users.Add(new User
                {
                    UserId = Guid.NewGuid(),
                    Surname = surnames[random.Next(surnames.Length)],
                    FirstName = firstNames[random.Next(firstNames.Length)],
                    Patronymic = patronymics[random.Next(patronymics.Length)],
                    ImgUrl = imagePaths[random.Next(imagePaths.Length)],
                    Phone = $"+37529{random.Next(1000000, 9999999)}",
                    Email = $"{company.Name.ToLower()}-kadrovik-{random.Next(1000)}@example.com",
                    PasswordHash = defaultPasswordHash,
                    Company = company,
                    Post = кадровикPost
                });
            }

            // 6. Добавляем ВСЕ данные в контекст
            _dBContext.Country.AddRange(belarus, kazakhstan, russia, uae, china);
            _dBContext.Company.AddRange(companies);
            _dBContext.Post.AddRange(posts);
            _dBContext.TasksStatus.AddRange(tasksStatuses);
            _dBContext.User.AddRange(users);

            // 7. Сохраняем ВСЕ изменения одним вызовом
            await _dBContext.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("/crtCandidate")]
        public async Task<ActionResult> crtCandidate()
        {
            var belarus = _dBContext.Country.FirstOrDefault(c => c.Name == "Беларусь");
            if (belarus == null)
                throw new InvalidOperationException("Страна 'Беларусь' не найдена в базе данных.");

            // Получаем должность "Водитель"
            var driverPost = _dBContext.Post.FirstOrDefault(p => p.Name == "Водитель");
            if (driverPost == null)
                throw new InvalidOperationException("Должность 'Водитель' не найдена в базе данных.");

            var candidates = new List<Candidate>
            {
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Иванов",
                    FirstName = "Алексей",
                    Patronymic = "Петрович",
                    Phone = "+375291234567",
                    Email = "ivanov.alexei@mail.by",
                    Country = belarus,
                    Post = driverPost
                },
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Петрова",
                    FirstName = "Мария",
                    Patronymic = "Владимировна",
                    Phone = "+375293334455",
                    Email = "petrova.maria@mail.by",
                    Country = belarus,
                    Post = driverPost
                },
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Сидоров",
                    FirstName = "Дмитрий",
                    Patronymic = "Игоревич",
                    Phone = "+375292223344",
                    Email = "sidorov.dmitry@mail.by",
                    Country = belarus,
                    Post = driverPost
                },
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Новикова",
                    FirstName = "Екатерина",
                    Patronymic = "Сергеевна",
                    Phone = "+375294556778",
                    Email = "novikova.ekaterina@mail.by",
                    Country = belarus,
                    Post = driverPost
                },
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Кузнецов",
                    FirstName = "Игорь",
                    Patronymic = "Александрович",
                    Phone = "+375296789012",
                    Email = "kuznetsov.igor@mail.by",
                    Country = belarus,
                    Post = driverPost
                },
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Смирнова",
                    FirstName = "Ольга",
                    Patronymic = "Николаевна",
                    Phone = "+375297654321",
                    Email = "smirnova.olga@mail.by",
                    Country = belarus,
                    Post = driverPost
                },
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Зайцев",
                    FirstName = "Виктор",
                    Patronymic = "Федорович",
                    Phone = "+375293213141",
                    Email = "zaitsev.viktor@mail.by",
                    Country = belarus,
                    Post = driverPost
                },
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Борисова",
                    FirstName = "Алина",
                    Patronymic = "Михайловна",
                    Phone = "+375292314151",
                    Email = "borisova.alina@mail.by",
                    Country = belarus,
                    Post = driverPost
                },
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Трофимов",
                    FirstName = "Артур",
                    Patronymic = "Валентинович",
                    Phone = "+375293216171",
                    Email = "trofimov.artur@mail.by",
                    Country = belarus,
                    Post = driverPost
                },
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Кравцова",
                    FirstName = "Анна",
                    Patronymic = "Георгиевна",
                    Phone = "+375294414515",
                    Email = "kravtsova.anna@mail.by",
                    Country = belarus,
                    Post = driverPost
                },
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Волков",
                    FirstName = "Александр",
                    Patronymic = "Фёдорович",
                    Phone = "+375291111111",
                    Email = "volkov.alexandr@mail.by",
                    Country = belarus,
                    Post = driverPost
                },
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Максимова",
                    FirstName = "Алина",
                    Patronymic = "Игоревна",
                    Phone = "+375292222222",
                    Email = "maksimova.alina@mail.by",
                    Country = belarus,
                    Post = driverPost
                },
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Гончаров",
                    FirstName = "Владислав",
                    Patronymic = "Анатольевич",
                    Phone = "+375293333333",
                    Email = "goncharov.vladislav@mail.by",
                    Country = belarus,
                    Post = driverPost
                },
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Попова",
                    FirstName = "Анна",
                    Patronymic = "Евгеньевна",
                    Phone = "+375294444444",
                    Email = "popova.anna@mail.by",
                    Country = belarus,
                    Post = driverPost
                },
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Медведев",
                    FirstName = "Павел",
                    Patronymic = "Дмитриевич",
                    Phone = "+375295555555",
                    Email = "medvedev.pavel@mail.by",
                    Country = belarus,
                    Post = driverPost
                },
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Орлова",
                    FirstName = "Елизавета",
                    Patronymic = "Александровна",
                    Phone = "+375296666666",
                    Email = "orlova.elizaveta@mail.by",
                    Country = belarus,
                    Post = driverPost
                },
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Романов",
                    FirstName = "Михаил",
                    Patronymic = "Васильевич",
                    Phone = "+375297777777",
                    Email = "romanov.mikhail@mail.by",
                    Country = belarus,
                    Post = driverPost
                },
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Климова",
                    FirstName = "Дарья",
                    Patronymic = "Ильинична",
                    Phone = "+375298888888",
                    Email = "klimova.darya@mail.by",
                    Country = belarus,
                    Post = driverPost
                },
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Соколова",
                    FirstName = "Вера",
                    Patronymic = "Николаевна",
                    Phone = "+375299999999",
                    Email = "sokolova.vera@mail.by",
                    Country = belarus,
                    Post = driverPost
                },
                new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = "Белоусов",
                    FirstName = "Константин",
                    Patronymic = "Петрович",
                    Phone = "+375290101010",
                    Email = "belousov.konstantin@mail.by",
                    Country = belarus,
                    Post = driverPost
                }

            };

            // Добавляем кандидатов в контекст
            _dBContext.Candidate.AddRange(candidates);

            // Сохраняем изменения
            await _dBContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("/crtTask")]
        public async Task<ActionResult> crtTask()
        {
            // Получение статуса "Назначено собеседование"
            var interviewStatus = _dBContext.TasksStatus.FirstOrDefault(ts => ts.Name == "Назначено собеседование");
            if (interviewStatus == null)
                throw new InvalidOperationException("Статус 'Назначено собеседование' не найден.");

            // Получение всех HR пользователей из Беларуси
            var hrUsers = _dBContext.User
                .Where(u => u.Post.Name == "HR" && u.Company.Country.Name == "Беларусь")
                .ToList();

            if (!hrUsers.Any())
                throw new InvalidOperationException("Не найдены HR пользователи из Беларуси.");

            // Получение всех кандидатов из Беларуси
            var belarusCandidates = _dBContext.Candidate
                .Where(c => c.Country.Name == "Беларусь")
                .ToList();

            if (!belarusCandidates.Any())
                throw new InvalidOperationException("Не найдены кандидаты из Беларуси.");

            // Генерация задач
            var tasks = new List<Tasks>();
            var random = new Random();

            foreach (var candidate in belarusCandidates)
            {
                // Выбор случайного HR
                var randomHr = hrUsers[random.Next(hrUsers.Count)];

                // Создание задачи для кандидата
                tasks.Add(new Tasks
                {
                    TasksId = Guid.NewGuid(),
                    Status = interviewStatus,
                    DateTime = DateTime.Now.AddDays(random.Next(-10, -1)), // Случайная дата в течение месяца
                    User = randomHr,
                    Candidate = candidate,
                    IsArchive = false
                });
            }

            // Добавление задач в контекст базы данных
            _dBContext.Tasks.AddRange(tasks);

            // Сохранение изменений в базе данных
            await _dBContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("/deleteAll")]
        public async Task<ActionResult> deleteAll()
        {
            // Отключение проверки внешних ключей
            await _dBContext.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable @command1 = 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");

            // Очистка всех таблиц
            await _dBContext.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable @command1 = 'DELETE FROM ?'");

            // Включение проверки внешних ключей
            await _dBContext.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable @command1 = 'ALTER TABLE ? CHECK CONSTRAINT ALL'");

            Console.WriteLine("Все таблицы успешно очищены.");
            return Ok();
        }

        [HttpPost("/deleteTasks")]
        public async Task<ActionResult> deleteTasks()
        {
            // Удаление всех данных из таблицы Tasks
            await _dBContext.Database.ExecuteSqlRawAsync("DELETE FROM [Tasks]");

            Console.WriteLine("Таблица Tasks успешно очищена.");
            return Ok();
        }

        [HttpPost("/updAllNotificationsText")]
        public async Task UpdateAllNotificationsTextAsync()
        {
            // Получаем все задачи из базы данных
            var notifications = await _dBContext.Notifications
                .Include(t => t.Task)
                .Include(t => t.Task.Status)
                .Include(t => t.Task.Candidate)
                .Include(t => t.Task.User)
                .Include(t => t.Task.Candidate.Post)
                .ToListAsync();

            // Проходимся по каждой задаче и обновляем текст уведомлений
            foreach (var notification in notifications)
            {
                var candidate = notification.Task.Candidate;
                var user = notification.Task.User;

                if (candidate == null || user == null)
                {
                    throw new InvalidOperationException($"Задача с ID {notification.Task.TasksId} не содержит информации о кандидате или пользователе.");
                }

                if (notification.Task.Status.MessageTemplate != null)
                {
                    if (notification.Task.Status.Name == "Взят кандидат" ) 
                        Console.WriteLine("ff");
                    // Формирование текста уведомления
                    var updatedMessage = notification.Task.Status.MessageTemplate
                        .Replace("[Candidate.Surname]", candidate.Surname)
                        .Replace("[Candidate.FirstName]", candidate.FirstName)
                        .Replace("[Candidate.Post]", candidate.Post?.Name ?? "Должность не указана")
                        .Replace("[User.Surname]", user.Surname)
                        .Replace("[User.FirstName]", user.FirstName)
                        .Replace("[Date]", notification.Task.DateTime.ToShortDateString())
                        .Replace("[DateTime]", notification.Task.DateTime.ToString("dd-MM-yyyy HH:mm"));

                    // Обновляем текст уведомления в задаче
                    notification.TextMessage = updatedMessage;
                }
            }

            // Сохраняем изменения в базе данных
            await _dBContext.SaveChangesAsync();
        }

    }
}
