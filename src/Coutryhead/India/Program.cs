namespace cInApi;

public class Program
{
    public static int Main(string[] args)
    {


        try
        {
            CreateHostBuilder(args).Build().Run();
            return 0;
        }
        catch
        {
            return 1;
        }
        finally
        {
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}


