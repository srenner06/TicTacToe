namespace TicTacToe.Win.Helpers;

internal class CacheHelper : Utils.Helpers.CacheHelper
{
	private CacheHelper() { }
	private static readonly Lazy<CacheHelper> _instance = new Lazy<CacheHelper>(() => new CacheHelper());
	public static CacheHelper Instance => _instance.Value;
	public string SettingsPath
		=> Path.Combine(GetAppDataFolder(), "settings.json");
}
