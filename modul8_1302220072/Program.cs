using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        TransferData defaultData = new TransferData();

        string transferEN = "Please insert the amount of money to transfer";
        string transferID = "Masukkan jumlah uang yang akan di-transfer: ";
        string methodEN = "Select transfer method:";
        string methodID = "Pilih metode transfer:";
        string confirmEN = "Please type yes to confirm the transaction: ";
        string confirmID = "Ketik ya untuk mengkonfirmasi transaksi: ";
        string cancelEN = "Transfer is cancelled";
        string cancelID = "Transfer dibatalkan";

        if(defaultData.bankConfig.lang == "en")
        {
            Console.WriteLine(transferEN);
        }

        else
        {
            Console.WriteLine(transferID);
        }

        long money = Convert.ToInt64(Console.ReadLine());
        int cost;

        if(money < defaultData.bankConfig.transfer.threshold)
        {
            cost = defaultData.bankConfig.transfer.low_fee;
        }
        else
        {
            cost = defaultData.bankConfig.transfer.high_fee;
        }

        if (defaultData.bankConfig.lang == "en")
        {
            Console.WriteLine(methodEN);
        }


        else
        {
            Console.WriteLine(methodID);
        }

        Console.WriteLine("1. RTO");
        Console.WriteLine("2. SKN");
        Console.WriteLine("3. RTGS");
        Console.WriteLine("4. BI FAST");
        int tMethod = Convert.ToInt32(Console.ReadLine());

        if (tMethod < 1 || tMethod >= 5)
        {
            throw new ArgumentException();
        }


    }
}

// JSON Data
public class BankTransferConfig
{
    public string lang { get; set; }
    public transfer transfer { get; set; }
    public List<string> methods { get; set; }
    public confirmation confirmation { get; set; }

    public BankTransferConfig() { } // kosong sebagai constructor serealisasi
    public BankTransferConfig(string lang, transfer transfer, List<string> methods, confirmation confirmation)
    {
        this.lang = lang;
        this.transfer = transfer;
        this.methods = methods;
        this.confirmation = confirmation;
    }
}

public class transfer
{
    public int threshold { get; set; }
    public int low_fee { get; set; }
    public int high_fee { get; set; }

    public transfer(int threshold, int low_fee, int high_fee)
    {
        this.threshold = threshold;
        this.low_fee = low_fee;
        this.high_fee = high_fee;
    }
}

public class confirmation
{
    public string en { get; set; }
    public string id { get; set; }

    public confirmation(string en, string id)
    {
        this.en = en;
        this.id = id;
    }
}

// Defaults
public class TransferData
{
    public BankTransferConfig bankConfig;
    public const string filePath = @"transferconfig.json";

    public TransferData()
    {
        try
        {
            ReadConfig();
        }

        catch (Exception)
        {
            SetDefault();
            WriteNewConfig();
        }

    }

    private BankTransferConfig ReadConfig()
    {
        string JsonData = File.ReadAllText(filePath);
        bankConfig = JsonSerializer.Deserialize<BankTransferConfig>(JsonData);
        return bankConfig;
    }

    private void SetDefault()
    {
        string lang = "en";
        transfer trans = new transfer(25000000, 6500, 15000);
        List<string> methods = new List<string>();
        methods.Add("RTO (Real Time)");
        methods.Add("SKN");
        methods.Add("RTGS");
        methods.Add("BI FAST");
        confirmation confirm = new confirmation("yes", "ya");

        bankConfig = new BankTransferConfig(lang, trans, methods, confirm); 
    }

    public void WriteNewConfig()
    {
        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };

        string jsonString = JsonSerializer.Serialize(bankConfig, options);
        File.WriteAllText(filePath, jsonString);
    }
}