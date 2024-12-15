using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YavaHire.Core.Interfaces;

namespace YavaHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HRController : ControllerBase
    {
        private IHRService _HRService;
        public HRController(IHRService hRService)
        {
            _HRService = hRService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHrs()
        {
            return Ok(await _HRService.GetAll());
        }
        
    }
}
