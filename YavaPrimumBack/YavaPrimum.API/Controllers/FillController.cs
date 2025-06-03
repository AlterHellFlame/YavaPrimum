using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Interfaces;
using YavaPrimum.Core.Services;

namespace YavaPrimum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FillController : ControllerBase
    {
        private readonly YavaPrimumDBContext _dBContext;
        private ICandidateService _candidateService;
        private ICountryService _countryService;
        private IPostService _postService;
        private IUserService _userService;
        private IJwtProvider _jwtProvider;
        private ITasksService _tasksService;
        private INotificationsService _notificationsService;
        private IConverterService _converterService;

        public FillController(
            ICandidateService candidateService,
            ICountryService countryService,
            IPostService postService,
            IUserService userService,
            IJwtProvider jwtProvider,
            ITasksService tasksService,
            INotificationsService notificationsService,
            IConverterService converterService,
            YavaPrimumDBContext dBContext
            )
        {
            _candidateService = candidateService;
            _countryService = countryService;
            _postService = postService;
            _userService = userService;
            _jwtProvider = jwtProvider;
            _tasksService = tasksService;
            _notificationsService = notificationsService;
            _converterService = converterService;
            _dBContext = dBContext;
        }


        private Country FindCountry(string countryName, List<Country> countries)
        {
            return countries.FirstOrDefault(c => c.Name == countryName) ?? throw new InvalidOperationException($"Страна '{countryName}' не найдена в базе данных.");
        }

        [HttpPost("/crtBase")]
        public async Task<ActionResult> crtBase()
        {

            var countries = new List<Country>
            {
                new Country
                {
                    CountryId = Guid.NewGuid(),
                    Name = "Беларусь",
                    PhoneMask = "+375 (XX) XXX-XX-XX"
                },
                new Country
                {
                    CountryId = Guid.NewGuid(),
                    Name = "Казахстан",
                    PhoneMask = "+7 (XXX) XXX-XX-XX"
                },
                new Country
                {
                    CountryId = Guid.NewGuid(),
                    Name = "Россия",
                    PhoneMask = "+7 (XXX) XXX-XX-XX"
                },
                new Country
                {
                    CountryId = Guid.NewGuid(),
                    Name = "Узбекистан",
                    PhoneMask = "+998 (XX) XXX-XX-XX"
                },
                new Country
                {
                    CountryId = Guid.NewGuid(),
                    Name = "Монголия",
                    PhoneMask = "+976 (XX) XXXX-XXX"
                },
                new Country
                {
                    CountryId = Guid.NewGuid(),
                    Name = "Польша",
                    PhoneMask = "+48 (XXX) XXX-XXX"
                }
            };

            // 2. Создаем компании
            var companies = new List<Company>
            {
            // Россия
            new Company { CompanyId = Guid.NewGuid(), Name = "АО Вестинтертранс", Country = FindCountry("Россия", countries) },
            new Company { CompanyId = Guid.NewGuid(), Name = "АО Рус-Авто", Country = FindCountry("Россия", countries) },
            new Company { CompanyId = Guid.NewGuid(), Name = "ООО Первый элемент", Country = FindCountry("Россия", countries) },

            // Беларусь
            new Company { CompanyId = Guid.NewGuid(), Name = "ООО Примвэй", Country = FindCountry("Беларусь", countries) },

            // Узбекистан
            new Company { CompanyId = Guid.NewGuid(), Name = "ООО IAT Cargo Corp", Country = FindCountry("Узбекистан", countries) },

            // Казахстан
            new Company { CompanyId = Guid.NewGuid(), Name = "ТОО Примавто", Country = FindCountry("Казахстан", countries) },
            new Company { CompanyId = Guid.NewGuid(), Name = "ТОО ИнтерАвтоТранс", Country = FindCountry("Казахстан", countries) },

            // Монголия
            new Company { CompanyId = Guid.NewGuid(), Name = "КОО Браво Авто", Country = FindCountry("Монголия", countries) },

            // Польша
            new Company { CompanyId = Guid.NewGuid(), Name = "Starprim sp. z o.o.", Country = FindCountry("Польша", countries) }
            };

            // 3. Создаем должности
            var posts = new List<Post>
            {
                new Post { PostId = Guid.NewGuid(), Name = "!Админ" },
                new Post { PostId = Guid.NewGuid(), Name = "HR" },
                new Post { PostId = Guid.NewGuid(), Name = "Кадровик" },

                // Дополнительные должности из логистики
                new Post { PostId = Guid.NewGuid(), Name = "Водитель-дальнобойщик" },
                new Post { PostId = Guid.NewGuid(), Name = "Водитель-экспедитор" },
                new Post { PostId = Guid.NewGuid(), Name = "Водитель категории C" },
                new Post { PostId = Guid.NewGuid(), Name = "Водитель категории E" },
                new Post { PostId = Guid.NewGuid(), Name = "Водитель автобуса" },
                new Post { PostId = Guid.NewGuid(), Name = "Водитель спецтехники" },
                new Post { PostId = Guid.NewGuid(), Name = "Оператор складского транспорта" },
                new Post { PostId = Guid.NewGuid(), Name = "Координатор логистики" },
                new Post { PostId = Guid.NewGuid(), Name = "Механик по грузовым авто" },
                new Post { PostId = Guid.NewGuid(), Name = "Диспетчер транспортной компании" }
            };


            // 4. Создаем статусы задач
            var tasksStatuses = new List<TasksStatus>
{
    new TasksStatus { TasksStatusId = Guid.Parse("5FC1E2EF-6F18-46D7-9046-45896819967A"), Name = "Ожидается подтверждение даты", TypeStatus = 3, MessageTemplate = "Новая дата [Date] для кандидата [Candidate.Surname] [Candidate.FirstName] ожидает подтверждения" },
    new TasksStatus { TasksStatusId = Guid.Parse("EF5BF5E5-0B1C-4D85-B54C-63C625091E0E"), Name = "Взят кандидат", TypeStatus = 3, MessageTemplate = "Кандидат [Candidate.Surname] [Candidate.FirstName] на должности [Candidate.Post] был взят в обработку кадровиком [User.Surname] [User.FirstName] и может быть принят(а) [Date]" },
    new TasksStatus { TasksStatusId = Guid.Parse("6C9C58CE-2E4A-43F7-850D-720F1DAB0C36"), Name = "Ожидается подтверждение времени", TypeStatus = 3, MessageTemplate = "Время [Time] для кандидата [Candidate.Surname] [Candidate.FirstName] ожидает подтверждения" },
    new TasksStatus { TasksStatusId = Guid.Parse("B69AAEDD-39CE-4BEF-861D-7AB27A04ABE2"), Name = "Собеседование не пройдено", TypeStatus = 2, MessageTemplate = null },
    new TasksStatus { TasksStatusId = Guid.Parse("5ADF907C-2AEC-4D63-9414-80D2336A6707"), Name = "Собеседование пройдено", TypeStatus = 2, MessageTemplate = "Кандидат [Candidate.Surname] [Candidate.FirstName] прошел(шла) собеседование на должность [Candidate.Post] и ожидает выбора кадровика" },
    new TasksStatus { TasksStatusId = Guid.Parse("E5FC2163-FFEB-4931-B631-8801F1B8F0AF"), Name = "Не пришел", TypeStatus = 2, MessageTemplate = "Кандидат [Candidate.Surname] [Candidate.FirstName] на должности [Candidate.Post] не пришел к кадровику" },
    new TasksStatus { TasksStatusId = Guid.Parse("24EAC6D7-4C0D-4254-A157-67771A79E53B"), Name = "Пришел", TypeStatus = 2, MessageTemplate = "Кандидат [Candidate.Surname] [Candidate.FirstName] на должности [Candidate.Post] успешно принят на работу" },
    new TasksStatus { TasksStatusId = Guid.Parse("0D3C45D8-8CEF-4D9E-AEF9-2E2AB6DE9D2E"), Name = "Выполнено тестовое задание", TypeStatus = 2, MessageTemplate = "Кандидат [Candidate.Surname] [Candidate.FirstName] прошел(шла) собеседование на должность [Candidate.Post] и ожидает выбора кадровика" },
    new TasksStatus { TasksStatusId = Guid.Parse("5B30069B-1BB4-4C12-A7D7-E62263981CBF"), Name = "Не выполнено тестовое задание", TypeStatus = 2, MessageTemplate = null },
    new TasksStatus { TasksStatusId = Guid.Parse("BB48E8BA-6D5B-4821-A0CA-F49D21AADC1F"), Name = "Назначено собеседование", TypeStatus = 0, MessageTemplate = null },
    new TasksStatus { TasksStatusId = Guid.Parse("DD100C62-83B9-47C3-AF1D-3F3D935FB4FA"), Name = "Назначен приём", TypeStatus = 0, MessageTemplate = null },
    new TasksStatus { TasksStatusId = Guid.Parse("87B85A86-2B26-4931-973D-201EBF938574"), Name = "Срок тестового задания", TypeStatus = 0, MessageTemplate = "Кандидат [Candidate.Surname] должен выполнить тестовое задание до [Date]" },
    new TasksStatus { TasksStatusId = Guid.Parse("CBBE4BB0-8A9D-4830-BDA0-9713E5D7FE11"), Name = "Подтверждение даты", TypeStatus = -1, MessageTemplate = null },
    new TasksStatus { TasksStatusId = Guid.Parse("6A6F5BD2-FB98-4E66-BE74-B0DABD372BD4"), Name = "Запрос на смену времени", TypeStatus = -1, MessageTemplate = "Кандидат [Candidate.Surname] [Candidate.FirstName] на должности [Candidate.Post] нуждается в смене времени" },
    new TasksStatus { TasksStatusId = Guid.Parse("E9DED4B6-D1BC-4460-BA8E-769C48C69E6F"), Name = "Запрос на смену даты", TypeStatus = -1, MessageTemplate = "Кандидат [Candidate.Surname] [Candidate.FirstName] на должности [Candidate.Post] нуждается в смене даты" },
    new TasksStatus { TasksStatusId = Guid.Parse("FEF06092-F6BB-4896-BA3E-F85E61309138"), Name = "Подтверждение времени", TypeStatus = -1, MessageTemplate = null },
    new TasksStatus { TasksStatusId = Guid.Parse("EC75AFA1-0E3A-43BE-ABD7-83862AF3AF86"), Name = "Дата подтверждена", TypeStatus = -2, MessageTemplate = "Дата [Date] для кандидата [Candidate.Surname] [Candidate.FirstName] была подтвеждена" },
    new TasksStatus { TasksStatusId = Guid.Parse("B312D748-1CC3-42A0-8FFA-C304497CB29C"), Name = "Время отказано", TypeStatus = -2, MessageTemplate = "Дата и время [Date] [Time] для кандидата [Candidate.Surname] [Candidate.FirstName] не были подтвеждена. Пожалуйста выберите новое время" },
    new TasksStatus { TasksStatusId = Guid.Parse("6DB2A556-6190-4022-AB3B-66315E6C4A35"), Name = "Дата отказана", TypeStatus = -2, MessageTemplate = "Дата [Date] для кандидата [Candidate.Surname] [Candidate.FirstName] не была подтвеждена. Пожалуйста выберите новую дату" },
    new TasksStatus { TasksStatusId = Guid.Parse("6B657AD4-0BB3-42C1-9C7A-57E318C3F21A"), Name = "Время подтверждено", TypeStatus = -2, MessageTemplate = "Дата и время [Date] [Time] для кандидата [Candidate.Surname] [Candidate.FirstName] были подтвеждена" },
    new TasksStatus { TasksStatusId = Guid.Parse("7D8E09B4-335A-4EEF-943B-9D68506CBFA0"), Name = "Перенесено собеседование", TypeStatus = -2, MessageTemplate = null },
    new TasksStatus { TasksStatusId = Guid.Parse("315A68D8-C2DA-4F5C-9233-2643729608DF"), Name = "Дата была подтверждена", TypeStatus = 3, MessageTemplate = null }
};
            // 5. Создаем пользователей
            var users = new List<User>();
            var adminPost = posts.First(p => p.Name == "!Админ");
            var hrPost = posts.First(p => p.Name == "HR");
            var кадровикPost = posts.First(p => p.Name == "Кадровик");

            // Данные для генерации случайных пользователей
            var surnames = new[] {
                "Витченко", "Хлыстов", "Сидоров", "Цылев", "Бланш", "Будейко", "Белов", "Новиков",
                "Масюченя", "Шивалец", "Бусенко", "Орлов", "Миронов", "Гаврилов", "Жуков", "Тихонов",
                "Фомин", "Зимин", "Кондратьев", "Давыдов", "Ершов", "Калинин"
            };
                        var firstNames = new[] {
                "Алексей", "Платон", "Дмитрий", "Пётр", "Игорь", "Марк", "Сергей", "Владислав",
                "Николай", "Артём", "Михаил", "Роман", "Василий", "Тимофей", "Егор", "Леонид"
            };
                        var patronymics = new[] {
                "Алексеевич", "Иванов", "Сергеевич", "Петрович", "Владимирович", "Николаевич",
                "Максимович", "Андреевич", "Фёдорович", "Геннадьевич", "Тимофеевич", "Артёмович",
                "Станиславович", "Леонидович"
            };

            var imagePaths = new[] {
            "profile_photo/ava1.png", "profile_photo/ava2.png", "profile_photo/ava3.png",
            "profile_photo/ava4.png", "profile_photo/ava5.png", "profile_photo/ava6.png",
            "profile_photo/ava7.png", "profile_photo/ava8.png", "profile_photo/ava9.png",
            "profile_photo/ava10.png", "profile_photo/ava11.png", "default.png", "default.png", "default.png", "default.png"
};

            var random = new Random();
            const string defaultPasswordHash = "$2a$11$rxIpj2vtErDmhuUc8isouODt3yOtce5bKfKQ.3la.JCMPdCL/j8zG";

            // Администратор
            users.Add(new User
            {
                UserId = Guid.NewGuid(),
                Surname = "Кривой",
                FirstName = "Сергей",
                Patronymic = "Алексеевич",
                ImgUrl = "profile_photo/default.png",
                Phone = "+375291234567",
                Email = "krivoi@primum.ru",
                PasswordHash = defaultPasswordHash,
                Company = companies.First(c => c.Country.Name == "Беларусь"),
                Post = adminPost
            });

            foreach (var company in companies)
            {
                // Определяем количество кадровиков (минимум 1, максимум 2)
                int kadrovikCount = random.Next(1, 3);

                for (int i = 0; i < kadrovikCount; i++)
                {
                    users.Add(new User
                    {
                        UserId = Guid.NewGuid(),
                        Surname = surnames[random.Next(surnames.Length)],
                        FirstName = firstNames[random.Next(firstNames.Length)],
                        Patronymic = patronymics[random.Next(patronymics.Length)],
                        ImgUrl = imagePaths[random.Next(imagePaths.Length)],
                        Phone = $"+37529{random.Next(1000000, 9999999)}",
                        Email = $"{company.Name.ToLower()}Kadr{random.Next(1000)}@primum.com",
                        PasswordHash = defaultPasswordHash,
                        Company = company,
                        Post = кадровикPost
                    });
                }

                users.Add(new User
                {
                    UserId = Guid.NewGuid(),
                    Surname = surnames[random.Next(surnames.Length)],
                    FirstName = firstNames[random.Next(firstNames.Length)],
                    Patronymic = patronymics[random.Next(patronymics.Length)],
                    ImgUrl = imagePaths[random.Next(imagePaths.Length)],
                    Phone = $"+37544{random.Next(1000000, 9999999)}",
                    Email = $"{company.Name.ToLower()}Hr{random.Next(1000)}@primum.com",
                    PasswordHash = defaultPasswordHash,
                    Company = company,
                    Post = hrPost
                });

            }


            // 6. Добавляем ВСЕ данные в контекст
            _dBContext.Country.AddRange(countries);
            _dBContext.Company.AddRange(companies);
            _dBContext.Post.AddRange(posts);
            _dBContext.TasksStatus.AddRange(tasksStatuses);
            _dBContext.User.AddRange(users);

            // 7. Сохраняем ВСЕ изменения одним вызовом
            await _dBContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("/crtAnTask")]
        public async Task<ActionResult> crtAnTask()
        {
            // Получение статуса "Назначено собеседование"
            var interviewStatus = _dBContext.TasksStatus.FirstOrDefault(ts => ts.Name == "Назначено собеседование");
            if (interviewStatus == null)
                throw new InvalidOperationException("Статус 'Назначено собеседование' не найден.");

            // Получение всех компаний по странам
            var companiesByCountry = _dBContext.Company
                .GroupBy(c => c.Country.Name)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Получение всех HR пользователей по странам
            var hrUsersByCountry = _dBContext.User
                .Where(u => u.Post.Name == "HR")
                .GroupBy(u => u.Company.Country.Name)
                .ToDictionary(g => g.Key, g => g.ToList());

            var candidatesByCountry = _dBContext.Candidate
                .GroupBy(c => c.Country.Name)
                .ToDictionary(g => g.Key, g => g.ToList());

            var tasks = new List<Tasks>();
            var random = new Random();

            foreach (var (country, candidates) in candidatesByCountry)
            {
                if (!hrUsersByCountry.ContainsKey(country) || hrUsersByCountry[country].Count == 0)
                    throw new InvalidOperationException($"Не найдены HR пользователи в стране '{country}'.");

                if (!companiesByCountry.ContainsKey(country) || companiesByCountry[country].Count == 0)
                    throw new InvalidOperationException($"Не найдены компании в стране '{country}'.");

                foreach (var candidate in candidates)
                {
                    // Выбор случайного HR из той же страны
                    var randomHr = hrUsersByCountry[country][random.Next(hrUsersByCountry[country].Count)];

                    // Создание задачи для кандидата
                    tasks.Add(new Tasks
                    {
                        TasksId = Guid.NewGuid(),
                        Status = interviewStatus,
                        DateTime = DateTime.Now.AddDays(random.Next(-100, 0)), // Случайная дата за последние 30 дней
                        User = randomHr,
                        Candidate = candidate,
                        IsArchive = false
                    });
                }
            }

                // Добавление задач в контекст базы данных
                _dBContext.Tasks.AddRange(tasks);

            // Сохранение изменений
            await _dBContext.SaveChangesAsync();


            foreach (var task in tasks)
            {

                StatusUpdateRequest statusUpdateRequest = new StatusUpdateRequest()
                {
                    Status = random.Next(100) < 50
                    ? "Собеседование не пройдено"
                    : "Собеседование пройдено",

                };


                await _tasksService.SetNewStatus(task, statusUpdateRequest);
            }

            var completedInterviews = _dBContext.Tasks
                .Include(t => t.Candidate)
                .Include(t => t.Candidate.Country)
                .Include(t => t.Candidate.Post)
                .Where(t => t.Status.Name == "Собеседование пройдено")
                .ToList();

            foreach (var task in completedInterviews)
            {
                var candidate = _dBContext.Candidate.FirstOrDefault(c => c.CandidateId == task.Candidate.CandidateId);

                if (candidate != null)
                {
                    var hrUser = _dBContext.User
                        .FirstOrDefault(u => u.Post.Name == "Кадровик" && u.Company.Country == candidate.Country);


                    Tasks newTask = new Tasks
                    {
                        TasksId = Guid.NewGuid(),
                        Status = await _tasksService.GetStatusByName("Взят кандидат"),
                        Candidate = task.Candidate,
                        User = hrUser,
                        DateTime = Convert.ToDateTime(task.DateTime.AddDays(random.Next(0, 10))),
                        AdditionalData = task.AdditionalData
                    };
                    await _tasksService.Create(newTask);


                    await _notificationsService.ReadNotificationOfCandidate(task.Candidate.CandidateId);
                   // await _notificationsService.SendMessage(newTask);

                    await _dBContext.SaveChangesAsync();




                }
            }

            var completedKadr = _dBContext.Tasks
                .Include(t => t.Candidate)
                .Include(t => t.Candidate.Country)
                .Include(t => t.Candidate.Post)
                .Where(t => t.Status.Name == "Взят кандидат")
                .ToList();

            foreach (var task in completedKadr)
            {
                if (random.Next(100) > 80)
                {
                    task.Status = await _tasksService.GetStatusByName(random.Next(100) < 80
                  ? "Пришел"
                  : "Не пришел");

                

                    await _notificationsService.ReadNotificationOfCandidate(task.Candidate.CandidateId);
                    await _notificationsService.SendMessage(task);

                    await _dBContext.SaveChangesAsync();
                }

            }

            return Ok();

        }


        [HttpPost("/crtCandidate")]
        public async Task<ActionResult> crtCandidate()
        {
            //await _dBContext.Database.ExecuteSqlRawAsync("DELETE FROM [Tasks]");
            //await _dBContext.Database.ExecuteSqlRawAsync("DELETE FROM [Candidate]");
            int count = 200;
            Random _random = new Random();

            var countries = _dBContext.Country.ToList();
            var posts = _dBContext.Post.ToList();

            if (!countries.Any() || !posts.Any())
                throw new InvalidOperationException("В базе данных отсутствуют страны или должности.");

            var maleSurnames = new[] { "Аракчеев", "Левицкий", "Добронравов", "Ромашов", "Крутов", "Шахматов", "Ветроградский", "Черемных", "Звягинцев", "Сафронов", "Мелехов", "Гнездилов" };
            var femaleSurnames = new[] { "Брусницкая", "Орехова", "Зорина", "Лебедева", "Погодина", "Глинская", "Федорова", "Рахманова", "Цветаева", "Чайковская", "Крылова", "Булатова" };

            var maleFirstNames = new[] { "Арсений", "Виталий", "Тимур", "Лаврентий", "Григорий", "Елисей", "Святослав", "Ростислав", "Анатолий", "Всеволод", "Артемий", "Захар" };
            var femaleFirstNames = new[] { "Агата", "Милана", "Есения", "Таисия", "Мирослава", "Лада", "Елена", "Анастасия", "Вера", "Полина", "Лилия", "Алёна" };

            var malePatronymics = new[] { "Петрович", "Игоревич", "Александрович", "Федорович", "Михайлович", "Васильевич", "Анатольевич", "Станиславович", "Григорьевич", "Дмитриевич", "Николаевич", "Тимофеевич" };
            var femalePatronymics = new[] { "Петровна", "Игоревна", "Александровна", "Федоровна", "Михайловна", "Васильевна", "Анатольевна", "Станиславовна", "Григорьевна", "Дмитриевна", "Николаевна", "Тимофеевна" };

            var candidates = new List<Candidate>();

            for (int i = 0; i < count; i++)
            {
                bool isMale = _random.Next(2) == 0;

                candidates.Add(new Candidate
                {
                    CandidateId = Guid.NewGuid(),
                    Surname = isMale ? maleSurnames[_random.Next(maleSurnames.Length)] : femaleSurnames[_random.Next(femaleSurnames.Length)],
                    FirstName = isMale ? maleFirstNames[_random.Next(maleFirstNames.Length)] : femaleFirstNames[_random.Next(femaleFirstNames.Length)],
                    Patronymic = isMale ? malePatronymics[_random.Next(malePatronymics.Length)] : femalePatronymics[_random.Next(femalePatronymics.Length)],
                    Phone = $"+37529{_random.Next(1000000, 9999999)}",
                    Email = $"{(isMale ? maleFirstNames : femaleFirstNames)[_random.Next((isMale ? maleFirstNames : femaleFirstNames).Length)].ToLower()}.{(isMale ? maleSurnames : femaleSurnames)[_random.Next((isMale ? maleSurnames : femaleSurnames).Length)].ToLower()}@mail.com",
                    Country = countries[_random.Next(countries.Count)],
                    Post = posts[_random.Next(posts.Count)]
                });
            }

            _dBContext.Candidate.AddRange(candidates);
            _dBContext.SaveChanges();


            



                return Ok("Хабиб");
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

            await _dBContext.Database.ExecuteSqlRawAsync("DELETE FROM [Notifications]");
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
