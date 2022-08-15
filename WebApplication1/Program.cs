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
/// Ȯ�� �޼���� ���������� ���� �޼����Դϴ�. 
/// �ڵ��� �������� Ȯ���� �����ֱ� ������ .NET Core �������� �߿��� ������ �մϴ�. 
/// �ٸ� ���� �޼���� �ٸ� ���� "this"�� ù ��° �Ű� ������ �޾Ƶ��̰� "this"�� �ش� Ȯ�� �޼��带 ����ϴ� ��ü�� ������ ������ ��Ÿ���ϴ�. 
/// Ȯ�� �޼���� ���� Ŭ���� �ȿ� �־�� �մϴ�
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