// SharedLogic/BasePage.cs
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using RapGame.Shared.DTOs;

namespace RapGame.Client.SharedLogic
{
    public class BasePage : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; } = default!;

        protected List<AlbumDto> albuns = new();
        protected List<AlbumDto> sugestoes = new();
        protected AlbumDto? albumSelecionado;

        protected void BuscarAlbunsSugestao(string texto)
        {
            if (!string.IsNullOrWhiteSpace(texto))
            {
                sugestoes = albuns
                    .Where(a => a.Nome.Contains(texto, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                sugestoes.Clear();
            }
        }

        protected async Task BuscarAlbumPorId(int id)
        {
            try
            {
                var response = await Http.GetAsync($"api/album/{id}");
                if (response.IsSuccessStatusCode)
                {
                    albumSelecionado = await response.Content.ReadFromJsonAsync<AlbumDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar álbum: {ex.Message}");
            }
        }
    }
}
