using Microsoft.AspNetCore.Mvc;
using RapGame.Data;
using RapGame.Models;
using Microsoft.EntityFrameworkCore;
using RapGame.Shared.DTOs;

[Route("api/[controller]")]
[ApiController]
public class ArtistaController : ControllerBase
{
    private readonly RapGameDbContext _context;

    public ArtistaController(RapGameDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArtistaDto>>> GetArtistas()
    {
        var artistas = await _context.Artistas
            .Include(a => a.AlbumArtistas) 
            .Select(artista => new ArtistaDto
            {
                Id = artista.Id,
                Nome = artista.Nome
            })
            .ToListAsync();

        return Ok(artistas);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ArtistaDto>> GetArtista(int id)
    {
        var artista = await _context.Artistas
            .Include(a => a.AlbumArtistas)
            .ThenInclude(aa => aa.Album) // Se precisar incluir os álbuns
            .FirstOrDefaultAsync(a => a.Id == id);

        if (artista == null)
            return NotFound();

        var artistaDto = new ArtistaDto
        {
            Id = artista.Id,
            Nome = artista.Nome
        };

        return Ok(artistaDto);
    }

    [HttpPost]
    public async Task<ActionResult<Artista>> PostArtista(ArtistaDto artistaDto)
    {
        var artista = new Artista
        {
            Nome = artistaDto.Nome
        };

        _context.Artistas.Add(artista);        
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetArtista), new { id = artista.Id }, artista);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArtista(int id)
    {
        var artista = await _context.Artistas
            .Include(a => a.AlbumArtistas)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (artista == null)
            return NotFound();

        _context.Artistas.Remove(artista);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}