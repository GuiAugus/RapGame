using RapGame.Client.SharedLogic;
using RapGame.Shared.DTOs;
using System.Net.Http.Json;

namespace RapGame.Client.Pages
{
    public partial class Rapdle : BasePage
    {
        private string _tentativa = "";
        private string Tentativa
        {
            get => _tentativa;
            set
            {
                _tentativa = value;
                BuscarAlbunsSugestao(value);
            }
        }

        private string? Mensagem { get; set; }
        private List<AlbumDto> TentativasComInfo { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                albuns = await Http.GetFromJsonAsync<List<AlbumDto>>("api/Album") ?? new();
                await BuscarAlbumPorId(12); // Ou aleatório depois
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar álbuns: {ex.Message}");
                albuns = new();
            }
        }

        private async Task SelecionarSugestao(AlbumDto sugestao)
        {
            Tentativa = sugestao.Nome;
            sugestoes.Clear();
            TentativasComInfo.Insert(0, sugestao);
            await VerificarTentativa(sugestao);
        }

        private async Task VerificarTentativa(AlbumDto tentativa)
        {
            if (albumSelecionado != null && tentativa.Nome.Equals(albumSelecionado.Nome, StringComparison.OrdinalIgnoreCase))
            {
                Mensagem = "Você acertou!";
            }
            else
            {
                Mensagem = $"Tente novamente. Você digitou \"{tentativa.Nome}\"";
            }

            Tentativa = "";
            await InvokeAsync(StateHasChanged);
        }

        private string ObterClasseCor(bool condicao)
            => condicao ? "bg-success text-white" : "bg-danger text-white";
    }
}
