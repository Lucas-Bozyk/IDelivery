using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IDelivery.Tests.TestInfrastructure;

public class TestApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dbDir = Path.Combine(Path.GetTempPath(), "idelivery-tests", Guid.NewGuid().ToString("N"));
    public TestApiFactory()
    {
        Environment.SetEnvironmentVariable("Jwt__Issuer", "IDelivery.Tests");
        Environment.SetEnvironmentVariable("Jwt__Audience", "IDelivery.Tests.Client");
        Environment.SetEnvironmentVariable("Jwt__Key", "THIS_IS_A_32_PLUS_CHAR_TEST_ONLY_SECRET_KEY_123");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Directory.CreateDirectory(_dbDir);
        builder.UseEnvironment("Testing");
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var cfg = new Dictionary<string, string?>
            {
                ["ConnectionStrings:IdentityDb"] = $"Data Source={Path.Combine(_dbDir, "identity.test.db")}",
                ["ConnectionStrings:DeliveryDb"] = $"Data Source={Path.Combine(_dbDir, "delivery.test.db")}",
                ["Jwt:Issuer"] = "IDelivery.Tests",
                ["Jwt:Audience"] = "IDelivery.Tests.Client",
                ["Jwt:Key"] = "THIS_IS_A_32_PLUS_CHAR_TEST_ONLY_SECRET_KEY_123"
            };
            config.AddInMemoryCollection(cfg);
        });
    }
}
