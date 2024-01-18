using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.PublicComments.Areas.PublicComments.ViewModels;
using MaziStore.Module.PublicComments.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MaziStore.Module.PublicComments.Services
{
   public class PublicCommentsService : IPublicCommentsService
   {
      private readonly IRepository<PublicComment> _publicCommentRepository;

      public PublicCommentsService(
         IRepository<PublicComment> publicCommentRepository
      )
      {
         _publicCommentRepository = publicCommentRepository;
      }

      public void AddComment(PublicCommentVm model)
      {
         var comment = new PublicComment
         {
            Comment = model.Comment,
            Title = model.Title,
            Name = model.Name,
            Email = model.Email,
            CreatedOn = DateTime.Now
         };
         _publicCommentRepository.AddRp(comment);
         _publicCommentRepository.SaveChangesRp();
      }

      public void RemoveComment(int commentId)
      {
         var comment = _publicCommentRepository
            .QueryRp()
            .FirstOrDefault(x => x.Id == commentId);
         if (comment != null)
         {
            _publicCommentRepository.RemoveRp(comment);
            _publicCommentRepository.SaveChangesRp();
         }
      }

      public IList<PublicComment> List()
      {
         return _publicCommentRepository.QueryRp().ToList();
      }
   }
}
