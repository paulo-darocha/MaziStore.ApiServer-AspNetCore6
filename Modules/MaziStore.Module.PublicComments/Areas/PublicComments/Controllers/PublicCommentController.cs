using MaziStore.Module.PublicComments.Areas.PublicComments.ViewModels;
using MaziStore.Module.PublicComments.Services;
using Microsoft.AspNetCore.Mvc;

namespace MaziStore.Module.PublicComments.Areas.PublicComments.Controllers
{
   [ApiController]
   [Area("PublicComments")]
   [Route("api/[controller]")]
   public class PublicCommentController : ControllerBase
   {
      private readonly IPublicCommentsService _publicCommentService;

      public PublicCommentController(IPublicCommentsService publicCommentService)
      {
         _publicCommentService = publicCommentService;
      }

      [HttpGet]
      public IActionResult List()
      {
         var list = _publicCommentService.List();
         return Ok(list);
      }

      [HttpPost]
      public IActionResult AddComment(PublicCommentVm model)
      {
         if (model == null)
         {
            return BadRequest("Comment cannot be null");
         }
         _publicCommentService.AddComment(model);

         return Ok();
      }

      [HttpDelete("{commentId}")]
      public IActionResult DeleteComment(int commentId)
      {
         if (commentId != 0)
         {
            _publicCommentService.RemoveComment(commentId);
         }

         return Ok();
      }
   }
}
