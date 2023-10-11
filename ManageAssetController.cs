[HttpPost("[action]")]
        public async Task<ActionResult> UpdateAssetSubCategory(int id,[FromBody] AddAssetSubCategory model)
        {
            try
            {
                var result = await _context.Asset_SubCategory
                    .FirstOrDefaultAsync(x => x.AssetSubCategory_ID == model.assetCategoryID);

                if (result == null)
                {
                    return NotFound("Asset_SubCategory not found");
                }

                result.Asset_Sub_Category = model.assetSubCategotyType;
                result.IsActive = model.isActive;
                result.Last_Updated_Date = model.Last_Updated_Date;
                _context.Update(result);
                await _context.SaveChangesAsync();

                return Ok("Asset_SubCategory updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
