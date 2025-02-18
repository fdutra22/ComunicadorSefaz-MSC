
using ComunicadorSefaz.Services;
using ComunicadorSefaz.Services.Interfaces;
using Scalar.AspNetCore;

namespace ComunicadorSefaz
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
         

            builder.Services.AddScoped<ICertificadoService, CertificadoService>();
            builder.Services.AddScoped<IAssinarXmlService, AssinarXmlService>();
            builder.Services.AddScoped<ICienciaEmissaoService, CienciaEmissaoService>();
            builder.Services.AddScoped<IComunicadorSefazService, ComunicadorSefazService>();
            builder.Services.AddScoped<IMontarXmlEnvioService, MontarXmlEnvioService>();


            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi]

            // Customise default API behaviour
            builder.Services.AddEndpointsApiExplorer();

            // Add the Open API document generation services
            builder.Services.AddOpenApi();

            var app = builder.Build();

            app.MapStaticAssets();

            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options
                    .WithTitle("TITLE_HERE")
                    .WithDownloadButton(true)
                    .WithTheme(ScalarTheme.Moon)
                    .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios);
            });


            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
