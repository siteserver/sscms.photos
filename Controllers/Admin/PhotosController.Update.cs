﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Photos.Core;

namespace SSCMS.Photos.Controllers.Admin
{
    public partial class PhotosController
    {
        [HttpPost, Route(RouteUpdate)]
        public async Task<ActionResult<BoolResult>> Update([FromBody] UpdateRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, PhotoManager.PermissionsContent))
                return Unauthorized();

            if (request.Type == "description")
            {
                await _photoRepository.UpdateDescriptionAsync(request.PhotoId, request.Description);
            }
            else if (request.Type == "taxis")
            {
                await _photoRepository.UpdateTaxisAsync(request.PhotoIds);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
