using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using RapGame.Shared.DTOs;
using Microsoft.AspNetCore.Components.Web;


namespace RapGame.Client.SharedLogic
{
    public class BasePage : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; } = default!;
        protected string _tentativa = "";
        public string Tentativa
        {
            get => _tentativa;
            set
            {
                _tentativa = value;
                BuscarAlbunsSugestao(value);
            }
        }
        protected List<AlbumDto> albuns = new();
        public List<AlbumDto> sugestoes = new();
        public List<AlbumDto> tentativas {get; set; } = new();
        protected AlbumDto? albumSelecionado;
        public bool jogoEncerrado {get; set; } = false;
        public string DicaFeat { get; set; } = "";
        public string DicaFaixaFamosa { get; set; } = "";
        public string? Mensagem { get; set; }
        public List<AlbumDto> TentativasComInfo { get; set; } = new();

        protected void BuscarAlbunsSugestao(string texto)
        {
            if (!string.IsNullOrWhiteSpace(texto))
            {
                var nomesChutados = tentativas.Select(t => t.Nome.ToLower()).ToHashSet();

                sugestoes = albuns
                    .Where(a => a.Nome.Contains(texto, StringComparison.OrdinalIgnoreCase)
                        && !nomesChutados.Contains(a.Nome.ToLower()))
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

        public async Task SelecionarSugestao(AlbumDto sugestao)
        {
            Tentativa = sugestao.Nome;
            sugestoes.Clear();
            TentativasComInfo.Insert(0, sugestao);
            await VerificarTentativa(sugestao);
        }

        public async Task VerificarTentativa(AlbumDto tentativa)
        {
            if (albumSelecionado != null && tentativa.Nome.Equals(albumSelecionado.Nome, StringComparison.OrdinalIgnoreCase))
            {
                Mensagem = "Você acertou!";
                jogoEncerrado = true;
            }

            if (!tentativas.Any(a => a.Nome.Equals(tentativa.Nome, StringComparison.OrdinalIgnoreCase)))
            {
                tentativas.Add(tentativa);
            }
            if (tentativas.Count == 2 && string.IsNullOrEmpty(DicaFeat))
            {
                DicaFeat = $"Dica:  {$"Esse album possui um feat de: {albumSelecionado?.ArtistaParticipacoes?.FirstOrDefault()}" ?? "Esse album nao possui feat."}";
            }

            if (tentativas.Count == 4 && string.IsNullOrEmpty(DicaFaixaFamosa))
            {
                DicaFaixaFamosa = $"Dica: {$"Faixa mais famosa do Album: {albumSelecionado?.FaixaMaisPopular}" ?? "Desconhecida"}";
            }

            BuscarAlbunsSugestao(Tentativa);

            Tentativa = "";
            await InvokeAsync(StateHasChanged);
        }

        public async Task HandleKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                if (sugestoes.Any())
                {
                    var primeiraSugestao = sugestoes.First();
                    await SelecionarSugestao(primeiraSugestao);
                }
            }
        }
    }
}
