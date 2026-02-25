using Microsoft.AspNetCore.Authentication.Cookies;
using RoyalVilla.DTO;
using RoyalVillaWeb.Services;
using RoyalVillaWeb.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(60);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});
builder.Services.AddAutoMapper(o =>
{
    o.CreateMap<VillaDTO, VillaCreateDTO>().ReverseMap();
    o.CreateMap<VillaDTO, VillaUpdateDTO>().ReverseMap();
    //o.CreateMap<User, UserDTO>().ReverseMap();
    //o.CreateMap<VillaAmenities, VillaAmenitiesCreateDTO>().ReverseMap();
    //o.CreateMap<VillaAmenities, VillaAmenitiesUpdateDTO>().ReverseMap();
    //o.CreateMap<VillaAmenities, VillaAmenitiesDTO>().ForMember(dest => dest.VillaName, u => u.MapFrom(src => src.Villa != null ? src.Villa.Name : null));
    //o.CreateMap<VillaAmenitiesDTO, VillaAmenities>();
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";
});
builder.Services.AddHttpClient("RoyalVillaAPI", options =>
{
   var villaAPIUrl =  builder.Configuration.GetValue<string>("ServiceUrls:VillaAPI");
    options.BaseAddress = new Uri(villaAPIUrl);
    options.DefaultRequestHeaders.Add("Accept", "application/json");    
});

builder.Services.AddScoped<IVillaService, VillaService>();
builder.Services.AddScoped<IAuthService, AuthService>();

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
