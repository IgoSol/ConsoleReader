using System;

namespace ConsoleReaderDemo
{
	class Program
	{
		public enum ConsoleDialogResult
		{
			Yes,
			No
		}

		static void Main(string[] args)
		{
			Console.WriteLine("Do you want me to proceed?");

			/*var inputLine = ConsoleReader.ConsoleReader.ReadLine(15);
			if (inputLine != null)
				Console.WriteLine(inputLine);*/

			ConsoleDialogResult result = ConsoleDialogResult.No;

			var input = ConsoleReader.ConsoleReader.ReadKey(15, (countDownSeconds) => string.Format("'{1}' will be selected after ({0}) sec. Press [Y]es [N]o: ", countDownSeconds, result));

			switch (input.KeyChar)
			{
				case 'y':
				case 'Y':
					result = ConsoleDialogResult.Yes;
					break;
				case 'n':
				case 'N':
					result = ConsoleDialogResult.No;
					break;
			}

			Console.WriteLine();
			Console.WriteLine(result);
		}
	}
}
