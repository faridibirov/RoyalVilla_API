using RoyalVilla.DTO;
using RoyalVillaWeb.Models;
using RoyalVillaWeb.Services.IServices;

namespace RoyalVillaWeb.Services;

public class AuthService : BaseService, IAuthService
{

    private readonly string APIEndPoint = "/api/auth";
    public AuthService(IHttpClientFactory httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        : base(httpClient, httpContextAccessor)
    {
    }

    public Task<T?> LoginAsync<T>(LoginRequestDTO loginRequestDTO)
    {
        return SendAsync<T>(new ApiRequest
        {
            ApiType = SD.ApiType.POST,
            Data = loginRequestDTO,
            Url = APIEndPoint+"/login",
        });
    }

    public Task<T?> RegisterAsync<T>(RegisterationRequestDTO registerationRequestDTO)
    {
        return SendAsync<T>(new ApiRequest
        {
            ApiType = SD.ApiType.POST,
            Data = registerationRequestDTO,
            Url = APIEndPoint+"/register",
        });
    }
}
