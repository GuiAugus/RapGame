using Microsoft.AspNetCore.Mvc;
using RapGame.Data;
using RapGame.Models;
using Microsoft.EntityFrameworkCore;

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
    public async Task<ActionResult<IEnumerable<Artista>>> GetArtistas()
    {
        return await _context.Artistas.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Artista>> GetArtista(int id)
    {
        var artista = await _context.Artistas.FindAsync(id);
        if (artista == null)
        {
            return NotFound();
        }
        return artista;
    }

    [HttpPost]
    public async Task<ActionResult<Artista>> PostArtista(Artista artista)
    {
        _context.Artistas.Add(artista);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetArtista), new { id = artista.Id }, artista);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArtista(int id)
    {
        var artista = await _context.Artistas.FindAsync(id);
        if (artista == null)
        {
            return NotFound();
        }

        _context.Artistas.Remove(artista);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}