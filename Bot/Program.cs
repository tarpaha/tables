namespace Bot;

using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using Args;
using Solver;
using Solver.Result;

public static class Program
{
    private const int MaxWorkDurationMs = 5000;
    private const string TelegramBotTokenVariable = "TELEGRAM_BOT_TOKEN";
    
    public static async Task Main()
    {
        var botToken = Environment.GetEnvironmentVariable(TelegramBotTokenVariable);
        if (string.IsNullOrEmpty(botToken))
        {
            await Console.Error.WriteLineAsync($"{TelegramBotTokenVariable} is not set in the environment");
            return;
        }
        
        var botClient = new TelegramBotClient(botToken);
        
        using var cts = new CancellationTokenSource();
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = new[]
            {
                UpdateType.Message,
                UpdateType.EditedMessage
            }
        };
        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );
        
        var me = await botClient.GetMeAsync(cts.Token);
        Console.WriteLine($"Start listening for @{me.Username}");
        
        await Task.Delay(Timeout.Infinite, cts.Token);
    }
    
    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        var message = update.Message ?? update.EditedMessage;
        if (message?.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;
        
        try
        {
            var args = messageText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var (players, rounds, prefer4) = Parser.Parse(args);
            
            using var solverCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            solverCts.CancelAfter(MaxWorkDurationMs);
            var result = await Solver.Solve(players, rounds, prefer4, solverCts.Token);

            await botClient.SendTextMessageAsync(
                chatId, ResultMessage(result, rounds),
                replyToMessageId: message.MessageId, cancellationToken: ct);
        }
        catch (ArgumentException ex)
        {
            await botClient.SendTextMessageAsync(chatId, HelpText(), cancellationToken: ct);
            await botClient.SendTextMessageAsync(chatId, $"Error: {ex.Message}", cancellationToken: ct);
        }
        catch (OperationCanceledException)
        {
            await botClient.SendTextMessageAsync(
                chatId, $"Error: No solution found after {MaxWorkDurationMs}ms",
                replyToMessageId: message.MessageId, cancellationToken: ct);
        }
    }

    private static string ResultMessage(Result result, int rounds)
    {
        var sb = new StringBuilder();
        foreach (var round in result.Rounds)
        {
            sb.AppendLine($"Round: {round.Id}");
            foreach (var table in round.Tables)
            {
                var players = string.Join(", ", table.Players.Select(p => p.Id.ToString("D2")));
                sb.AppendLine($"{table.Id:D2} - {players}");
            }
            sb.AppendLine();
        }
        if(rounds > result.Rounds.Count())
            sb.AppendLine($"Error! Cannot find solution for {rounds} rounds");
        return sb.ToString();
    }

    private static string HelpText()
    {
        return "The bot seats players at tables to play several rounds, neighbors are always new.\n" +
               "Message format: [players] [rounds] [3/4]\n" +
               "    [players] - total number of players\n" +
               "    [rounds]  - how much rounds to play\n" +
               "    [3/4]     - preferred table size, can be only 3 or 4\n" +
               "Example:\n" +
               "    37 3 4\n";
    }

    private static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };
        Console.Error.WriteLine(errorMessage);
        return Task.CompletedTask;
    }    
}