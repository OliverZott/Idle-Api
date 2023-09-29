namespace IdleApi.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class StatusController : ControllerBase
{
    private readonly Status _status;

    public StatusController(Status status)
    {
        _status = status;
    }


    [HttpGet]
    public async Task<string> GetStatus()
    {
        return await Task.FromResult(_status.State);
    }

    [HttpPost]
    public async Task<string> PostStatus([FromQuery] string state)
    {
        _status.State = state;
        return await Task.FromResult(_status.State);
    }
}

public class Status
{
    public string State { get; set; } = "idle";

}
