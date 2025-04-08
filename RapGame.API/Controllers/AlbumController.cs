using System.Text.Json;
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
        private readonly IWebHostEnvironment _env;

        public AlbumController(RapGameDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
                    AlbumDate = album.AlbumDate,
                    QuantidadeFaixas = album.QuantidadeFaixas,
                    FaixaMaisPopular = album.FaixaMaisPopular,
                    CapaUrl = album.CapaUrl,
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
                AlbumDate = album.AlbumDate,
                QuantidadeFaixas = album.QuantidadeFaixas,
                FaixaMaisPopular = album.FaixaMaisPopular,
                CapaUrl = album.CapaUrl,
                ArtistaIds = album.AlbumArtistas.Select(aa => aa.ArtistId).ToList(),
                ArtistaPrincipais = album.AlbumArtistas.Select(aa => aa.Artista.Nome).ToList(),
                ArtistaParticipacoesIds = album.Participacoes.Select(p => p.ArtistaId).ToList(),
                ArtistaParticipacoes = album.Participacoes.Select(ap => ap.Artista.Nome).ToList()
            };

            return Ok(albumDto);
        }

        [HttpPost]
        public async Task<ActionResult<Album>> PostAlbum([FromForm] string albumJson, [FromForm] IFormFile file)
        {
            if (string.IsNullOrWhiteSpace(albumJson))
            {
                return BadRequest("Dados do album estao vazios");
            }

            AlbumDto? albumDto;
            try
            {
                albumDto = JsonSerializer.Deserialize<AlbumDto>(albumJson);
            }
            catch
            {
                return BadRequest("Erro ao converter dados do album.");
            }

            if (albumDto == null)
                return BadRequest("Dados do album invalido");

            string CapaUrl = null!;
            if (file != null && file.Length > 0)
            {
                var nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var caminhoCapas = Path.Combine(_env.WebRootPath, "capas");

                if(!Directory.Exists(caminhoCapas))
                    Directory.CreateDirectory(caminhoCapas);
                
                var caminhoCompleto = Path.Combine(caminhoCapas, nomeArquivo);

                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                CapaUrl = $"capas/{nomeArquivo}";
            }

            var album = new Album
            {
                Nome = albumDto.Nome,
                QuantidadeFaixas = albumDto.QuantidadeFaixas,
                AlbumDate = albumDto.AlbumDate,
                FaixaMaisPopular = albumDto.FaixaMaisPopular,
                CapaUrl = CapaUrl
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

            var albumCriadoDto = new AlbumDto
            {
                Id = album.Id,
                Nome = album.Nome,
                AlbumDate = album.AlbumDate,
                QuantidadeFaixas = album.QuantidadeFaixas,
                FaixaMaisPopular = album.FaixaMaisPopular,
                CapaUrl = album.CapaUrl,
                ArtistaIds = albumDto.ArtistaIds,
                ArtistaParticipacoesIds = albumDto.ArtistaParticipacoesIds
            };

            return CreatedAtAction(nameof(GetAlbum), new { id = album.Id }, albumCriadoDto);
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

        [HttpGet("search")]
        public async Task<ActionResult<List<AlbumDto>>> BuscarPorNome([FromQuery] string nome)
        {
            var resultado = await _context.Albuns
                .Where(a => a.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase))
                .Select(a => new AlbumDto
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    QuantidadeFaixas = a.QuantidadeFaixas,
                    AlbumDate = a.AlbumDate,
                    FaixaMaisPopular = a.FaixaMaisPopular,
                    ArtistaIds = a.AlbumArtistas.Select(aa => aa.ArtistId).ToList(),
                    ArtistaParticipacoesIds = a.Participacoes.Select(ap => ap.ArtistaId).ToList(),
                    CapaUrl = a.CapaUrl
                })
                .ToListAsync();

            return Ok(resultado);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlbum(int id, AlbumDto albumDto)
        {
            if (albumDto == null)
            {
                return BadRequest("O corpo da requisição não pode estar vazio.");
            }

            var albumExistente = await _context.Albuns
                .Include(a => a.AlbumArtistas)
                .Include(a => a.Participacoes)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (albumExistente == null)
            {
                return NotFound();
            }

            // Atualiza campos básicos
            albumExistente.Nome = albumDto.Nome;
            albumExistente.QuantidadeFaixas = albumDto.QuantidadeFaixas;
            albumExistente.AlbumDate = albumDto.AlbumDate;
            albumExistente.FaixaMaisPopular = albumDto.FaixaMaisPopular;
            albumExistente.CapaUrl = albumDto.CapaUrl;

            // Atualiza relacionamentos com artistas

            // Remove os antigos
            _context.AlbumArtistas.RemoveRange(albumExistente.AlbumArtistas);
            _context.AlbumParticipacoes.RemoveRange(albumExistente.Participacoes);

            // Verifica se os novos artistas existem
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

            // Recria os relacionamentos com os artistas
            var novosAlbumArtistas = artistas
            .Where(a => albumDto.ArtistaIds.Contains(a.Id))
            .Select(artista => new AlbumArtista
            {
                AlbumId = albumExistente.Id,
                Album = albumExistente,
                ArtistId = artista.Id,
                Artista = artista
            }).ToList();

        var novasParticipacoes = artistas
            .Where(a => albumDto.ArtistaParticipacoesIds.Contains(a.Id))
            .Select(artista => new AlbumParticipacoes
            {
                AlbumId = albumExistente.Id,
                Album = albumExistente,
                ArtistaId = artista.Id,
                Artista = artista
            }).ToList();

            _context.AlbumArtistas.AddRange(novosAlbumArtistas);
            _context.AlbumParticipacoes.AddRange(novasParticipacoes);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadImagem(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Nenhum arquivo enviado.");

            var nomeArquivo = Path.GetFileName(file.FileName);
            var pastaDestino = Path.Combine(_env.WebRootPath, "img", "capas");

            if (!Directory.Exists(pastaDestino))
                Directory.CreateDirectory(pastaDestino);

            var caminhoCompleto = Path.Combine(pastaDestino, nomeArquivo);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var urlImagem = $"img/capas/{nomeArquivo}";
            return Ok(urlImagem);
        }          


    }
}
