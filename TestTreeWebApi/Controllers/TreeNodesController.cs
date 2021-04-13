using Microsoft.AspNetCore.Mvc;
using TestTreeWebApi.Services;
using TestTreeWebApi.ServiceModels;
using System.Collections.Generic;
using TestTreeWebApi.ApiModels;

namespace TestTreeWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreeNodesController : ControllerBase
    {
        private readonly TreeNodeService _service;

        public TreeNodesController(TreeNodeService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<TreeNodeResponse>> Get()
        {
            var treeNodes = _service.GetAll();
            if (treeNodes == null)
            {
                return NotFound();
            }
            var responses = new List<TreeNodeResponse>();
            foreach(TreeNodeDTO treeNode in treeNodes)
            {
                responses.Add(new TreeNodeResponse(treeNode));
            }
            return Ok(responses);
        }

        [HttpGet("by-name/{name}")]
        public ActionResult<TreeNodeResponse> GetByName(string name)
        {
            var treeNode = _service.GetByName(name);
            if (treeNode == null)
            {
                return NotFound();
            }
            return Ok(new TreeNodeResponse(treeNode));
        }

        [HttpGet("{name}/path")]
        public ActionResult<PathResponse> GetPath(string name)
        {
            var path = _service.GetPath(name);
            if (path == null)
            {
                return NotFound();
            }
            return Ok(new PathResponse(path));
        }

        [HttpPut("{name}")]
        public ActionResult<TreeNodeResponse> UpdateName(string name, TreeNodeUpdateNameRequest request)
        {
            var treeNode = _service.UpdateName(name, request.Name);
            if (treeNode == null)
            {
                return NotFound();
            }
            return Ok(new TreeNodeResponse(treeNode));
        }

        [HttpPut("{name}/update-parent")]
        public ActionResult<TreeNodeResponse> UpdateParent(string name, TreeNodeUpdateParentRequest request)
        {
            TreeNodeDTO treeNode;
            try
            {
                treeNode = _service.UpdateParent(name, request.ParentName);
            }
            catch (TreeNodeMoveException e)
            {
                return Conflict(e.Message);
            }
            catch (TreeNodeCreateException ex)
            {
                return NotFound(ex.Message);
            }
            return Ok(new TreeNodeResponse(treeNode));
        }

        [HttpPost]
        public ActionResult<TreeNodeResponse> AddToRoot(TreeNodeCreateRequest request)
        {
            var treeNode = new TreeNodeDTO()
            {
                Name = request.Name
            };
            try
            {
                treeNode = _service.Create(treeNode);
            }
            catch (TreeNodeCreateException e)
            {
                return Conflict(e.Message);
            }
            return Ok(new TreeNodeResponse(treeNode));
        }

        [HttpPost("{name}/add-child")]
        public ActionResult<TreeNodeResponse> AddChildNode(string name, TreeNodeCreateRequest request)
        {
            var treeNode = new TreeNodeDTO()
            {
                Name = request.Name
            };
            try
            {
                treeNode = _service.Create(name, treeNode);
            }
            catch (TreeNodeCreateException e)
            {
                return Conflict(e.Message);
            }
            if (treeNode == null)
            {
                return NotFound();
            }
            return Ok(new TreeNodeResponse(treeNode));
        }

        [HttpDelete("{name}")]
        public IActionResult Delete(string name)
        {
            var response = _service.Delete(name);
            if (response == Enums.DeleteResponse.NOT_FOUND)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
