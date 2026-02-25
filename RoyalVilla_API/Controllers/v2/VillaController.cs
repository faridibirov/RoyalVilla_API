using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla.DTO;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;


namespace RoyalVilla_API.Controllers.v2;

[Route("api/v{version:apiVersion}/villa")]
[ApiVersion("2.0")]
[ApiController]
//[Authorize]
public class VillaController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public VillaController(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<String>> GetVillas()
    {

        return "This is V2";
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<String>> GetVillaById(int id)
    {
		return "This is V2 for ID: "+id;
	}

}
