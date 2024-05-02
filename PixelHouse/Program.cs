var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Lista}");

app.MapControllerRoute(
    name: "LoginIndex",
    pattern: "{controller=Login}/{action=Index}",
    defaults: new {controller = "Login", action = "Index" });

app.MapControllerRoute(
    name: "Login",
    pattern: "{controller=Login}/{action=Login}",
    defaults: new {controller = "Login", action = "Login" });

app.MapControllerRoute(
    name: "LoginCadastrar",
    pattern: "{controller=Login}/{action=Cadastrar}",
    defaults: new {controller = "Login", action = "Cadastrar" });

app.MapControllerRoute(
    name: "LoginAlterar",
    pattern: "{controller=Login}/{action=Alterar}",
    defaults: new {controller = "Login", action = "Alterar" });

app.MapControllerRoute(
    name: "LoginLista",
    pattern: "{controller=Login}/{action=Lista}",
    defaults: new {controller = "Login", action = "Lista" });

app.Run();