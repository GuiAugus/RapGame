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
                .ThenInclude(aa => aa.Artista)
            .ToListAsync();

            var albunsDto = albuns.Select(album => new AlbumDto
                 {
                    Id = album.Id,
                    Nome = album.Nome,
                    QuantidadeFaixas = album.QuantidadeFaixas,
                    AlbumDate = album.AlbumDate,
                    FaixaMaisPopular = album.FaixaMaisPopular,
                    ArtistaIds = album.AlbumArtistas.Select(aa => aa.ArtistId).ToList(),
                    ArtistaPrincipais = album.AlbumArtistas.Select(aa => aa.Artista.Nome).ToList()
                }).ToList();

            return Ok(albunsDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AlbumDto>> GetAlbum(int id)
        {
            var album = await _context.Albuns
                .Include(a => a.AlbumArtistas)
                    .ThenInclude(aa => aa.Artista)
                .Include(a => a.Participacoes)
                    .ThenInclude(ap => ap.Artista)
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
                ArtistaIds = album.AlbumArtistas.Select(aa => aa.ArtistId).ToList(),
                ArtistaPrincipais = album.AlbumArtistas.Select(aa => aa.Artista.Nome).ToList(),
                ArtistaParticipacoesIds = album.Participacoes.Select(p => p.ArtistaId).ToList(),
                ArtistaParticipacoes = album.Participacoes.Select(ap => ap.Artista.Nome).ToList()
            };

            return Ok(albumDto);
        }

        [HttpPost]
        public async Task<ActionResult<Album>> PostAlbum(AlbumDto albumDto)
        {
            if (albumDto == null)
            {
                return BadRequest("O corpo da requisição não pode estar vazio.");
            }

            var album = new Album
            {
                Nome = albumDto.Nome,
                QuantidadeFaixas = albumDto.QuantidadeFaixas,
                AlbumDate = albumDto.AlbumDate,
                FaixaMaisPopular = albumDto.FaixaMaisPopular
            };

            _context.Albuns.Add(album);
            await _context.SaveChangesAsync(); 

            var artistaIds = albumDto.ArtistaIds.Concat(albumDto.ArtistaParticipacoesIds).Distinct().ToList();

            var artistas = await _context.Artistas
                .Where(a => artistaIds.Contains(a.Id))
                .ToListAsync();


            var artistasExistentesIds = artistas.Select(a => a.Id).ToHashSet();
            if (!albumDto.ArtistaIds.All(id => artistasExistentesIds.Contains(id)) ||
                !albumDto.ArtistaParticipacoesIds.All(id => artistasExistentesIds.Contains(id)))
            {
                return BadRequest("Um ou mais artistas fornecidos não existem.");
            }

            var albumArtistas = artistas
                .Where(a => albumDto.ArtistaIds.Contains(a.Id))
                .Select(artista => new AlbumArtista
                {
                    AlbumId = album.Id,
                    Album = album, 
                    ArtistId = artista.Id,
                    Artista = artista
                }).ToList();

            var albumParticipacoes = artistas
                .Where(a => albumDto.ArtistaParticipacoesIds.Contains(a.Id))
                .Select(artista => new AlbumParticipacoes
                {
                    AlbumId = album.Id,
                    Album = album,
                    ArtistaId = artista.Id,
                    Artista = artista
                }).ToList();


            _context.AlbumArtistas.AddRange(albumArtistas);
            _context.AlbumParticipacoes.AddRange(albumParticipacoes);

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
