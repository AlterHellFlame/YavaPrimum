using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Interfaces;
using YavaPrimum.Core.Services;

namespace YavaPrimum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private ICountryService _countryService;
        private IPostService _postService;
        private ICompanyService _companyService;
        private IUserService _userService;
        private IJwtProvider _jwtProvider;
        private IConverterService _converterService;
        private IVacancyService _vacancyService;

        public UserController(ICountryService countryService, IPostService postService, 
            ICompanyService companyService, IUserService userService, 
            IJwtProvider jwtProvider, IConverterService converterService, IVacancyService vacancyService)
        {
            _countryService = countryService;
            _postService = postService;
            _companyService = companyService;
            _userService = userService;
            _jwtProvider = jwtProvider;
            _converterService = converterService;
            _vacancyService = vacancyService;
        }

        [HttpPost("/update-user-data")]
        public async Task<ActionResult<UserRequestResponse>> UpdateUserData([FromBody] UserRequestResponse request)
        {
            User user = await _userService.GetById(request.UserId);


            user.Surname = request.Surname;
            user.FirstName = request.FirstName;
            user.Patronymic = request.Patronymic;
            user.Company = await _companyService.GetByName(request.Company);
            user.Email = request.Email;
            user.Phone = request.Phone;

            user.Post = await _postService.GetByName(request.Post);
            await _userService.UpdateUser(user);

            return Ok();
        }


        [HttpDelete("/delete-user/{userId:guid}")]
        public async Task<ActionResult<UserRequestResponse>> DeleteUser(Guid userId)
        {
            await _userService.DeleteUserById(userId);

            return Ok();
        }

        [HttpGet("/get-user-data")]
        public async Task<ActionResult<UserRequestResponse>> GetUserData()
        {
            if (HttpContext.Request.Cookies.Count == 0)
            {
                Console.WriteLine("Куков нет");
                return StatusCode(166, "Куков нет"); // Возвращает код ошибки 166 с сообщением
            }
            string token = HttpContext.Request.Cookies[JwtProvider.CookiesName];

            return await _userService.GetByIdToFront(await _jwtProvider.GetUserIdFromToken(token));
        }

        [HttpGet("/get-all-users-data")]
        public async Task<ActionResult<List<UserRequestResponse>>> GetAllUsersData()
        {

            return await _converterService.ConvertToFront(await _userService.GetAll());
        }

        [HttpGet("/get-all-vacancies")]
        public async Task<ActionResult<List<VacancyResponce>>> GetAllVacancies()
        {
            Console.WriteLine("get-all-vacancies");
            return await _converterService.ConvertToFront(await _vacancyService.GetAll());
        }

        [HttpPost("/create-vacancy")]
        public async Task<ActionResult> CreateVacancy([FromBody] VacancyRequest newVacancy)
        {
            if (HttpContext.Request.Cookies.Count == 0)
            {
                Console.WriteLine("Куков нет");
                return StatusCode(166, "Куков нет"); // Возвращает код ошибки 166 с сообщением
            }
            string token = HttpContext.Request.Cookies[JwtProvider.CookiesName];

            User user = await _userService.GetById(await _jwtProvider.GetUserIdFromToken(token));

            await _vacancyService.Create(newVacancy, user);
            return Ok();
        }

        [HttpPut("/update-vacancy/{vacancyId:guid}")]
        public async Task<ActionResult<Vacancy>> UpdateVacancy(Guid vacancyId, [FromBody] VacancyRequest updateRequest)
        {

            await _vacancyService.Update(vacancyId, updateRequest);
            return Ok();
        }

        [HttpPatch("/close-vacancy/{vacancyId:guid}")]
        public async Task<ActionResult> CloseVacancy(Guid vacancyId)
        {

            await _vacancyService.Close(vacancyId);
            return Ok();
        }

        [HttpGet("/get-posts-countries")]
        public async Task<ActionResult<List<PostCountryResponse>>> GetPostsCountries()
        {
            List<Post> Posts = await _postService.GetAll();
            List<Country> Countries = await _countryService.GetAll();
            List<Company> Companies = await _companyService.GetAll();

            PostCountryResponse postCountry = new PostCountryResponse()
            {
                Posts = Posts.Select(post => post.Name).ToList(),
                Countries = Countries.Select(country => country.Name).ToList(),
                Companies = Companies.Select(company => company.Name).ToList(),
                PhoneMasks = Countries.ToDictionary(country => country.Name, country => country.PhoneMask) // Создание словаря
            };

            return Ok(postCountry);
        }


    }
}
