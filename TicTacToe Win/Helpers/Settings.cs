using Utils.Helpers;
namespace TicTacToe.Win.Helpers;
internal record Settings
{
	public Color P1Color { get; set; } = Color.Blue;
	public Color P2Color { get; set; } = Color.Red;
	public Color NoPColor { get; set; } = Color.WhiteSmoke;
	public int[] CustomColorsDefined { get; set; } = [];

	public static Settings Load()
	{
		var settings = CacheHelper.Instance.SettingsPath.LoadFromFile<Settings>();

		if (settings is null)
		{
			var val = new Settings();
			val.Save();
			return val;
		}
		return settings;
	}
	public void Save()
	{
		this.SaveToFile(CacheHelper.Instance.SettingsPath);
	}
}
