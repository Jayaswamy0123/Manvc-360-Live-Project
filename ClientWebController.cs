[AllowAnonymous]
[HttpGet("[action]")]
public async Task<IActionResult> DownloadPdfGenerationForPMService()
{
    try
    {
        var pdfLinks = await _context.Assetwise_Technician
            .Where(Assetwise_Technician => !string.IsNullOrEmpty(Assetwise_Technician.filePath))
            .Select(Assetwise_Technician => Assetwise_Technician.filePath)
            .ToListAsync();

        if (pdfLinks == null || pdfLinks.Count == 0)
        {
            return BadRequest("No PDF links found.");
        }

        var zipFileName = $"Ticket_PDFs_{DateTime.Now:yyyyMMddHHmmss}.zip";

        using (var memoryStream = new MemoryStream())
        {
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                using (var httpClient = new HttpClient())
                {
                    foreach (var pdfLink in pdfLinks)
                    {
                        try
                        {
                            
                            var pdfBytes = await httpClient.GetByteArrayAsync(pdfLink);

                            var pdfFileName = $"{Guid.NewGuid()}.pdf";
e
                            var entry = archive.CreateEntry(pdfFileName, CompressionLevel.Fastest);
                          
                            using (var entryStream = entry.Open())
                            {
                                await entryStream.WriteAsync(pdfBytes, 0, pdfBytes.Length);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error downloading PDF from {pdfLink}: {ex.Message}");
                        }
                    }
                }
            }

            Response.Headers.Add("Content-Disposition", $"attachment; filename={zipFileName}");
            Response.Headers.Add("Content-Type", "application/zip");
            memoryStream.Seek(0, SeekOrigin.Begin);

            return File(memoryStream.ToArray(), "application/zip", zipFileName);
        }
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}
