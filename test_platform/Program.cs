
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using test_platform.Configuration;
using test_platform.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Đăng ký FirebaseClientManager với DI container
builder.Services.AddSingleton(new DBService());
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddScoped<IHistoryContext, HistoryContext>();
builder.Services.AddScoped<IQuizContext, QuizContext>();
builder.Services.AddTransient<IMailService, MailService>();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));
var app = builder.Build();

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
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
