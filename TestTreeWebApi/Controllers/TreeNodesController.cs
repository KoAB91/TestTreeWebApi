using Microsoft.AspNetCore.Mvc;
using TestTreeWebApi.Services;
using TestTreeWebApi.ServiceModel;
using System.Collections.Generic;
using TestTreeWebApi.ApiModel;

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

        [HttpGet("{id}")]
        public ActionResult<TreeNodeResponse> GetById(long id)
        {
            var treeNode = _service.GetById(id);
            if (treeNode == null)
            {
                return NotFound();
            }
            return Ok(new TreeNodeResponse(treeNode));
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
        public ActionResult<TreeNodeResponse> UpdateName(string name, TreeNodeUpdateRequest request)
        {
            var treeNode = new TreeNodeDTO(request);
            treeNode = _service.Update(name, treeNode);
            if (treeNode == null)
            {
                return NotFound();
            }
            return Ok(new TreeNodeResponse(treeNode));
        }

        [HttpPost]
        public ActionResult<TreeNodeResponse> AddToRoot(TreeNodeCreateRequest request)
        {
            var treeNode = new TreeNodeDTO(request);
            treeNode = _service.Create(null, treeNode);
            return Ok(new TreeNodeResponse(treeNode));
        }

        [HttpPost("{name}/add-child")]
        public ActionResult<TreeNodeResponse> AddChildNode(string name, TreeNodeCreateRequest request)
        {
            var treeNode = new TreeNodeDTO(request);
            treeNode = _service.Create(name, treeNode);
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
