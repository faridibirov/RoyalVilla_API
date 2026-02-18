using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTO;

namespace RoyalVilla_API.Controllers;

[Route("api/villa")]
[ApiController]
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
    public async  Task<ActionResult<IEnumerable<Villa>>> GetVillas()
    {
        var villas = await _db.Villas.ToListAsync();

        if (villas == null || villas.Count == 0)
        {
            return NotFound();
        }

        return Ok(villas);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Villa>> GetVillaById(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Villa ID must be greated than 0");
            }
            var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);

            if (villa == null)
            {
                return NotFound($"Villa with ID {id} was not found");
            }

            return Ok(villa);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                $"An error occured while retrieving villa with ID {id}: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Villa>> CreateVilla(VillaCreateDTO villaDTO)
    {
        try
        {
            if (villa == null)
            {
                return BadRequest($"Villa data is required");
            }

            Villa villa = _mapper.Map<Villa>(villaDTO);

            await _db.Villas.AddAsync(villa);
            _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVillaById), new {id=villa.Id}, villa);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"An error occured while creating the villa: {ex.Message}");
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Villa>> UpdateVilla(int id, VillaUpdateDTO villaDTO)
    {
        try
        {
            if (villa == null)
            {
                return BadRequest($"Villa data is required");
            }

            if(id!=villaDTO.Id)
            {
                return BadRequest($"Villa ID in URL does not match Villa ID in request body");
            }
            
            var existingVilla = await _db.Villas.FirstOrDefaultAsync(u=>u.Id==id);

            if (existingVilla == null)
            {
                return NotFound($"Villa with ID {id} was not found");
            }


           _mapper.Map(villaDTO, existingVilla);
            existingVilla.UpdatedDate = DateTime.Now;
            _db.SaveChangesAsync();

            return Ok(villaDTO);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"An error occured while updating the villa: {ex.Message}");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Villa>> DeleteVilla(int id)
    {
        try
        {
            var existingVilla = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);

            if (existingVilla == null)
            {
                return NotFound($"Villa with ID {id} was not found");
            }

            _db.Villas.Remove(existingVilla);
            await _db.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"An error occured while deleting the villa: {ex.Message}");
        }
    }

}
