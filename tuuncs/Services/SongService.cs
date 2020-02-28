using System;
namespace tuuncs.Services
{
    public class SongService
    {
        public SongService()
        {
            //# Algorithm

            //1) Grab user playlists and last 50 listened
            //1a) Group songs by artist
            //1b) Grab artists genre, assign to artist in hashtable
            //2) Remove artists and their songs not under chosen genre
            //3) Grab song analysis for results of filter.
            //4) Weight 70% songs shared by more than one user, 30% weight on all songs that pass genre
            //5) Sum and average song analysis objects considering weights.
            //6) Grab top 5 of most similar songs to "perfect song" from filtered list, pass them and perfect song values to spotify get-similar-tracks
            //7) Add mix of results from request and shared songs into room "queue".
            //8) Serve room queue to front end.


        }
    }
}
