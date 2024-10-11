using TicTacToe.Win.Helpers;
using TicTacToe.Win.Overlay;
using Utils.Windows.Extensions;

namespace TicTacToe.Win;
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
			if (_cts.TryReset() == false)
				_cts = new();
			Task.Run(ShowDialog, _cts.Token);
			//startThread = new Thread(() => this.ShowDialog());
			//startThread.Start();
		}
	}
}
