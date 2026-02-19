using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTO;

namespace RoyalVilla_API.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public AuthService(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<UserDTO?> RegisterAsync(RegisterationRequestDTO registerationRequestDTO)
    {
        try
        {
            if (await IsEmailExistsAsync(registerationRequestDTO.Email))
            {
                return null;
            }
            User user = new()
            {
                Email = registerationRequestDTO.Email,
                Name = registerationRequestDTO.Name,
                Password = registerationRequestDTO.Password,
                Role = string.IsNullOrEmpty(registerationRequestDTO.Role) ? "Customer" : registerationRequestDTO.Role,
                CreatedDate = DateTime.Now
            };

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            return _mapper.Map<UserDTO>(user);
        }
        catch (Exception ex)
        {
            // Handle any other unexpected errors

            throw new InvalidOperationException("An unexcepted error occured during user registration", ex);
        }
    }

    public Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
    {

    }

    public async Task<bool> IsEmailExistsAsync(string email)
    {
        return await _db.Users.AnyAsync(u => u.Email.ToLower()==email.ToLower());
    }
}
