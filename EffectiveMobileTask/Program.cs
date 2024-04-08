using EffectiveMobileTask;
using System.Net;
using System.Text.Json;

string? ip;
long ipEnd = 0;
long ipStart = 0;
string? timeEnd;
string? timeStart;
string? pathToLogFile;
string? pathToOutputFile;
string? addressMask = null;
string? addressStart = null;
int formatedAddressMask = int.MaxValue;

void WritingOutputFile()
{
    Console.WriteLine("Please insert parameters.");

    Console.Write("Path to log file = ");
    pathToLogFile = Console.ReadLine();

    Console.Write("Path to output file = ");
    pathToOutputFile = Console.ReadLine();

    while (true)
    {
        Console.Write("Bottom edge of address = ");
        addressStart = Console.ReadLine();
        if (String.IsNullOrWhiteSpace(addressStart)) break;

        string[] splitValues = addressStart.Split('.');
        if (splitValues.Length != 4)
        {
            Console.WriteLine("Invalid ip address");
            continue;
        }

        byte tempForParsing;

        if (splitValues.All(r => byte.TryParse(r, out tempForParsing))) break;
    }

    while (formatedAddressMask > 32 || formatedAddressMask < 0)
    {
        Console.Write("Address mask = ");
        addressMask = Console.ReadLine();

        if (String.IsNullOrEmpty(addressMask)) break;
        formatedAddressMask = int.Parse(addressMask);
    }

    Console.Write("Start of the time interval = ");
    timeStart = Console.ReadLine();

    Console.Write("End of the time interval = ");
    timeEnd = Console.ReadLine();


    if (!File.Exists(pathToLogFile)) throw new ArgumentException("File does not exist");

    if (!String.IsNullOrEmpty(addressStart))
    {
        string url = $"https://networkcalc.com/api/ip/{addressStart + "/" + addressMask}";

        using (WebClient client = new WebClient())
        {
            string json = client.DownloadString(url);
            var data = JsonSerializer.Deserialize<JsonAddresses>(json);
            ip = data.CalculatedAssignableHost.Last_assignable_host;
        }
        ipEnd = ConvertToIp(ip);
        ipStart = ConvertToIp(addressStart);
        
    }

    var logFileValues = File.ReadAllLines(pathToLogFile);
    Dictionary<string, int> outputFileValues = new Dictionary<string, int>();
    for (int i = 0; i < logFileValues.Length; i++)
    {
        string[] logFileUnit = logFileValues[i].Split(new char[] {' ',':' } ,3);
        var time = DateOnly.Parse(logFileUnit[1]);

        DateOnly formatedStartTime;
        var startTime = DateOnly.TryParseExact(timeStart, "dd.MM.yyyy", out formatedStartTime);

        DateOnly formatedEndTime;
        var endTime = DateOnly.TryParseExact(timeEnd, "dd.MM.yyyy", out formatedEndTime);

        if (!(formatedStartTime < time) || !(time < formatedEndTime)) continue;

        if (!String.IsNullOrEmpty(addressStart))
        {
            if (!IsInRange(logFileUnit[0])) continue;
        }

        if (outputFileValues.TryGetValue(logFileUnit[0], out int some))
        {
            outputFileValues[logFileUnit[0]] = ++some;
        }
        else
        {
            outputFileValues.Add(logFileUnit[0], 1);
        }
    }

    foreach(var outputFileValue in outputFileValues)
    {
        File.AppendAllText(@"C:\\Projects\\EffectiveMobileTask\\EffectiveMobileTask\\FileOutput.txt", outputFileValue.Key + " count = " + outputFileValue.Value + Environment.NewLine);
    }
}

long ConvertToIp(string ip)
{
    return BitConverter.ToInt32(IPAddress.Parse(ip).GetAddressBytes().Reverse().ToArray(), 0);
}

bool IsInRange(string address)
{
    long ip = ConvertToIp(address);
    return ip >= ipStart && ip <= ipEnd;
}

WritingOutputFile();