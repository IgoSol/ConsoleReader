# ConsoleReader
Safely adds a Timeout to Console.ReadLine() along with a countdown message

Usage:
```
// wait 5 seconds for a console input
var inputLine = ConsoleReader.ConsoleReader.ReadLine(5);

if (inputLine != null)
	Console.WriteLine(inputLine);
```

"ConsoleReaderDemo" project provides a sample user dialog implementation.