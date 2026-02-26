using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla.DTO;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace RoyalVilla_API.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public AuthService(ApplicationDbContext db, IMapper mapper, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
	{
		_db = db;
		_mapper = mapper;
		_configuration = configuration;
		_userManager = userManager;
		_roleManager = roleManager;
	}

	public async Task<UserDTO?> RegisterAsync(RegisterationRequestDTO registerationRequestDTO)
    {
        try
        {
            if (await IsEmailExistsAsync(registerationRequestDTO.Email))
            {
                return null;
            }
            ApplicationUser user = new()
            {
                Email = registerationRequestDTO.Email,
                Name = registerationRequestDTO.Name,
                UserName = registerationRequestDTO.Email,
                NormalizedEmail = registerationRequestDTO.Email.ToUpper()
            };

            var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e=>e.Description));
				throw new InvalidOperationException("User registration failed: "+ errors);
			}

            var role = string.IsNullOrEmpty(registerationRequestDTO.Role) ? "Customer" : registerationRequestDTO.Role;

            if(!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

            await _userManager.AddToRoleAsync(user, role);

            var userDto = _mapper.Map<userDto>(user);
            userDto.Role = role;

            return userDto;

        }
        catch (Exception ex)
        {
            // Handle any other unexpected errors

            throw new InvalidOperationException("An unexcepted error occured during user registration", ex);
        }
    }


    public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
    {
        try
        {
            var user = await _db.AppliationUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == loginRequestDTO.Email.ToLower());
            
            if (user == null )
            {
                return null; //user is not found
            }

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (!isValid)
            {
                return null; //invalid password
            }

            //generate TOKEN

            var token = await GenerateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);    
            var loginResponseDTO =   new LoginResponseDTO
            {
                UserDTO = _mapper.Map<UserDTO>(user),
                Token = token
            };

            loginResponseDTO.UserDTO.Role = roles.FirstOrDefault()??"Customer";

            return loginResponseDTO;

        }
        catch (Exception ex)
        {
            // Handle any other unexpected errors

            throw new InvalidOperationException("An unexcepted error occured during user login", ex);
        }
    }


    public async Task<bool> IsEmailExistsAsync(string email)
    {
        return await _db.AppliationUsers.AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    private async string GenerateJwtToken (ApplicationUser user)
    {
        var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtSettings")["Secret"]);
        var roles = await _userManager.GetRolesAsync(user); 
        var tokenDescriptor = new SecurityTokenDescriptor{
            Subject = new ClaimsIdentity(new[]
            {
                new Claim (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim (ClaimTypes.Email, user.Email),
                new Claim (ClaimTypes.Name, user.Name),
                new Claim (ClaimTypes.Role, roles.FirstOrDefault()),
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
