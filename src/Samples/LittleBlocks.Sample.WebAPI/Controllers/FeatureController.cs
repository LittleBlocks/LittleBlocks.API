﻿namespace LittleBlocks.Sample.WebAPI.Controllers;

[Route("api/[controller]")]
public class FeatureController : Controller
{
    private readonly IFeatureManager _featureManager;

    public FeatureController(IFeatureManager featureManager)
    {
        _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetFeatureAvailability(string name)
    {
        if (!await _featureManager.GetFeatureNamesAsync().AnyAsync(m => m == name))
            return NotFound(false);

        return Ok(await _featureManager.IsEnabledAsync(name));
    }
}
