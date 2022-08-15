namespace WebApplication1;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.ConfigureCors();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}

/// <summary>
/// 확장 메서드는 본질적으로 정적 메서드입니다. 
/// 코드의 가독성을 확실히 높여주기 때문에 .NET Core 구성에서 중요한 역할을 합니다. 
/// 다른 정적 메서드와 다른 점은 "this"를 첫 번째 매개 변수로 받아들이고 "this"는 해당 확장 메서드를 사용하는 개체의 데이터 형식을 나타냅니다. 
/// 확장 메서드는 정적 클래스 안에 있어야 합니다
/// </summary>
public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });
    }
}