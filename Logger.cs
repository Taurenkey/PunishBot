using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PunishBot
{
    public class InternalLog(LogSeverity severity, string source, string message, Exception? exception)
    {
        public override string ToString()
        {
            return ToString(null, fullException: true, prependTimestamp: true, DateTimeKind.Local);
        }

        public string ToString(StringBuilder? builder = null, bool fullException = true, bool prependTimestamp = true, DateTimeKind timestampKind = DateTimeKind.Local, int? padSource = 11)
        {
            string source = Source;
            string message = Message;
            padSource = Math.Max(11, source?.Length ?? 0);
            string? text = (!fullException) ? Exception?.Message : Exception?.ToString();
            int capacity = 1 + (prependTimestamp ? 8 : 0) + 1 + padSource.Value + 1 + (message?.Length ?? 0) + (text?.Length ?? 0) + 3;
            if (builder == null)
            {
                builder = new StringBuilder(capacity);
            }
            else
            {
                builder.Clear();
                builder.EnsureCapacity(capacity);
            }

            if (prependTimestamp)
            {
                DateTime dateTime = ((timestampKind != DateTimeKind.Utc) ? DateTime.Now : DateTime.UtcNow);
                string text2 = "HH:mm:ss";
                builder.Append(dateTime.ToString(text2));
                builder.Append(' ');
            }

            if (source != null)
            {
                if (padSource.HasValue)
                {
                    if (source.Length < padSource.Value)
                    {
                        builder.Append(source);
                        builder.Append(' ', padSource.Value - source.Length);
                    }
                    else if (source.Length > padSource.Value)
                    {
                        builder.Append(source.AsSpan(0, padSource.Value));
                    }
                    else
                    {
                        builder.Append(source);
                    }
                }

                builder.Append(' ');
            }

            if (!string.IsNullOrEmpty(Message))
            {
                foreach (var c in from char c in message!
                                  where !char.IsControl(c)
                                  select c)
                {
                    builder.Append(c);
                }
            }

            if (text != null)
            {
                if (!string.IsNullOrEmpty(Message))
                {
                    builder.Append(':');
                    builder.AppendLine();
                }

                builder.Append(text);
            }

            return builder.ToString();
        }

        public LogSeverity Severity { get; set; } = severity;

        public string Source { get; set; } = source;

        public string Message { get; set; } = message;

        public Exception? Exception { get; set; } = exception;
    }
    public class Logger(DiscordSocketClient client)
    {
        private readonly DiscordSocketClient _client = client;

        public async Task Setup()
        {
            _client.Log += Log;
            await Task.CompletedTask;
        }

        public static Task Log(LogMessage msg)
        {
            InternalLog m = new(msg.Severity, msg.Source, msg.Message, msg.Exception);
            Log(m);
            return Task.CompletedTask;
        }

        public static Task Log(InternalLog msg)
        {
            if (msg.Exception is CommandException cmdException)
            {
                Console.WriteLine($"[Command/{msg.Severity}] {cmdException.Command.Aliases[0]}"
                    + $" failed to execute in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException);
            }
            else
                Console.WriteLine($"[General/{msg.Severity}] {msg}");
            return Task.CompletedTask;
        }

        public static Task Log(LogSeverity severity, string source, string message, Exception? exception = null)
        {
            InternalLog m = new(severity, source, message, exception);
            Log(m);
            return Task.CompletedTask;
        }
    }
}
