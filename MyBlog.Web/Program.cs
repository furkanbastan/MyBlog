using MyBlog.Service;
using MyBlog.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddServiceLayerServices();

builder.Services.AddWebLayerServices();

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//
app.UseNToastNotify();

app.UseHttpsRedirection();
app.UseStaticFiles();
//app.UseSession();
app.UseRouting();

app.UseAuthorization();

/*app.MapControllerRoute(    şununla aynı => endpoints.MapDefaultControllerRoute();
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");*/

app.UseEndpoints(endpoints => 
{
    endpoints.MapAreaControllerRoute(
        name : "Admin",
        areaName : "Admin",
        pattern : "Admin/{controller=Home}/{action=Index}/{id?}"
    );
    endpoints.MapDefaultControllerRoute();
});

app.Run();
