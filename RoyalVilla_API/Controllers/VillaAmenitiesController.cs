using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla.DTO;
using Microsoft.AspNetCore.Authorization;

namespace RoyalVilla_API.Controllers;

[Route("api/villa-amenities")]
[ApiController]
//[Authorize(Roles = "Customer,Admin")]
[Authorize]
public class VillaAmenitiesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public VillaAmenitiesController(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<VillaAmenitiesDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    // [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VillaAmenitiesDTO>>>> GetVillaAmenities()
    {
        var villaAmenities = await _db.VillaAmenities.Include(u => u.Villa).ToListAsync();

        if (villaAmenities == null || villaAmenities.Count == 0)
        {
            return NotFound(ApiResponse<object>.NotFound());
        }

        var dtoResponseVillaAmenities = _mapper.Map<List<VillaAmenitiesDTO>>(villaAmenities);

        var response = ApiResponse<IEnumerable<VillaAmenitiesDTO>>.Ok(dtoResponseVillaAmenities, "Villas amenities retrieved successfully");

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    // [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<VillaAmenitiesDTO>>> GetVillaAmenitiesById(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(ApiResponse<object>.BadRequest("Villa amenities ID must be greater than 0"));
            }
            var villaAmenities = await _db.VillaAmenities.Include(u=>u.Villa).FirstOrDefaultAsync(v => v.Id == id);

            if (villaAmenities == null)
            {
                return NotFound(ApiResponse<object>.NotFound($"Villa amenities with ID {id} was not found"));
            }

            return Ok(ApiResponse<VillaAmenitiesDTO>.Ok(_mapper.Map<VillaAmenitiesDTO>(villaAmenities), "Records retrieved successfully"));
        }
        catch (Exception ex)
        {
            var errorResponse = ApiResponse<object>.Error(500, $"An error occured while retrieving villa amenities with ID {id}: ", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<VillaAmenitiesDTO>>> CreateVillaAmenities(VillaAmenitiesCreateDTO villaAmenitiesDTO)
    {
        try
        {
            if (villaAmenitiesDTO == null)
            {
                return BadRequest(ApiResponse<object>.BadRequest("Villa amenities data is required"));
            }

            var villaExists = await _db.Villas.FirstOrDefaultAsync(v => v.Id == villaAmenitiesDTO.VillaId);

            if (villaExists == null)
            {
                return Conflict(ApiResponse<object>.Conflict($"A villa with the ID '{villaAmenitiesDTO.VillaId}' does not exists"));
            }

            VillaAmenities villaAmenities = _mapper.Map<VillaAmenities>(villaAmenitiesDTO);
            villaAmenities.CreatedDate = DateTime.Now;
            await _db.VillaAmenities.AddAsync(villaAmenities);
            await _db.SaveChangesAsync();

            var response = ApiResponse<object>.CreatedAt(_mapper.Map<VillaAmenitiesDTO>(villaAmenities), "Villa amenities created successfully");

            return CreatedAtAction(nameof(GetVillaAmenitiesById), new { id = villaAmenities.Id }, response);
        }
        catch (Exception ex)
        {
            var errorResponse = ApiResponse<object>.Error(500, "An error occured while creating the villa amenities: ", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<VillaAmenitiesUpdateDTO>>> UpdateVillaAmenities(int id, VillaAmenitiesUpdateDTO villaAmenitiesDTO)
    {
        try
        {
            if (villaAmenitiesDTO == null)
            {
                return BadRequest(ApiResponse<object>.BadRequest("Villa amenities data is required"));
            }

            if (id != villaAmenitiesDTO.Id)
            {
                return BadRequest(ApiResponse<object>.BadRequest($"Villa amenities ID in URL does not match Villa amenities ID in request body"));
            }

            var existingVillaAmenities = await _db.VillaAmenities.FirstOrDefaultAsync(u => u.Id == id);

            if (existingVillaAmenities == null)
            {
                return NotFound(ApiResponse<object>.NotFound($"Villa amenities with ID {id} was not found"));
            }


            var villaExists = await _db.Villas.FirstOrDefaultAsync(v => v.Id == villaAmenitiesDTO.VillaId);

            if (villaExists == null)
            {
                return Conflict(ApiResponse<object>.Conflict($"A villa with the ID '{villaAmenitiesDTO.VillaId}' does not exists"));
            }

            _mapper.Map(villaAmenitiesDTO, existingVillaAmenities);
            existingVillaAmenities.UpdatedDate = DateTime.Now;
            await _db.SaveChangesAsync();

            var response = ApiResponse<VillaAmenitiesDTO>.Ok(_mapper.Map<VillaAmenitiesDTO>(existingVillaAmenities), "Villa amenities updated successfully");

            return Ok(response);
        }
        catch (Exception ex)
        {
            var errorResponse = ApiResponse<object>.Error(500, "An error occured while updating the villa amenities: ", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteVillaAmenities(int id)
    {
        try
        {
            var existingVillaAmenities = await _db.VillaAmenities.FirstOrDefaultAsync(u => u.Id == id);

            if (existingVillaAmenities == null)
            {
                return NotFound(ApiResponse<object>.NotFound($"Villa amenities with ID {id} was not found"));
            }

            _db.VillaAmenities.Remove(existingVillaAmenities);
            await _db.SaveChangesAsync();
            return Ok(ApiResponse<object>.NoContent("Villa amenities deleted successfully"));

        }
        catch (Exception ex)
        {
            var errorResponse = ApiResponse<object>.Error(500, "An error occured while deleting the villa amenities: ", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }

}
