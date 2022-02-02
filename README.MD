
## NamedPipes

Use can use classes provided by `NamedPipes.dll` to create pipe server- and client for cross process communication on same machine.
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

You can use command line test application to play around with the pipes.<br>
Launch from command line using `test.exe [type] [pipe-name]`

    test.exe server mypipe
    test.exe client mypipe

After that just type a `string` you want to send or "special keyword" `example` to send an instance of `ExampleMessage` with randomized data.
Message is deserialized before sending and deserialized again when received on another end.

You can also send messages to the created pipe from console itself

`C:\WINDOWS\system32>echo something_i_want_to_send > \\.\pipe\mypipe`

![test command line application](/git_images/pipes.gif?raw=true)
