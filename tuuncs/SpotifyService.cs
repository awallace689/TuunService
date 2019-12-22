/*using System;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using SpotifyAPI.Web.Auth;
using Secrets;
public sealed class SpotifyApiWrapper
{
	private string clientID = Secret._clientID;
	private string clientSecret = Secret._clientSecret;

	public TokenSwapWebAPIFactory webApiFactory;
	public SpotifyWebAPI spotify;
	//reference this for access to singleton service
	public static readonly SpotifyApiWrapper spotifyService = new SpotifyApiWrapper();

	public Exception authException;

	static SpotifyApiWrapper()
	{
	}

	//https://johnnycrazy.github.io/SpotifyAPI-NET/auth/token_swap.html#using-tokenswapwebapifactory
	public static async void initialize()
	{
		// You should store a reference to WebAPIFactory if you are using AutoRefresh or want to manually refresh it later on. New WebAPIFactory objects cannot refresh SpotifyWebAPI object that they did not give to you.
		spotifyService.webApiFactory = new TokenSwapWebAPIFactory("https://localhost:44382/Tuun")
		{
			Scope = Scope.UserReadPrivate | Scope.UserReadEmail | Scope.PlaylistReadPrivate,
			AutoRefresh = true
		};
		// You may want to react to being able to use the Spotify service.
		// webApiFactory.OnAuthSuccess += (sender, e) => authorized = true;
		// You may want to react to your user's access expiring.
		// webApiFactory.OnAccessTokenExpired += (sender, e) => authorized = false;

		try
		{
			spotifyService.spotify = await spotifyService.webApiFactory.GetWebApiAsync();
			// Synchronous way:
			// spotify = webApiFactory.GetWebApiAsync().Result;
		}
		catch (Exception ex)
		{
			spotifyService.authException = ex;
		}
	}
	public static SpotifyApiWrapper Instance
    {
		get
		{
			return spotifyService;
		}

		private set 
		{
		}
    }
}
*/