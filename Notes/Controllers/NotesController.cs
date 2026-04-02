using Microsoft.AspNetCore.Mvc;
using Notes.DTO.ResponseDto;
using Notes.Interfaces;
using static Notes.DTO.NotesResponseDto.NotesResponseDto;
[ApiController]
[Route("api/v1/[controller]")]  
[ApiExplorerSettings(GroupName = "v1")]
public class NotesController : ControllerBase
{
    private readonly INotesService _service;
    public NotesController(INotesService service)
    {
        _service = service;
    }
    [HttpPost]
    public async Task<ActionResult<CreatedNoteDto>> Create(
        CreateNoteDto dto,
        CancellationToken cancellationToken)
    {
        var note = await _service.CreateNoteAsync(dto, cancellationToken);
        return Created(string.Empty, note);
    }
    [HttpGet("{externalNoteReference:guid}")]
    public async Task<IActionResult> GetById(
        Guid externalNoteReference,
        CancellationToken cancellationToken)
    {
        var note = await _service.GetNoteByIdAsync(externalNoteReference, cancellationToken);
        return Ok(note);
    }
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateNoteDto dto,
        CancellationToken cancellationToken)
    {
        await _service.UpdateNoteAsync(id, dto, cancellationToken);
        return NoContent();
    }
    [HttpDelete("{externalNoteReference:guid}")]
    public async Task<IActionResult> Delete(
        Guid externalNoteReference,
        CancellationToken cancellationToken)
    {
        await _service.DeleteNoteAsync(externalNoteReference, cancellationToken);
        return NoContent();
    }
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string sortOrder = "desc",
        [FromQuery] string sortBy = "createdAt",
        CancellationToken cancellationToken = default)
    {
        if (searchTerm.Length < 3)
            return BadRequest("Search text should be more then 2 letters");

        var result = await _service.SearchNotesAsync(
            searchTerm, page, pageSize, sortOrder, sortBy, cancellationToken);
        return Ok(result);
    }
    [HttpGet("getall")]
    public async Task<IActionResult> GetAllNotes(
        CancellationToken cancellationToken)
    {
        var notes = await _service.GetAllNotes(cancellationToken);
        return Ok(notes);
    }
}
