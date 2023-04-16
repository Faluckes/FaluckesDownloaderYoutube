using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Log/FaluckesLogger.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Debug().CreateLogger();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(
        name: "selectedFolder",
        pattern: "{controller=Home}/{action=SelectedFolder}/{folderPath?}");
});

app.UseStaticFiles();
app.UseDirectoryBrowser();

app.Run();


Log.Information("Iniciando site...");
