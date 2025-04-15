using Blazored.LocalStorage.StorageOptions;
using RapGame.Client.SharedLogic;
using RapGame.Shared.DTOs;
using System.Net.Http.Json;

namespace RapGame.Client.Pages
{
    public partial class Rapdle : BasePage
    {             
        protected override async Task OnInitializedAsync()
        {
            try
            {
                albuns = await Http.GetFromJsonAsync<List<AlbumDto>>("api/Album") ?? new();
                
                var idDoAlbum = await localStorage.GetItemAsync<int>("rapdle_album_id");
                await BuscarAlbumPorId(idDoAlbum);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar álbuns: {ex.Message}");
                albuns = new();
            }
        }

        

        protected int ObterAno(string? dataFormatada)
        {
            if (string.IsNullOrWhiteSpace(dataFormatada))
                return 0;

            return DateTime.TryParseExact(dataFormatada, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out var data)
                ? data.Year
                : 0;        }


        private string ObterClasseCor(bool condicao)
            => condicao ? "bg-success text-white" : "bg-danger text-white";
    }
}
