using Utils.Helpers;

namespace TicTacToe_WIn.Helpers;
internal class Settings
{
	public Color P1Color = Color.Blue;
	public Color P2Color = Color.Red;
	public Color NoPColor = Color.WhiteSmoke;

	public static Settings Load()
	{
		var settings = CacheHelper.Instance.SettingsPath.LoadFromFile<Settings>();

		if (settings is null)
		{
			new Settings().Save();
			return Load();
		}
		return settings;
	}
	public void Save()
	{
		this.SaveToFile(CacheHelper.Instance.SettingsPath);
	}
}
