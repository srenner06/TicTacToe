namespace TikTakToe.Overlay;

public interface IOverlay
{
	void SetText(string text);
	void Start();
	void End();

}
internal class NoOverlay : IOverlay
{
	public void SetText(string text)
	{
	}

	public void End()
	{
	}
	public void Start()
	{
	}

}
