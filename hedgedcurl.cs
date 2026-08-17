class HedgedCurl(ILogger<HedgedCurl> logger)
{
    internal class Options {
        public bool ShowHelp { get; set; } = false;
        public List<Uri> Urls { get; set; } = [];
        public int TimeOutSeconds { get; set; } = DefaultTimeoutSeconds;
    }

    public class ReturnValue {
        public int ReturnValueCode { get; set; }
        public string Stdout { get; set; } = string.Empty;
        public string Stderr { get; set; } = string.Empty;

        public int InputToConsole()
        {
            Console.WriteLine(Stdout);
            Console.Error.WriteLine(Stderr);
            return ReturnValueCode;
        }
    }
    public static ReturnValue Success(string stdout)
    {
        return new ReturnValue
        {
            ReturnValueCode = 0,
            Stdout = stdout,
            Stderr = string.Empty,
        };
    }
    public static ReturnValue Fail(int code, string stderr)
    {
        return new ReturnValue
        {
            ReturnValueCode = code,
            Stdout = string.Empty,
            Stderr = stderr,
        };
    }

    private const int SuccessExitCode = 0;
    private const int TimeoutExitCode = 228;
    private const int GeneralErrorExitCode = 2;
    private const int DefaultTimeoutSeconds = 15;

    private readonly ILogger<HedgedCurl> _logger = logger;

    public async Task<int> RunHedgedCurl(string[] args) {
        try
        {
            var options = ParseArgs(args);
            if (options.ShowHelp)
            {
                Console.WriteLine(ShowHelp());
                return SuccessExitCode;
            }
            var result = await ExecuteHedgedRequest(options);
            var exitCode = result.InputToConsole();
            return exitCode;
        } catch (ArgumentException e)
        {
            Console.Error.WriteLine(e.Message);
            return GeneralErrorExitCode;
        }
    }

    private static string ShowHelp()
    {
        string helpText = """
            Usage: hedgedcurl [OPTIONS] URL1 URL2 ...\n
            Options:\n
              -t, --timeout SECONDS -- Set timeout for all HTTP requests in seconds (Default: 15 seconds)\n
              -h, --help -- Show this help message
        """;
        return helpText;
    }

    private Options ParseArgs(string[] args) {
        Options options = new();

        for (int i = 0; i < args.Length; ++i) {
            string arg = args[i].ToLower();

            if (arg == "-h" || arg == "--help") {
                options.ShowHelp = true;
                return options;
            }

            if (arg == "-t" || arg == "--timeout") {
                if (i + 1 >= args.Length || !int.TryParse(args[++i], out int timeout))
                {
                    throw new ArgumentException("Timeout has no value.");
                }
                if (timeout <= 0)
                {
                    throw new ArgumentException("Timeout cannot be lower or equal to zero.");
                }
                options.TimeOutSeconds = timeout;
                continue;
            }

            if (!Uri.TryCreate(arg, UriKind.Absolute, out Uri? uri)
                || uri == null)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("{Arg} is not valid URL -- skipping.", arg);
                continue;
            }

            if (uri.Scheme != Uri.UriSchemeHttp
                && uri.Scheme != Uri.UriSchemeHttps)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("{Arg} is non-HTTP/HTTPS URL scheme (scheme: {Scheme}) -- skipping.", arg, uri.Scheme);
                continue;
            }

            options.Urls.Add(uri);
        }

        if (options.Urls.Count == 0)
        {
            throw new ArgumentException("At least one Valid Uri is needed.");
        }

        return options;
    }

    private async Task<ReturnValue> ExecuteHedgedRequest(Options options)
    {
        using var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(options.TimeOutSeconds)
        };
        using var cts = new CancellationTokenSource();

        var tasks = options.Urls.Select(url => SendRequestAsync(client, url, cts.Token)).ToList();
        while(tasks.Count > 0) {
            try
            {
                var completedTask = await Task.WhenAny(tasks);
                tasks.Remove(completedTask);
                var response = await completedTask;
                cts.Cancel();
                var stdout = await BuildResponseStringAsync(response);
                response.Dispose();
                return Success(stdout);
            }
            catch(OperationCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                return Fail(TimeoutExitCode, "Processes timed out");
            }
            catch (Exception e)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("Url threw an Exception -- {Arg}.", e.Message);
                continue;
            }

        }

        return Fail(GeneralErrorExitCode, "All processes returned Exceptions.");   
    }

    private async Task<string> BuildResponseStringAsync(HttpResponseMessage response)
    {
        StringBuilder stdout = new();
        stdout.AppendLine($"HTTP/1.1 {(int)response.StatusCode} {response.StatusCode.ToString()}");
        stdout.AppendLine("--- Headers ---");
        foreach (var header in response.Headers)
        {
            stdout.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
        }
        foreach (var header in response.Content.Headers)
        {
            stdout.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
        }
        stdout.AppendLine("--- Body ---");
        stdout.Append(await response.Content.ReadAsStringAsync());
        return stdout.ToString();
    }

    private async Task<HttpResponseMessage> SendRequestAsync(HttpClient client, Uri url, CancellationToken token)
    {
        return await client.GetAsync(url.ToString(), HttpCompletionOption.ResponseHeadersRead, token);
    }
}

class Program
{
    static async Task<int> Main(string[] args)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder =>
        {
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        ILogger<HedgedCurl> logger = factory.CreateLogger<HedgedCurl>();
        HedgedCurl app = new(logger);

        return await app.RunHedgedCurl(args);
    }
}