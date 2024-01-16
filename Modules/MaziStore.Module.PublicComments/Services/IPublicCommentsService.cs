using MaziStore.Module.PublicComments.Areas.PublicComments.ViewModels;
using MaziStore.Module.PublicComments.Models;
using System.Collections.Generic;

namespace MaziStore.Module.PublicComments.Services
{
   public interface IPublicCommentsService
   {
      public void AddComment(PublicCommentVm model);

      public IList<PublicComment> List();

      public void RemoveComment(int commentId);
   }
}
