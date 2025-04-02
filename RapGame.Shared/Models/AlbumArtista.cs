namespace RapGame.Models
{
    public class AlbumArtista
    {
        public int AlbumId { get; set; }
        public required Album Album { get; set; }

        public int ArtistId { get; set; }
        public required Artista Artista { get; set; }
    }
}
