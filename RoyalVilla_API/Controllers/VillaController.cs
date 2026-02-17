using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;

namespace RoyalVilla_API.Controllers;

[Route("api/villa")]
[ApiController]
public class VillaController : ControllerBase
{
    protected ApplicationDbContext _db;

    public VillaController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Villa>> GetVillas()
    {
        var villas = _db.Villas.ToList();

        if (villas == null || villas.Count == 0)
        {
            return NotFound();
        }

        return Ok(villas);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Villa> GetVillaById(int id)
    {
        var villa = _db.Villas.FirstOrDefault(v => v.Id == id);

        if (villa == null)
        {
            return NotFound();
        }

        return Ok(villa);
    }

}
