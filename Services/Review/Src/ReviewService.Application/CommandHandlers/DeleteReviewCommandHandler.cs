﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abstraction.Handler;
using ReviewService.Abstraction.Command;
using ReviewService.Domain.Entities;
using ReviewService.Infrastructure.Context;
using Data.UnitOfWork;
using MediatR;
using Simple.OData.Client;

namespace ReviewService.Application.CommandHandlers
{
    public class DeleteReviewCommandHandler : ICommandHandler<DeleteReviewCommand, Unit>
    {
        private readonly IUnitOfWork<ReviewDbContext> _unitOfWork;
        private readonly ODataClient _oDataClient;
        

        public DeleteReviewCommandHandler(IUnitOfWork<ReviewDbContext> unitOfWork, ODataClient oDataClient)
        {
            _unitOfWork = unitOfWork;
            _oDataClient = oDataClient;
        }

        public async Task<Unit> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _unitOfWork.GetRepository<Review>()
                .GetFirstAsync(x => x.ReviewId == request.ReviewId, cancellationToken).ConfigureAwait(false);
            if (review == null)
            {
                throw new Exception($"Record does not exist. ReviewId: {request.ReviewId} ");
            }
            _unitOfWork.GetRepository<Review>().Remove(review);
            await _unitOfWork.Complete(); //TODO move to postProcessor
            return Unit.Value;
        }
    }
}