
## NamedPipes

Use can use classes provided by `NamedPipes.dll` to create pipe server- and client for cross thread communication.
Both types implements common interface for ease of use.

## Features
- Connection state detection
- Events when connection state changes or when message is sent or received
- You can send any `string` or any instance of an class derived fron `MessageBase` (sample included)
- Messaging is two-way and asynchronous

## Basic usage
    IPipe server = new PipeServer("MyPipe);
    IPipe client = new PipeClient("MyPipe);

    server.OnMessageReceived += (sender, args) => Console.WriteLine(args.Message);
    client.Send("Hello");

## Console test application

You can use provided command line test application to play around with the pipes.<br>
Launch from command line using `test.txt [type] [pipe-name]`

    test.exe server mypipe
    test.exe client mypipe

![test command line application](/git_images/pipes.gif?raw=true)
