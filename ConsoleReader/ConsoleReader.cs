using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleReader
{
	public class ConsoleReader
	{
		/// <summary>
		/// Default countDownMessage message
		/// </summary>
		/// <param name="countDownSeconds">seconds left</param>
		/// <returns>message to print</returns>
		public static string CountDownMessageDefault(uint countDownSeconds)
		{
			return string.Format("Time left {0} seconds ", countDownSeconds);
		}

		/// <summary>
		/// Reads a single key input 
		/// </summary>
		/// <param name="timeoutSeconds">user input timeout</param>
		/// <param name="countDownMessage">message to print on countdown</param>
		/// <param name="samplingFrequencyMilliseconds">frequecy of countdown event</param>
		/// <returns>input key</returns>
		public static ConsoleKeyInfo ReadKey(uint timeoutSeconds, Func<uint, string> countDownMessage = null, uint samplingFrequencyMilliseconds = 1000)
		{
			return ShowDialogInternal(() => Console.ReadKey(), timeoutSeconds, countDownMessage, samplingFrequencyMilliseconds);
		}

		/// <summary>
		/// Reads a string line input 
		/// </summary>
		/// <param name="timeoutSeconds">user input timeout</param>
		/// <param name="countDownMessage">message to print on countdown</param>
		/// <param name="samplingFrequencyMilliseconds">frequecy of countdown event</param>
		/// <returns>string line</returns>
		public static string ReadLine(uint timeoutSeconds, Func<uint, string> countDownMessage = null, uint samplingFrequencyMilliseconds = 1000)
		{
			return ShowDialogInternal(() => Console.ReadLine(), timeoutSeconds, countDownMessage, samplingFrequencyMilliseconds);
		}

		private static T ShowDialogInternal<T>(Func<T> consoleRead, uint timeoutSeconds, Func<uint, string> countDownMessage, uint samplingFrequencyMilliseconds)
		{
			if (timeoutSeconds == 0)
				return default(T);

			var timeoutMilliseconds = timeoutSeconds * 1000;

			if (samplingFrequencyMilliseconds > timeoutMilliseconds)
				throw new ArgumentException("Sampling frequency must not be greater then timeout!", "samplingFrequencyMilliseconds");

			CancellationTokenSource cts = new CancellationTokenSource();

			Task.Factory
				.StartNew(() => SpinUserDialog(timeoutMilliseconds, countDownMessage, samplingFrequencyMilliseconds, cts.Token), cts.Token)
				.ContinueWith(t => {
					var hWnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
					PostMessage(hWnd, 0x100, 0x0D, 9);
				}, TaskContinuationOptions.NotOnCanceled);


			T inputLine = consoleRead();
			cts.Cancel();

			return inputLine;
		}


		private static void SpinUserDialog(uint countDownMilliseconds, Func<uint, string> countDownMessage, uint samplingFrequencyMilliseconds,
			CancellationToken token)
		{
			while (countDownMilliseconds > 0)
			{
				token.ThrowIfCancellationRequested();

				if (countDownMessage != null)
					Console.Write("\r" + countDownMessage(countDownMilliseconds / 1000));

				Thread.Sleep((int)samplingFrequencyMilliseconds);

				countDownMilliseconds -= countDownMilliseconds > samplingFrequencyMilliseconds
					? samplingFrequencyMilliseconds
					: countDownMilliseconds;
			}
		}


		[DllImport("User32.Dll", EntryPoint = "PostMessageA")]
		private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
	}
}
