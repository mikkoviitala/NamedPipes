
## NamedPipes

Use can use classes provided by `NamedPipes.dll` to create pipe server- and client for cross process communication on the same machine.<br>
Both types implements common interface for ease of use.

## Features
- Connection state detection
- Events when connection state changes or when message is sent or received
- You can send any `string` or any instance of an class derived from `MessageBase` (sample included)
- Messaging is two-way and asynchronous

## Basic usage
    IPipe server = new PipeServer("MyPipe);
    IPipe client = new PipeClient("MyPipe);

    server.OnMessageReceived += (sender, args) => Console.WriteLine(args.Message);
    client.Send("Hello");

## Console test application

You can use command line test application to play around with the pipes.<br>
Launch from command line using `test.exe [type] [pipe-name]`

    test.exe server mypipe
    test.exe client mypipe

You can chage connection state by using `open` and `close` keywords.

After connection is established, just type a `string` you want to send or `example` to send an instance of `ExampleMessage` with randomized data.
Message is deserialized before sending and deserialized again when received on another end.

You can also send messages to the pipe using console alone

`C:\WINDOWS\system32>echo something_i_want_to_send > \\.\pipe\mypipe`

![test command line application](/git_images/pipes.gif?raw=true)
