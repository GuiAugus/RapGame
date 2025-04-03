using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RapGame.Data;
using RapGame.Models;
using RapGame.Shared.DTOs;

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
        public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAlbuns()
        {
            var albuns = await _context.Albuns
                .Include(a => a.AlbumArtistas)
                .Select(album => new AlbumDto
                {
                    Id = album.Id,
                    Nome = album.Nome,
                    QuantidadeFaixas = album.QuantidadeFaixas,
                    AlbumDate = album.AlbumDate,
                    FaixaMaisPopular = album.FaixaMaisPopular,
                    ArtistaIds = album.AlbumArtistas.Select(aa => aa.ArtistId).ToList()
                })
                .ToListAsync();

            return Ok(albuns);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAlbum(int id)
        {
            var album = await _context.Albuns
                .Include(a => a.AlbumArtistas)
                .FirstOrDefaultAsync(a => a.Id == id);

                if (album == null)
                    return NotFound();

                var albumDto = new AlbumDto
                {
                    Id = album.Id,
                    Nome = album.Nome,
                    QuantidadeFaixas = album.QuantidadeFaixas,
                    AlbumDate = album.AlbumDate,
                    FaixaMaisPopular = album.FaixaMaisPopular,
                    ArtistaIds = album.AlbumArtistas.Select(aa => aa.ArtistId).ToList()
                };

            return Ok(albumDto);
        }

        [HttpPost]
        public async Task<ActionResult<Album>> PostAlbum(AlbumDto albumDto)
        {
            var album = new Album
            {
                Nome = albumDto.Nome,
                QuantidadeFaixas = albumDto.QuantidadeFaixas,
                AlbumDate = albumDto.AlbumDate,
                FaixaMaisPopular = albumDto.FaixaMaisPopular
            };

            var artistasExistentes = await _context.Artistas
                .Where(a => albumDto.ArtistaIds.Contains(a.Id))
                .ToListAsync();

            if (artistasExistentes.Count != albumDto.ArtistaIds.Count)
            {
                return BadRequest("Um ou mais artistas fornecidos nao existem.");
            }

            album.AlbumArtistas = artistasExistentes.Select(a => new AlbumArtista
            {
                ArtistId = a.Id, 
                Album = album,
                Artista = a
            }).ToList();

            _context.Albuns.Add(album);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAlbum), new { id = album.Id }, album);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlbum(int id)
        {
            var album = await _context.Albuns
                .Include(a => a.AlbumArtistas)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
                return NotFound();

            _context.Albuns.Remove(album);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
