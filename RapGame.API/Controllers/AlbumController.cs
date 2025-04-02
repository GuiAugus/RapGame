using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RapGame.Data;
using RapGame.Models;

namespace RapGame.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly RapGameDbContext _context;

        public AlbumController(RapGameDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Album>>> GetAlbuns()
        {
            return await _context.Albuns.ToListAsync();
        }
    }
}
