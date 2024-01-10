using TikTakToe.Helpers;
using TikTakToe.Overlay;

namespace TikTakToe;
public partial class OverlayForm : Form, IOverlay
{
	private readonly object _lock = new object(); // Used for synchronization
	private CancellationTokenSource _cts = new();

	public OverlayForm()
	{
		InitializeComponent();
	}

	public void SetText(string text)
		=> lblText.SafeInvoke(() => lblText.Text = text);

	public void End()
	{
		lock (_lock)
		{
			_cts.Cancel();
			this.SafeInvoke(() =>
			{
				if (!IsDisposed && !Disposing)
				{
					Close();
				}
			});
		}
	}

	public void Start()
	{
		lock (_lock)
		{
			_cts.Cancel();
			_cts.TryReset();
			Task.Run(ShowDialog, _cts.Token);
			//startThread = new Thread(() => this.ShowDialog());
			//startThread.Start();
		}
	}
}
